using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace kolejarka
{
    public class ManageStationsForm : Form
    {
        private readonly MySqlConnection connection;
        private DataGridView stationsGridView;
        private Button addStationButton;
        private Button editStationButton;
        private Button deleteStationButton;
        private Button refreshButton;

        public ManageStationsForm(MySqlConnection conn)
        {
            connection = conn;
            InitializeComponents();
            LoadStations();
        }

        private void InitializeComponents()
        {
            this.Text = "Zarządzanie stacjami";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            // DataGridView
            stationsGridView = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 500,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.Controls.Add(stationsGridView);

            // Panel przycisków
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            addStationButton = new Button
            {
                Text = "Dodaj stację",
                Location = new Point(20, 10),
                Size = new Size(100, 30)
            };
            addStationButton.Click += AddStationButton_Click;

            editStationButton = new Button
            {
                Text = "Edytuj stację",
                Location = new Point(140, 10),
                Size = new Size(100, 30)
            };
            editStationButton.Click += EditStationButton_Click;

            deleteStationButton = new Button
            {
                Text = "Usuń stację",
                Location = new Point(260, 10),
                Size = new Size(100, 30)
            };
            deleteStationButton.Click += DeleteStationButton_Click;

            refreshButton = new Button
            {
                Text = "Odśwież",
                Location = new Point(380, 10),
                Size = new Size(100, 30)
            };
            refreshButton.Click += RefreshButton_Click;

            buttonPanel.Controls.AddRange(new Control[] { 
                addStationButton, 
                editStationButton, 
                deleteStationButton, 
                refreshButton 
            });

            this.Controls.Add(buttonPanel);
        }

        private void LoadStations()
        {
            try
            {
                string query = "SELECT id, name, station_order, cn FROM stations ORDER BY station_order";
                using var cmd = new MySqlCommand(query, connection);
                using var adapter = new MySqlDataAdapter(cmd);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                stationsGridView.DataSource = dataTable;
                stationsGridView.Columns["id"].Visible = false;
                stationsGridView.Columns["name"].HeaderText = "Nazwa stacji";
                stationsGridView.Columns["station_order"].HeaderText = "Numer stacji";
                stationsGridView.Columns["cn"].HeaderText = "Numer CN";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas ładowania stacji: " + ex.Message);
            }
        }

        private void AddStationButton_Click(object sender, EventArgs e)
        {
            using var addForm = new DodajStacjeForm(connection);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadStations();
            }
        }

        private void EditStationButton_Click(object sender, EventArgs e)
        {
            if (stationsGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Wybierz stację do edycji");
                return;
            }

            int stationId = Convert.ToInt32(stationsGridView.SelectedRows[0].Cells["id"].Value);
            using var editForm = new DodajStacjeForm(connection, stationId);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadStations();
            }
        }

        private void DeleteStationButton_Click(object sender, EventArgs e)
        {
            if (stationsGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Wybierz stację do usunięcia");
                return;
            }

            int stationId = Convert.ToInt32(stationsGridView.SelectedRows[0].Cells["id"].Value);
            string stationName = stationsGridView.SelectedRows[0].Cells["name"].Value.ToString();

            var result = MessageBox.Show(
                $"Czy na pewno chcesz usunąć stację {stationName}?",
                "Potwierdzenie usunięcia",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM stations WHERE id = @id";
                    using var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", stationId);
                    cmd.ExecuteNonQuery();
                    LoadStations();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas usuwania stacji: " + ex.Message);
                }
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadStations();
        }
    }
} 