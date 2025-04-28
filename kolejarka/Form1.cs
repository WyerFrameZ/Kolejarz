using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kolejarka
{
    public partial class Form1 : Form
    {
        private readonly MySqlConnection connection;
        private readonly Button[] stationButtons;
        private readonly Button[] cnButtons;
        private const decimal PRICE_PER_STATION = 5.00M;
        private const int MAX_RETRY_ATTEMPTS = 3;
        private const int RETRY_DELAY_MS = 1000;
        private const string CONNECTION_STRING = "server=localhost;port=3307;user=root;password=ZSKZSKZSK;database=kolejarz;";

        public Form1()
        {
            InitializeComponent();
            connection = new MySqlConnection(CONNECTION_STRING);
            stationButtons = new Button[] { button1, button2, button3, button4, button5, button6, button7, button8, button9 };
            cnButtons = new Button[9];
            
            InitializeStationButtons();
            InitializeCNButtons();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await Task.Run(() =>
            {
                ConnectToDatabase();
                EnsureCNColumn();
                EnsureRoutes();
                LoadStations();
                LoadCNComboBox();
            });
        }

        private void InitializeCNButtons()
        {
            for (int i = 0; i < 9; i++)
            {
                cnButtons[i] = new Button
                {
                    Size = new Size(30, 30),
                    Text = "CN",
                    BackColor = Color.Yellow,
                    FlatStyle = FlatStyle.Flat,
                    Tag = i
                };
                cnButtons[i].Click += CNButton_Click;
                Controls.Add(cnButtons[i]);
            }
        }

        private void InitializeStationButtons()
        {
            foreach (var button in stationButtons)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.BackColor = Color.LightBlue;
                button.Click += StationButton_Click;
            }
        }

        private async Task ConnectToDatabase()
        {
            int retryCount = 0;
            while (retryCount < MAX_RETRY_ATTEMPTS)
            {
                try
                {
                    await connection.OpenAsync();
                    UpdateConnectionStatus(true);
                    return;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount == MAX_RETRY_ATTEMPTS)
                    {
                        UpdateConnectionStatus(false, ex.Message);
                    }
                    else
                    {
                        await Task.Delay(RETRY_DELAY_MS);
                    }
                }
            }
        }

        private void UpdateConnectionStatus(bool isConnected, string errorMessage = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateConnectionStatus(isConnected, errorMessage)));
                return;
            }

            label2.Text = isConnected ? "Stan bazy danych: Połączono" : "Stan bazy danych: Błąd połączenia";
            label2.ForeColor = isConnected ? Color.Green : Color.Red;
            
            if (!isConnected && errorMessage != null)
            {
                MessageBox.Show($"Błąd połączenia po {MAX_RETRY_ATTEMPTS} próbach: {errorMessage}");
            }
        }

        private async Task LoadCNComboBox()
        {
            try
            {
                if (InvokeRequired)
                {
                    await Invoke(async () => await LoadCNComboBox());
                    return;
                }

                cnComboBox.Items.Clear();
                const string query = "SELECT DISTINCT cn FROM stations WHERE cn IS NOT NULL ORDER BY cn";
                
                using var cmd = new MySqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    cnComboBox.Items.Add(reader.GetString("cn"));
                }

                if (cnComboBox.Items.Count > 0)
                {
                    cnComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas ładowania numerów CN: " + ex.Message);
            }
        }

        private async void cnComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cnComboBox.SelectedItem == null) return;

            try
            {
                string selectedCN = cnComboBox.SelectedItem.ToString();
                const string query = "UPDATE stations SET cn = @cn WHERE cn IS NULL";
                
                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@cn", selectedCN);
                
                int updatedRows = await cmd.ExecuteNonQueryAsync();
                if (updatedRows > 0)
                {
                    MessageBox.Show($"Zaktualizowano {updatedRows} stacji numerem CN: {selectedCN}");
                    await LoadStationsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas aktualizacji numerów CN: " + ex.Message);
            }
        }

        private async Task LoadStationsAsync()
        {
            if (!await EnsureDatabaseConnectionAsync()) return;

            try
            {
                if (InvokeRequired)
                {
                    await Invoke(async () => await LoadStationsAsync());
                    return;
                }

                const string query = "SELECT * FROM stations ORDER BY station_order";
                using var cmd = new MySqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                int buttonIndex = 0;
                while (await reader.ReadAsync() && buttonIndex < stationButtons.Length)
                {
                    if (!reader.IsDBNull(reader.GetOrdinal("name")))
                    {
                        UpdateStationButton(buttonIndex, reader);
                        buttonIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas ładowania stacji: " + ex.Message);
            }
        }

        private void UpdateStationButton(int buttonIndex, MySqlDataReader reader)
        {
            stationButtons[buttonIndex].Text = reader.GetString("name");
            stationButtons[buttonIndex].Tag = reader.GetInt32("id");
            
            Point stationLocation = stationButtons[buttonIndex].Location;
            cnButtons[buttonIndex].Location = new Point(
                stationLocation.X + stationButtons[buttonIndex].Width + 5,
                stationLocation.Y
            );
            cnButtons[buttonIndex].Tag = reader.GetInt32("id");
            cnButtons[buttonIndex].Text = reader.IsDBNull(reader.GetOrdinal("cn")) 
                ? "Brak CN" 
                : "CN: " + reader.GetString("cn");
        }

        private async Task<bool> EnsureDatabaseConnectionAsync()
        {
            if (connection.State != ConnectionState.Open)
            {
                try
                {
                    await ConnectToDatabase();
                    return connection.State == ConnectionState.Open;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nie można połączyć z bazą danych: " + ex.Message);
                    return false;
                }
            }
            return true;
        }

        private async void StationButton_Click(object sender, EventArgs e)
        {
            if (sender is not Button button || button.Tag == null) return;

            try
            {
                int stationId = Convert.ToInt32(button.Tag);
                using var destinationForm = new DestinationSelectionForm(connection, stationId, button.Text);
                
                if (destinationForm.ShowDialog() == DialogResult.OK)
                {
                    await ShowRouteInformationAsync(stationId, destinationForm.SelectedDestinationId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wyboru stacji: " + ex.Message);
            }
        }

        private async Task ShowRouteInformationAsync(int fromStationId, int toStationId)
        {
            if (!await EnsureDatabaseConnectionAsync()) return;

            string query = @"
                SELECT r.*, 
                       s1.name as from_station_name,
                       s2.name as to_station_name,
                       ABS(s2.station_order - s1.station_order) as stations_between
                FROM routes r
                JOIN stations s1 ON r.from_station_id = s1.id
                JOIN stations s2 ON r.to_station_id = s2.id
                WHERE r.from_station_id = @fromId AND r.to_station_id = @toId
                ORDER BY r.departure_time
                LIMIT 1";

            try
            {
                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@fromId", fromStationId);
                cmd.Parameters.AddWithValue("@toId", toStationId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    int stationsBetween = reader.GetInt32("stations_between");
                    decimal price = stationsBetween * PRICE_PER_STATION;

                    string message = $"Trasa: {reader["from_station_name"]} -> {reader["to_station_name"]}\n" +
                                   $"Odjazd: {reader["departure_time"]}\n" +
                                   $"Przyjazd: {reader["arrival_time"]}\n" +
                                   $"Liczba stacji: {stationsBetween}\n" +
                                   $"Cena biletu: {price:C2}";

                    var result = MessageBox.Show(
                        message + "\n\nCzy chcesz zarezerwować bilet?",
                        "Informacje o trasie",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        await BookTicketAsync(fromStationId, toStationId, price);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Brak dostępnych połączeń na wybranej trasie.\n\n" +
                        "Czy chcesz dodać nowe połączenie?",
                        "Brak połączeń",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas pobierania informacji o trasie: " + ex.Message);
            }
        }

        private async Task BookTicketAsync(int fromStationId, int toStationId, decimal price)
        {
            if (!await EnsureDatabaseConnectionAsync()) return;

            try
            {
                string query = @"
                    INSERT INTO tickets (from_station_id, to_station_id, price, booking_date)
                    VALUES (@fromId, @toId, @price, @bookingDate)";

                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@fromId", fromStationId);
                cmd.Parameters.AddWithValue("@toId", toStationId);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@bookingDate", DateTime.Now);
                
                await cmd.ExecuteNonQueryAsync();
                MessageBox.Show("Bilet został zarezerwowany pomyślnie!", "Potwierdzenie rezerwacji");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas rezerwacji biletu: " + ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Możesz dodać dodatkową funkcjonalność dla tytułu
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // Możesz dodać dodatkową funkcjonalność dla statusu bazy danych
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        private void EnsureCNColumn()
        {
            try
            {
                // Sprawdź czy kolumna cn istnieje
                string checkColumnQuery = @"
                    SELECT COUNT(*) 
                    FROM information_schema.COLUMNS 
                    WHERE TABLE_SCHEMA = 'kolejarz' 
                    AND TABLE_NAME = 'stations' 
                    AND COLUMN_NAME = 'cn'";

                using (MySqlCommand cmd = new MySqlCommand(checkColumnQuery, connection))
                {
                    int columnExists = Convert.ToInt32(cmd.ExecuteScalar());
                    if (columnExists == 0)
                    {
                        // Dodaj kolumnę cn
                        string addColumnQuery = "ALTER TABLE stations ADD COLUMN cn VARCHAR(50)";
                        using (MySqlCommand alterCmd = new MySqlCommand(addColumnQuery, connection))
                        {
                            alterCmd.ExecuteNonQuery();
                        }

                        // Dodaj przykładowe numery CN
                        string[] sampleCNs = new[] { "CN001", "CN002", "CN003", "CN004", "CN005" };
                        string updateQuery = "UPDATE stations SET cn = @cn WHERE id = @id";
                        
                        using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection))
                        {
                            for (int i = 1; i <= 5; i++)
                            {
                                updateCmd.Parameters.Clear();
                                updateCmd.Parameters.AddWithValue("@cn", sampleCNs[i-1]);
                                updateCmd.Parameters.AddWithValue("@id", i);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas inicjalizacji kolumny CN: " + ex.Message);
            }
        }

        private void addCnButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cnTextBox.Text))
            {
                MessageBox.Show("Wprowadź numer CN!");
                return;
            }

            try
            {
                string newCN = cnTextBox.Text.Trim();
                
                // Sprawdź czy taki CN już istnieje
                string checkQuery = "SELECT COUNT(*) FROM stations WHERE cn = @cn";
                using (MySqlCommand cmd = new MySqlCommand(checkQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@cn", newCN);
                    int exists = Convert.ToInt32(cmd.ExecuteScalar());
                    if (exists > 0)
                    {
                        MessageBox.Show("Ten numer CN już istnieje w bazie!");
                        return;
                    }
                }

                // Dodaj nowy CN do pierwszej stacji bez CN
                string updateQuery = "UPDATE stations SET cn = @cn WHERE cn IS NULL LIMIT 1";
                using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@cn", newCN);
                    int affected = cmd.ExecuteNonQuery();
                    if (affected > 0)
                    {
                        MessageBox.Show("Dodano nowy numer CN!");
                        cnTextBox.Clear();
                        LoadCNComboBox(); // Odśwież listę CN
                    }
                    else
                    {
                        MessageBox.Show("Nie znaleziono stacji bez numeru CN!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas dodawania numeru CN: " + ex.Message);
            }
        }

        private void EnsureRoutes()
        {
            try
            {
                // Sprawdź czy są jakieś trasy
                string checkQuery = "SELECT COUNT(*) FROM routes";
                using (MySqlCommand cmd = new MySqlCommand(checkQuery, connection))
                {
                    int routeCount = Convert.ToInt32(cmd.ExecuteScalar());
                    if (routeCount == 0)
                    {
                        // Dodaj przykładowe trasy
                        string insertQuery = @"
                            INSERT INTO routes (from_station_id, to_station_id, departure_time, arrival_time) 
                            VALUES 
                            (1, 2, '08:00:00', '09:00:00'),
                            (2, 3, '09:30:00', '10:30:00'),
                            (3, 4, '11:00:00', '12:00:00'),
                            (4, 5, '12:30:00', '13:30:00'),
                            (5, 1, '14:00:00', '15:00:00')";
                        using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection))
                        {
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas inicjalizacji tras: " + ex.Message);
            }
        }

        private void CNButton_Click(object sender, EventArgs e)
        {
            if (sender is Button cnButton && cnButton.Tag != null)
            {
                int stationId = Convert.ToInt32(cnButton.Tag);
                string newCN = Microsoft.VisualBasic.Interaction.InputBox(
                    "Wprowadź nowy numer CN dla stacji:",
                    "Edycja numeru CN",
                    cnButton.Text.StartsWith("CN: ") ? cnButton.Text.Substring(4) : "");

                if (!string.IsNullOrWhiteSpace(newCN))
                {
                    try
                    {
                        string updateQuery = "UPDATE stations SET cn = @cn WHERE id = @id";
                        using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@cn", newCN);
                            cmd.Parameters.AddWithValue("@id", stationId);
                            cmd.ExecuteNonQuery();
                            
                            LoadStations(); // Odśwież przyciski stacji
                            LoadCNComboBox(); // Odśwież listę CN
                            MessageBox.Show("Zaktualizowano numer CN!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd podczas aktualizacji numeru CN: " + ex.Message);
                    }
                }
            }
        }

        private void InitializeComponents()
        {
            // ... existing code ...

            // Dodaj przycisk zarządzania stacjami
            var manageStationsButton = new Button
            {
                Text = "Zarządzaj stacjami",
                Location = new Point(855, 200),
                Size = new Size(150, 30)
            };
            manageStationsButton.Click += ManageStationsButton_Click;
            this.Controls.Add(manageStationsButton);
        }

        private void ManageStationsButton_Click(object sender, EventArgs e)
        {
            using var manageForm = new ManageStationsForm(connection);
            if (manageForm.ShowDialog() == DialogResult.OK)
            {
                LoadStations();
            }
        }
    }

    public class DestinationSelectionForm : Form
    {
        private MySqlConnection connection;
        private ComboBox destinationComboBox;
        private Label stationInfoLabel;
        private Label routeInfoLabel;
        private Label priceLabel;
        public int SelectedDestinationId { get; private set; }
        private const decimal PRICE_PER_STATION = 5.00M;

        public DestinationSelectionForm(MySqlConnection conn, int fromStationId, string fromStationName)
        {
            connection = conn;
            InitializeComponents(fromStationName);
            LoadDestinations(fromStationId);
            LoadStationInfo(fromStationId);
        }

        private void InitializeComponents(string fromStationName)
        {
            this.Text = $"Wybierz destynację z {fromStationName}";
            this.Size = new Size(400, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            // Informacje o stacji początkowej
            stationInfoLabel = new Label();
            stationInfoLabel.AutoSize = true;
            stationInfoLabel.Location = new Point(20, 20);
            stationInfoLabel.Size = new Size(360, 60);
            this.Controls.Add(stationInfoLabel);

            // Lista dostępnych stacji
            Label destinationLabel = new Label();
            destinationLabel.Text = "Wybierz stację docelową:";
            destinationLabel.AutoSize = true;
            destinationLabel.Location = new Point(20, 100);
            this.Controls.Add(destinationLabel);

            destinationComboBox = new ComboBox();
            destinationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            destinationComboBox.Location = new Point(20, 130);
            destinationComboBox.Size = new Size(360, 25);
            destinationComboBox.SelectedIndexChanged += DestinationComboBox_SelectedIndexChanged;
            this.Controls.Add(destinationComboBox);

            // Informacje o trasie
            routeInfoLabel = new Label();
            routeInfoLabel.AutoSize = true;
            routeInfoLabel.Location = new Point(20, 170);
            routeInfoLabel.Size = new Size(360, 60);
            this.Controls.Add(routeInfoLabel);

            // Informacje o cenie
            priceLabel = new Label();
            priceLabel.AutoSize = true;
            priceLabel.Location = new Point(20, 240);
            priceLabel.Size = new Size(360, 30);
            priceLabel.Font = new Font(priceLabel.Font, FontStyle.Bold);
            this.Controls.Add(priceLabel);

            Button okButton = new Button();
            okButton.Text = "Kup bilet";
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(150, 280);
            okButton.Click += (s, e) =>
            {
                if (destinationComboBox.SelectedItem != null)
                {
                    var selectedItem = destinationComboBox.SelectedItem as dynamic;
                    SelectedDestinationId = selectedItem.id;
                    MessageBox.Show($"Dziękujemy za zakup biletu!\nŻyczymy miłej podróży!", "Bilet zakupiony", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            this.Controls.Add(okButton);
        }

        private void LoadStationInfo(int stationId)
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                stationInfoLabel.Text = "Brak połączenia z bazą danych";
                return;
            }

            string query = "SELECT * FROM stations WHERE id = @stationId";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@stationId", stationId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string name = reader["name"].ToString();
                            int order = Convert.ToInt32(reader["station_order"]);
                            
                            reader.Close();
                            cmd.CommandText = "SELECT COUNT(*) FROM routes WHERE from_station_id = @stationId";
                            int routeCount = Convert.ToInt32(cmd.ExecuteScalar());

                            cmd.CommandText = @"
                                SELECT GROUP_CONCAT(s2.name SEPARATOR ', ') 
                                FROM routes r 
                                JOIN stations s2 ON r.to_station_id = s2.id 
                                WHERE r.from_station_id = @stationId";
                            
                            object connectedStationsObj = cmd.ExecuteScalar();
                            string connectedStations = connectedStationsObj == DBNull.Value ? 
                                "Brak połączeń" : 
                                connectedStationsObj.ToString();

                            string stationInfo = $"Stacja: {name}\n" +
                                               $"Numer stacji: {order}\n" +
                                               $"Liczba połączeń: {routeCount}\n" +
                                               $"Połączone stacje: {connectedStations}";
                            stationInfoLabel.Text = stationInfo;
                        }
                        else
                        {
                            stationInfoLabel.Text = "Nie znaleziono informacji o stacji";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                stationInfoLabel.Text = "Błąd podczas ładowania informacji: " + ex.Message;
            }
        }

        private void LoadDestinations(int fromStationId)
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                MessageBox.Show("Brak połączenia z bazą danych");
                return;
            }

            string query = @"
                SELECT DISTINCT s.id, s.name, 
                       COUNT(r.id) as route_count,
                       ABS(s.station_order - (SELECT station_order FROM stations WHERE id = @fromId)) as stations_between
                FROM stations s
                LEFT JOIN routes r ON s.id = r.to_station_id AND r.from_station_id = @fromId
                WHERE s.id != @fromId
                GROUP BY s.id, s.name, s.station_order
                ORDER BY s.station_order";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@fromId", fromStationId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        var destinations = new List<dynamic>();
                        while (reader.Read())
                        {
                            destinations.Add(new
                            {
                                id = reader.GetInt32("id"),
                                name = reader.GetString("name"),
                                route_count = reader.GetInt32("route_count"),
                                stations_between = reader.GetInt32("stations_between")
                            });
                        }
                        destinationComboBox.DataSource = destinations;
                        destinationComboBox.DisplayMember = "name";
                        destinationComboBox.ValueMember = "id";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas ładowania stacji docelowych: " + ex.Message);
            }
        }

        private void DestinationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (destinationComboBox.SelectedItem != null)
            {
                try
                {
                    var selectedItem = destinationComboBox.SelectedItem as dynamic;
                    int toStationId = selectedItem.id;
                    int stationsBetween = selectedItem.stations_between;
                    decimal price = stationsBetween * PRICE_PER_STATION;
                    LoadRouteInfo(toStationId);
                    priceLabel.Text = $"Cena biletu: {price:C2} ({stationsBetween} stacji × {PRICE_PER_STATION:C2})";
                }
                catch (Exception ex)
                {
                    priceLabel.Text = "Błąd obliczania ceny: " + ex.Message;
                }
            }
        }

        private void LoadRouteInfo(int toStationId)
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                routeInfoLabel.Text = "Brak połączenia z bazą danych";
                return;
            }

            string query = @"
                SELECT r.*, 
                       s1.name as from_station_name,
                       s2.name as to_station_name
                FROM routes r
                JOIN stations s1 ON r.from_station_id = s1.id
                JOIN stations s2 ON r.to_station_id = s2.id
                WHERE r.to_station_id = @toId
                ORDER BY r.departure_time
                LIMIT 1";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@toId", toStationId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string routeInfo = $"Najbliższy odjazd: {reader["departure_time"]}\n" +
                                             $"Przyjazd: {reader["arrival_time"]}";
                            routeInfoLabel.Text = routeInfo;
                        }
                        else
                        {
                            routeInfoLabel.Text = "Brak dostępnych połączeń na tej trasie.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                routeInfoLabel.Text = "Błąd podczas ładowania informacji o trasie: " + ex.Message;
            }
        }
    }
}

