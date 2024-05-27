﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace CarsManagementSystem
{
    public partial class Dashboard : UserControl
    {
        private string connectionString = "Data Source=localhost;Initial Catalog=CarSearchSystem;Integrated Security=True";
        public Dashboard()
        {
            InitializeComponent();

            displayTE();
            displayAE();
            displayIE();
        }

        public void RefreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)RefreshData);
                return;
            }

            displayTE();
            displayAE();
            displayIE();
        }

        public void displayTE()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "Select Count(ID) from Cars ";

                var command = new SqlCommand(query, connection);
                var adapter = new SqlDataAdapter(command);

                connection.Open();
                var reader = command.ExecuteReader();
                
                if (reader.Read())
                {
                    int count = Convert.ToInt32(reader[0]);
                    dashboard_TE.Text = count.ToString();
                }
                //if(connect.State != ConnectionState.Open)
                //{
                //    try
                //    {
                //        connect.Open();

                //        string selectData = "SELECT COUNT(id) FROM employees WHERE delete_date IS NULL";

                //        using(SqlCommand cmd = new SqlCommand(selectData, connect))
                //        {
                //            SqlDataReader reader = cmd.ExecuteReader();

                //            if (reader.Read())
                //            {
                //                int count = Convert.ToInt32(reader[0]);
                //                dashboard_TE.Text = count.ToString();
                //            }
                //            reader.Close();
                //        }

                //    }catch(Exception ex)
                //    {
                //        MessageBox.Show("Error: " + ex, "Error Message"
                //            , MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    }
                //    finally
                //    {
                //        connect.Close();
                //    }
                //}
            }
        }

        public void displayAE()
        {
            //if (connect.State != ConnectionState.Open)
            //{
            //    try
            //    {
            //        connect.Open();

            //        string selectData = "SELECT COUNT(id) FROM employees WHERE status = @status " +
            //            "AND delete_date IS NULL";

            //        using (SqlCommand cmd = new SqlCommand(selectData, connect))
            //        {
            //            cmd.Parameters.AddWithValue("@status", "Active");
            //            SqlDataReader reader = cmd.ExecuteReader();

            //            if (reader.Read())
            //            {
            //                int count = Convert.ToInt32(reader[0]);
            //                dashboard_AE.Text = count.ToString();
            //            }
            //            reader.Close();
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("Error: " + ex, "Error Message"
            //            , MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    finally
            //    {
            //        connect.Close();
            //    }
            //}
        }

        public void displayIE()
        {
            //if (connect.State != ConnectionState.Open)
            //{
            //    try
            //    {
            //        connect.Open();

            //        string selectData = "SELECT COUNT(id) FROM employees WHERE status = @status " +
            //            "AND delete_date IS NULL";

            //        using (SqlCommand cmd = new SqlCommand(selectData, connect))
            //        {
            //            cmd.Parameters.AddWithValue("@status", "Ianctive");
            //            SqlDataReader reader = cmd.ExecuteReader();

            //            if (reader.Read())
            //            {
            //                int count = Convert.ToInt32(reader[0]);
            //                dashboard_IE.Text = count.ToString();
            //            }
            //            reader.Close();
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("Error: " + ex, "Error Message"
            //            , MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    finally
            //    {
            //        connect.Close();
            //    }
            //}
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
