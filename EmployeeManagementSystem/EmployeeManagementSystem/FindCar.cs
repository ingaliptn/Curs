using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;

namespace CarsManagementSystem
{
    public partial class FindCar : UserControl
    {

        private CarService carService = new CarService();

        public FindCar()
        {
            InitializeComponent();
            InitializeComboBoxes();
            LoadMakes();
            cmbMake.SelectedIndexChanged += cmbMake_SelectedIndexChanged; // Підключення події
        }


        private void InitializeComboBoxes()
        {
            cmbMake.SelectedIndex = -1;
            cmbModel.SelectedIndex = -1;
            cmbYear.SelectedIndex = -1;
            cmbModel.Enabled = false;  // Робимо cmbModel неактивним
            cmbYear.Items.Clear();

            // Додаємо роки у cmbYear з 2012 по 2024
            for (int year = 2012; year <= 2024; year++)
            {
                cmbYear.Items.Add(year);
            }
        }


        private void LoadMakes()
        {
            cmbMake.Items.Clear();
            using (SqlConnection connect = new SqlConnection(carService.ConnectionString))
            {
                connect.Open();
                string query = "SELECT DISTINCT Name FROM Makes";
                SqlCommand cmd = new SqlCommand(query, connect);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cmbMake.Items.Add(reader["Name"].ToString());
                }
            }
        }

        private void cmbMake_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMake = cmbMake.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedMake))
            {
                LoadModels(selectedMake);
                cmbModel.Enabled = true;  // Робимо cmbModel активним після вибору марки
            }
            else
            {
                cmbModel.SelectedIndex = -1;
                cmbModel.Enabled = false;  // Робимо cmbModel неактивним, якщо марка не вибрана
            }
        }

        private void LoadModels(string make)
        {
            cmbModel.Items.Clear();
            using (SqlConnection connect = new SqlConnection(carService.ConnectionString))
            {
                connect.Open();
                string query = "SELECT DISTINCT Models.Name FROM Models JOIN Makes ON Models.MakeID = Makes.ID WHERE Makes.Name = @make";
                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@make", make);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cmbModel.Items.Add(reader["Name"].ToString());
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string make = cmbMake.SelectedItem?.ToString();
            string model = cmbModel.SelectedItem?.ToString();
            int year = (cmbYear.SelectedItem != null) ? int.Parse(cmbYear.SelectedItem.ToString()) : 0;

            List<Car> cars = carService.GetCars(make, model, year);

            if (cars.Count > 0)
            {
                DisplayCarInfo(cars[0]);
            }
            else
            {
                MessageBox.Show("No cars found.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DisplayCarInfo(Car car)
        {
            lMakes.Text  = car.Make;
            lModel.Text  = car.Model;
            lYear.Text   = car.Year.ToString();
            lPrice.Text  = car.Price.ToString();
            lColor.Text  = car.Color;
            lType.Text   = car.CarType;
            lEngine.Text = car.EngineType;
            lDesc.Text   = car.Description;
            //lMakes.Text = $"Картинка: {car.ImagePath}";

            if (!string.IsNullOrEmpty(car.ImagePath))
            {
                // Оновлення шляху до зображення
                string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagePath = Path.Combine(projectDirectory, "images", Path.GetFileName(car.ImagePath));

                if (File.Exists(imagePath))
                {
                    pictureBoxCar.Image = Image.FromFile(imagePath);
                }
                else
                {
                    pictureBoxCar.Image = null;
                    MessageBox.Show($"Зображення не знайдено: {imagePath}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                pictureBoxCar.Image = null;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            InitializeComboBoxes();
            lMakes.Text = string.Empty;
            lModel.Text = string.Empty;
            lYear.Text = string.Empty;
            lPrice.Text = string.Empty;
            lColor.Text = string.Empty;
            lType.Text = string.Empty;
            lEngine.Text = string.Empty;
            lDesc.Text = string.Empty;
        }
    }
}
