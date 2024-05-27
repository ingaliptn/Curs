using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace CarsManagementSystem
{
    public partial class MainForm : Form
    {
        private string userRole;
        private string username;

        public MainForm(string role, string username)
        {
            InitializeComponent();
            userRole = role;
            this.username = username;
            ConfigureFormBasedOnRole();
        }

        private void ConfigureFormBasedOnRole()
        {
            if (userRole == "user")
            {
                // Вимкнути або приховати CRUD операції для звичайних користувачів
                salary_btn.Visible = false;
            }
            // Якщо роль - admin, всі кнопки залишаються доступними

            // Відображення імені користувача на формі, якщо необхідно
            greet_user.Text = $"Вітаю, {username}";

        }

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void logout_btn_Click(object sender, EventArgs e)
        {
            DialogResult check = MessageBox.Show("Ви впевнені що хочете вийти з акаунту?"
                , "Повідомлення", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(check == DialogResult.Yes)
            {
                Login loginForm = new Login();
                loginForm.Show();
                this.Hide();
            }
        }

        private void dashboard_btn_Click(object sender, EventArgs e)
        {
            dashboard1.Visible = true;
            addEmployee1.Visible = false;
            salary1.Visible = false;

            Dashboard dashForm = dashboard1 as Dashboard;

            if(dashForm != null)
            {
                dashForm.RefreshData();
            }

        }

        private void addEmployee_btn_Click(object sender, EventArgs e)
        {
            dashboard1.Visible = false;
            addEmployee1.Visible = true;
            salary1.Visible = false;

            AdminPanel addEmForm = addEmployee1 as AdminPanel;

            //if(addEmForm != null)
            //{
            //    addEmForm.RefreshData();
            //}

        }

        private void salary_btn_Click(object sender, EventArgs e)
        {
            dashboard1.Visible = false;
            addEmployee1.Visible = false;
            salary1.Visible = true;

            FindCar salaryForm = salary1 as FindCar;


        }
    }
}
