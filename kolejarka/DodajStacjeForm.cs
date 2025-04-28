using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace kolejarka
{
    public class DodajStacjeForm : Form
    {
        private readonly MySqlConnection connection;
        private readonly int? stationId;
        private TextBox nameTextBox;
        private NumericUpDown orderNumericUpDown;
        private TextBox cnTextBox;
        private Button saveButton;
        private Button cancelButton;

        public DodajStacjeForm(MySqlConnection conn, int? id = null)
        {
            connection = conn;
            stationId = id;
            InitializeComponents();
            if (stationId.HasValue)
            {
                LoadStationData();
            }
        }

        private void InitializeComponents()
        {
            this.Text = stationId.HasValue ? "Edytuj stację" : "Dodaj stację";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Nazwa stacji
            var nameLabel = new Label
            {
                Text = "Nazwa stacji:",
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(nameLabel);

            nameTextBox = new TextBox
            {
                Location = new Point(20, 40),
                Size = new Size(340, 20)
            };
            this.Controls.Add(nameTextBox);

            // Numer stacji
            var orderLabel = new Label
            {
                Text = "Numer stacji:",
                Location = new Point(20, 70),
                AutoSize = true
            };
            this.Controls.Add(orderLabel);

            orderNumericUpDown = new NumericUpDown
            {
                Location = new Point(20, 90),
                Size = new Size(100, 20),
                Minimum = 1,
                Maximum = 100
            };
            this.Controls.Add(orderNumericUpDown);

            // Numer CN
            var cnLabel = new Label
            {
                Text = "Numer CN:",
                Location = new Point(20, 120),
                AutoSize = true
            };
            this.Controls.Add(cnLabel);

            cnTextBox = new TextBox
            {
                Location = new Point(20, 140),
                Size = new Size(340, 20)
            };
            this.Controls.Add(cnTextBox);

            // Przyciski
            saveButton = new Button
            {
                Text = "Zapisz",
                DialogResult = DialogResult.OK,
                Location = new Point(20, 170),
                Size = new Size(100, 30)
            };
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            cancelButton = new Button
            {
                Text = "Anuluj",
                DialogResult = DialogResult.Cancel,
                Location = new Point(260, 170),
                Size = new Size(100, 30)
            };
            this.Controls.Add(cancelButton);

            this.AcceptButton = saveButton;
            this.CancelButton = cancelButton;
        }

        private void LoadStationData()
        {
            try
            {
                string query = "SELECT name, station_order, cn FROM stations WHERE id = @id";
                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", stationId.Value);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    nameTextBox.Text = reader.GetString("name");
                    orderNumericUpDown.Value = reader.GetInt32("station_order");
                    cnTextBox.Text = reader.IsDBNull(reader.GetOrdinal("cn")) ? "" : reader.GetString("cn");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas ładowania danych stacji: " + ex.Message);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Wprowadź nazwę stacji");
                return;
            }

            try
            {
                if (stationId.HasValue)
                {
                    UpdateStation();
                }
                else
                {
                    AddStation();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas zapisywania stacji: " + ex.Message);
            }
        }

        private void AddStation()
        {
            string query = @"
                INSERT INTO stations (name, station_order, cn) 
                VALUES (@name, @order, @cn)";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@name", nameTextBox.Text);
            cmd.Parameters.AddWithValue("@order", orderNumericUpDown.Value);
            cmd.Parameters.AddWithValue("@cn", string.IsNullOrWhiteSpace(cnTextBox.Text) ? DBNull.Value : cnTextBox.Text);
            
            cmd.ExecuteNonQuery();
        }

        private void UpdateStation()
        {
            string query = @"
                UPDATE stations 
                SET name = @name, 
                    station_order = @order, 
                    cn = @cn 
                WHERE id = @id";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@name", nameTextBox.Text);
            cmd.Parameters.AddWithValue("@order", orderNumericUpDown.Value);
            cmd.Parameters.AddWithValue("@cn", string.IsNullOrWhiteSpace(cnTextBox.Text) ? DBNull.Value : cnTextBox.Text);
            cmd.Parameters.AddWithValue("@id", stationId.Value);
            
            cmd.ExecuteNonQuery();
        }
    }
} 