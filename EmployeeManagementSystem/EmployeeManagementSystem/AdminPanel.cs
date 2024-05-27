using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CarsManagementSystem
{
    public partial class AdminPanel : UserControl
    {
        private DatabaseService dbService = new DatabaseService();
        private ComboBoxService cbService = new ComboBoxService();

        private bool isMakesLoaded = false;
        private bool isModelsLoaded = false;
        private bool isYearsLoaded = false;
        private bool isTypeLoaded = false;
        private bool isColorLoaded = false;
        private bool isEngineLoaded = false;
        private string imagePath = string.Empty;
        public AdminPanel()
        {
            InitializeComponent();
            LoadAllCars();
            InitializeComboBoxEvents();
            dgvCars.CellDoubleClick += dgvCars_CellDoubleClick;
            cmbMake.SelectionChangeCommitted += cmbMake_SelectionChangeCommitted;
            cbService.LoadComboBoxData(cmbMake, "Makes", "Name", "ID", null);
            cbService.LoadYears(cmbYear);
            clearFields();
            addEmployee_clearBtn.Click += addEmployee_clearBtn_Click;
            importBtn.Click += ImportBtn_Click; // Додаємо обробник події для кнопки importBtn
        }

        private void InitializeComboBoxEvents()
        {
            cmbMake.DropDown += (s, e) => { if (!isMakesLoaded) { LoadMakes(); isMakesLoaded = true; } };
            cmbModel.DropDown += (s, e) => { if (!isModelsLoaded && cmbMake.SelectedValue != null) { LoadModels((int)cmbMake.SelectedValue); isModelsLoaded = true; } };
            cmbYear.DropDown += (s, e) => { if (!isYearsLoaded) { cbService.LoadYears(cmbYear); isYearsLoaded = true; } };
            cmbCarType.DropDown += (s, e) => { if (!isTypeLoaded) { LoadCarTypes(); isTypeLoaded = true; } };
            cmbColor.DropDown += (s, e) => { if (!isColorLoaded) { LoadColors(); isColorLoaded = true; } };
            cmbEngineType.DropDown += (s, e) => { if (!isEngineLoaded) { LoadEngineTypes(); isEngineLoaded = true; } };
        }

        private void LoadAllCars()
        {
            string query = @"
            SELECT 
                Cars.ID,
                Cars.MakeID,
                Makes.Name AS Make,
                Cars.ModelID,
                Models.Name AS Model,
                Cars.Year,
                Cars.Price,
                Cars.ColorID,
                Colors.Name AS Color,
                Cars.CarTypeID,
                CarTypes.Type AS CarType,
                Cars.EngineTypeID,
                EngineTypes.Type AS EngineType,
                Cars.ImagePath
            FROM 
                Cars
            JOIN Makes ON Cars.MakeID = Makes.ID
            JOIN Models ON Cars.ModelID = Models.ID
            JOIN Colors ON Cars.ColorID = Colors.ID
            JOIN CarTypes ON Cars.CarTypeID = CarTypes.ID
            JOIN EngineTypes ON Cars.EngineTypeID = EngineTypes.ID";

            DataTable table = dbService.ExecuteQuery(query);

            dgvCars.DataSource = table;
            dgvCars.Columns["ID"].Visible = false;
            dgvCars.Columns["MakeID"].Visible = false;
            dgvCars.Columns["ModelID"].Visible = false;
            dgvCars.Columns["ColorID"].Visible = false;
            dgvCars.Columns["CarTypeID"].Visible = false;
            dgvCars.Columns["EngineTypeID"].Visible = false;
        }

        private void dgvCars_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCars.Rows[e.RowIndex];
                txtCarID.Text = row.Cells["ID"].Value.ToString();
                txtPrice.Text = row.Cells["Price"].Value.ToString();

                cbService.LoadComboBoxData(cmbMake, "Makes", "Name", "ID", row.Cells["MakeID"].Value);
                cbService.LoadComboBoxData(cmbModel, "Models", "Name", "ID", row.Cells["ModelID"].Value, row.Cells["MakeID"].Value);

                cmbYear.SelectedIndex = -1;
                if (row.Cells["Year"].Value != null)
                {
                    cmbYear.SelectedItem = row.Cells["Year"].Value;
                }

                cbService.LoadComboBoxData(cmbColor, "Colors", "Name", "ID", row.Cells["ColorID"].Value);
                cbService.LoadComboBoxData(cmbCarType, "CarTypes", "Type", "ID", row.Cells["CarTypeID"].Value);
                cbService.LoadComboBoxData(cmbEngineType, "EngineTypes", "Type", "ID", row.Cells["EngineTypeID"].Value);

                if (row.Cells["ImagePath"] != null && row.Cells["ImagePath"].Value != DBNull.Value)
                {
                    // Визначаємо кореневу директорію проекту
                    string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                    // Формуємо правильний шлях до зображення
                    string imagePath = Path.Combine(projectDirectory, row.Cells["ImagePath"].Value.ToString());
                    if (File.Exists(imagePath))
                    {
                        pictureBoxCar.Image = Image.FromFile(imagePath);
                    }
                    else
                    {
                        pictureBoxCar.Image = null;
                    }
                }
                else
                {
                    pictureBoxCar.Image = null;
                }
            }
        }

        private void addEmployee_addBtn_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO Cars (MakeID, ModelID, Year, Price, ColorID, CarTypeID, EngineTypeID, ImagePath) VALUES (@MakeID, @ModelID, @Year, @Price, @ColorID, @CarTypeID, @EngineTypeID, @ImagePath)";
            dbService.ExecuteNonQuery(query, command =>
            {
                command.Parameters.AddWithValue("@MakeID", cmbMake.SelectedValue);
                command.Parameters.AddWithValue("@ModelID", cmbModel.SelectedValue);
                command.Parameters.AddWithValue("@Year", cmbYear.SelectedItem);
                command.Parameters.AddWithValue("@Price", txtPrice.Text);
                command.Parameters.AddWithValue("@ColorID", cmbColor.SelectedValue);
                command.Parameters.AddWithValue("@CarTypeID", cmbCarType.SelectedValue);
                command.Parameters.AddWithValue("@EngineTypeID", cmbEngineType.SelectedValue);
                command.Parameters.AddWithValue("@ImagePath", imagePath); // Зберігаємо шлях до зображення
            });
            RefreshData();
            clearFields();
        }

        private void addEmployee_updateBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCarID.Text) && int.TryParse(txtCarID.Text, out int carId))
            {
                string query = "UPDATE Cars SET MakeID = @MakeID, ModelID = @ModelID, Year = @Year, Price = @Price, ColorID = @ColorID, CarTypeID = @CarTypeID, EngineTypeID = @EngineTypeID, ImagePath = @ImagePath WHERE ID = @ID";

                // Отримати поточний шлях до зображення з бази даних, якщо змінна imagePath порожня
                if (string.IsNullOrEmpty(imagePath))
                {
                    string selectQuery = "SELECT ImagePath FROM Cars WHERE ID = @ID";
                    var currentImagePath = dbService.ExecuteScalar(selectQuery, command =>
                    {
                        command.Parameters.AddWithValue("@ID", carId);
                    });
                    imagePath = currentImagePath?.ToString();
                }

                dbService.ExecuteNonQuery(query, command =>
                {
                    command.Parameters.AddWithValue("@ID", carId);
                    command.Parameters.AddWithValue("@MakeID", cmbMake.SelectedValue);
                    command.Parameters.AddWithValue("@ModelID", cmbModel.SelectedValue);
                    command.Parameters.AddWithValue("@Year", cmbYear.SelectedItem);
                    command.Parameters.AddWithValue("@Price", txtPrice.Text);
                    command.Parameters.AddWithValue("@ColorID", cmbColor.SelectedValue);
                    command.Parameters.AddWithValue("@CarTypeID", cmbCarType.SelectedValue);
                    command.Parameters.AddWithValue("@EngineTypeID", cmbEngineType.SelectedValue);
                    command.Parameters.AddWithValue("@ImagePath", imagePath); // Зберігаємо шлях до зображення
                });

                RefreshData();
                clearFields();
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть автомобіль для редагування.");
            }
        }

        private void addEmployee_deleteBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCarID.Text) && int.TryParse(txtCarID.Text, out int carId))
            {
                string query = "DELETE FROM Cars WHERE ID = @ID";
                dbService.ExecuteNonQuery(query, command =>
                {
                    command.Parameters.AddWithValue("@ID", carId);
                });
                RefreshData();
                clearFields();
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть автомобіль для видалення.");
            }
        }

        private void clearFields()
        {
            cmbMake.SelectedIndex = -1;
            cmbModel.SelectedIndex = -1;
            cmbYear.SelectedIndex = -1;
            cmbCarType.SelectedIndex = -1;
            cmbColor.SelectedIndex = -1;
            cmbEngineType.SelectedIndex = -1;
            txtPrice.Text = "";
            txtCarID.Text = "";
            imagePath = string.Empty;
            pictureBoxCar.Image = null;
        }

        private void RefreshData()
        {
            LoadAllCars();
        }

        private void cmbMake_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmbMake.SelectedValue != null)
            {
                int selectedMakeID = (int)cmbMake.SelectedValue;
                cbService.LoadComboBoxData(cmbModel, "Models", "Name", "ID", null, selectedMakeID);
            }
        }

        private void ImportBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                    string targetDirectory = Path.Combine(projectDirectory, "images");
                    string targetFilePath = Path.Combine(targetDirectory, Path.GetFileName(selectedFilePath));

                    try
                    {
                        // Використовуємо FileStream для копіювання файлу
                        using (FileStream sourceStream = new FileStream(selectedFilePath, FileMode.Open, FileAccess.Read))
                        using (FileStream destinationStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write))
                        {
                            sourceStream.CopyTo(destinationStream);
                        }

                        // Встановлюємо шлях до зображення
                        imagePath = "images\\" + Path.GetFileName(selectedFilePath);

                        // Відображаємо зображення у PictureBox
                        pictureBoxCar.Image = Image.FromFile(targetFilePath);
                    }
                    catch (IOException ioEx)
                    {
                        MessageBox.Show($"Error copying file: {ioEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void LoadMakes() => cbService.LoadComboBoxData(cmbMake, "Makes", "Name", "ID", null);
        private void LoadModels(int makeId) => cbService.LoadComboBoxData(cmbModel, "Models", "Name", "ID", null, makeId);
        private void LoadColors() => cbService.LoadComboBoxData(cmbColor, "Colors", "Name", "ID", null);
        private void LoadCarTypes() => cbService.LoadComboBoxData(cmbCarType, "CarTypes", "Type", "ID", null);
        private void LoadEngineTypes() => cbService.LoadComboBoxData(cmbEngineType, "EngineTypes", "Type", "ID", null);

        private void addEmployee_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }
    }
}
