using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class AdminPanel : UserControl
    {
        private string connectionString = "Data Source=localhost;Initial Catalog=CarSearchSystem;Integrated Security=True";

        // Змінні для контролю завантаження списків
        private bool isMakesLoaded = false;
        private bool isModelsLoaded = false;
        private bool isYearsLoaded = false;
        private bool isTypeLoaded = false;
        private bool isColorLoaded = false;
        private bool isEngineLoaded = false;

        public AdminPanel()
        {
            InitializeComponent();
            LoadAllCars(); // Завантажуємо всі автомобілі при ініціалізації
            InitializeComboBoxEvents(); // Ініціалізуємо події для ComboBox
            dgvCars.CellDoubleClick += dgvCars_CellDoubleClick; // Додаємо обробник події подвійного кліку на DataGridView
            cmbMake.SelectionChangeCommitted += cmbMake_SelectionChangeCommitted; // Обробник зміни вибраного елементу у ComboBox для виробника

            // Завантажуємо дані для cmbMake при завантаженні форми
            LoadComboBoxData(cmbMake, "Makes", "Name", "ID", null, null);

            // Завантажуємо роки один раз при ініціалізації форми
            LoadYears();
            clearFields(); // Очищаємо поля після видалення
            // Додаємо обробник події для кнопки Clear
            addEmployee_clearBtn.Click += addEmployee_clearBtn_Click;
        }

        // Метод для завантаження років
        private void LoadYears()
        {
            // Очищаємо попередні елементи ComboBox перед додаванням нових
            cmbYear.Items.Clear();

            // Додаємо роки з 2012 по 2024
            for (int year = 2012; year <= 2024; year++)
            {
                cmbYear.Items.Add(year);
            }

            // Встановлюємо значення за замовчуванням
            cmbYear.SelectedIndex = 0;
        }

        // Ініціалізуємо події для ComboBox
        private void InitializeComboBoxEvents()
        {
            cmbMake.DropDown += (s, e) => { if (!isMakesLoaded) { LoadMakes(); isMakesLoaded = true; } };
            cmbModel.DropDown += (s, e) => { if (!isModelsLoaded && cmbMake.SelectedValue != null) { LoadModels((int)cmbMake.SelectedValue); isModelsLoaded = true; } };
            cmbYear.DropDown += (s, e) => { if (!isYearsLoaded) { LoadYears(); isYearsLoaded = true; } };
            cmbCarType.DropDown += (s, e) => { if (!isTypeLoaded) { LoadCarTypes(); isTypeLoaded = true; } };
            cmbColor.DropDown += (s, e) => { if (!isColorLoaded) { LoadColors(); isColorLoaded = true; } };
            cmbEngineType.DropDown += (s, e) => { if (!isEngineLoaded) { LoadEngineTypes(); isEngineLoaded = true; } };
        }

        // Виконує SQL запит без повернення даних (INSERT, UPDATE, DELETE)
        private void ExecuteQuery(string query, Action<SqlCommand> parameterize)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(query, connection);
                parameterize?.Invoke(command); // Додаємо параметри до команди
                connection.Open();
                command.ExecuteNonQuery(); // Виконуємо команду
            }
        }

        // Виконує SQL запит і повертає результати у вигляді DataTable
        private DataTable ExecuteQuery(string query)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(query, connection);
                var adapter = new SqlDataAdapter(command);
                var table = new DataTable();
                adapter.Fill(table); // Заповнюємо таблицю результатами запиту
                return table;
            }
        }

        // Завантажуємо всі автомобілі і відображаємо їх у DataGridView
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
                EngineTypes.Type AS EngineType
            FROM 
                Cars
            JOIN Makes ON Cars.MakeID = Makes.ID
            JOIN Models ON Cars.ModelID = Models.ID
            JOIN Colors ON Cars.ColorID = Colors.ID
            JOIN CarTypes ON Cars.CarTypeID = CarTypes.ID
            JOIN EngineTypes ON Cars.EngineTypeID = EngineTypes.ID";

            dgvCars.DataSource = ExecuteQuery(query); // Присвоюємо результати запиту як джерело даних для DataGridView

            // Приховуємо стовпці ID
            dgvCars.Columns["ID"].Visible = false;
            dgvCars.Columns["MakeID"].Visible = false;
            dgvCars.Columns["ModelID"].Visible = false;
            dgvCars.Columns["ColorID"].Visible = false;
            dgvCars.Columns["CarTypeID"].Visible = false;
            dgvCars.Columns["EngineTypeID"].Visible = false;
        }

        // Завантажуємо дані у ComboBox
        private void LoadComboBoxData(ComboBox comboBox, string tableName, string displayMember, string valueMember, object selectedValue, object makeID = null)
        {
            string query = makeID != null && tableName == "Models"
                ? $"SELECT {valueMember}, {displayMember} FROM {tableName} WHERE MakeID = {makeID}"
                : $"SELECT {valueMember}, {displayMember} FROM {tableName}";

            var table = ExecuteQuery(query); // Виконуємо запит для отримання даних
            comboBox.DataSource = table; // Присвоюємо результати запиту як джерело даних для ComboBox
            comboBox.DisplayMember = displayMember;
            comboBox.ValueMember = valueMember;

            // Встановлюємо вибране значення, якщо воно не null
            if (selectedValue != null)
            {
                comboBox.SelectedValue = selectedValue;
            }
        }

        // Обробник події подвійного кліку на DataGridView
        private void dgvCars_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCars.Rows[e.RowIndex];
                txtCarID.Text = row.Cells["ID"].Value.ToString();
                txtPrice.Text = row.Cells["Price"].Value.ToString();

                LoadComboBoxData(cmbMake, "Makes", "Name", "ID", row.Cells["MakeID"].Value);
                LoadComboBoxData(cmbModel, "Models", "Name", "ID", row.Cells["ModelID"].Value, row.Cells["MakeID"].Value);

                // Очищення перед додаванням року
                cmbYear.SelectedIndex = -1;

                // Встановлюємо значення року
                if (row.Cells["Year"].Value != null)
                {
                    cmbYear.SelectedItem = row.Cells["Year"].Value;
                }

                LoadComboBoxData(cmbColor, "Colors", "Name", "ID", row.Cells["ColorID"].Value);
                LoadComboBoxData(cmbCarType, "CarTypes", "Type", "ID", row.Cells["CarTypeID"].Value);
                LoadComboBoxData(cmbEngineType, "EngineTypes", "Type", "ID", row.Cells["EngineTypeID"].Value);
            }
        }

        // Додаємо новий автомобіль
        private void addEmployee_addBtn_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO Cars (MakeID, ModelID, Year, Price, ColorID, CarTypeID, EngineTypeID) VALUES (@MakeID, @ModelID, @Year, @Price, @ColorID, @CarTypeID, @EngineTypeID)";
            ExecuteQuery(query, command =>
            {
                command.Parameters.AddWithValue("@MakeID", cmbMake.SelectedValue);
                command.Parameters.AddWithValue("@ModelID", cmbModel.SelectedValue);
                command.Parameters.AddWithValue("@Year", cmbYear.SelectedItem);
                command.Parameters.AddWithValue("@Price", txtPrice.Text);
                command.Parameters.AddWithValue("@ColorID", cmbColor.SelectedValue);
                command.Parameters.AddWithValue("@CarTypeID", cmbCarType.SelectedValue);
                command.Parameters.AddWithValue("@EngineTypeID", cmbEngineType.SelectedValue);
            });
            RefreshData(); // Оновлюємо дані після видалення
            clearFields(); // Очищаємо поля після видалення
        }

        // Оновлюємо існуючий автомобіль
        private void addEmployee_updateBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCarID.Text) && int.TryParse(txtCarID.Text, out int carId))
            {
                string query = "UPDATE Cars SET MakeID = @MakeID, ModelID = @ModelID, Year = @Year, Price = @Price, ColorID = @ColorID, CarTypeID = @CarTypeID, EngineTypeID = @EngineTypeID WHERE ID = @ID";
                ExecuteQuery(query, command =>
                {
                    command.Parameters.AddWithValue("@ID", carId);
                    command.Parameters.AddWithValue("@MakeID", cmbMake.SelectedValue);
                    command.Parameters.AddWithValue("@ModelID", cmbModel.SelectedValue);
                    command.Parameters.AddWithValue("@Year", cmbYear.SelectedItem);
                    command.Parameters.AddWithValue("@Price", txtPrice.Text);
                    command.Parameters.AddWithValue("@ColorID", cmbColor.SelectedValue);
                    command.Parameters.AddWithValue("@CarTypeID", cmbCarType.SelectedValue);
                    command.Parameters.AddWithValue("@EngineTypeID", cmbEngineType.SelectedValue);
                });
                RefreshData(); // Оновлюємо дані після видалення
                clearFields(); // Очищаємо поля після видалення
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть автомобіль для редагування.");
            }
        }

        // Видаляємо автомобіль
        private void addEmployee_deleteBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCarID.Text) && int.TryParse(txtCarID.Text, out int carId))
            {
                string query = "DELETE FROM Cars WHERE ID = @ID";
                ExecuteQuery(query, command =>
                {
                    command.Parameters.AddWithValue("@ID", carId);
                });
                RefreshData(); // Оновлюємо дані після видалення
                clearFields(); // Очищаємо поля після видалення
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть автомобіль для видалення.");
            }
        }

        // Очищаємо всі поля форми
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
        }

        // Оновлюємо дані у DataGridView
        private void RefreshData()
        {
            LoadAllCars();
        }

        // Обробник зміни вибраного виробника
        private void cmbMake_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmbMake.SelectedValue != null)
            {
                int selectedMakeID = (int)cmbMake.SelectedValue;
                LoadComboBoxData(cmbModel, "Models", "Name", "ID", null, selectedMakeID);
            }
        }

        // Методи для завантаження даних у ComboBox
        private void LoadMakes() => LoadComboBoxData(cmbMake, "Makes", "Name", "ID", null);
        private void LoadModels(int makeId) => LoadComboBoxData(cmbModel, "Models", "Name", "ID", null, makeId);
        private void LoadColors() => LoadComboBoxData(cmbColor, "Colors", "Name", "ID", null);
        private void LoadCarTypes() => LoadComboBoxData(cmbCarType, "CarTypes", "Type", "ID", null);
        private void LoadEngineTypes() => LoadComboBoxData(cmbEngineType, "EngineTypes", "Type", "ID", null);

        // Обробник події для кнопки Clear
        private void addEmployee_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }
    }
}
