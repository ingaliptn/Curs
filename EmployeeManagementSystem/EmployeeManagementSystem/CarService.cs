using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsManagementSystem
{
    public class CarService
    {
        public string ConnectionString { get; } = "Data Source=localhost;Initial Catalog=CarSearchSystem;Integrated Security=True";

        public List<Car> GetCars(string make, string model, int year)
        {
            List<Car> cars = new List<Car>();

            using (SqlConnection connect = new SqlConnection(ConnectionString))
            {
                connect.Open();
                string query = "SELECT Cars.ID, Makes.Name AS Make, Models.Name AS Model, Cars.Year, Cars.Price, Colors.Name AS Color, " +
                               "CarTypes.Type AS CarType, EngineTypes.Type AS EngineType, Cars.ImagePath, Cars.Description " +
                               "FROM Cars " +
                               "JOIN Makes ON Cars.MakeID = Makes.ID " +
                               "JOIN Models ON Cars.ModelID = Models.ID " +
                               "JOIN Colors ON Cars.ColorID = Colors.ID " +
                               "JOIN CarTypes ON Cars.CarTypeID = CarTypes.ID " +
                               "JOIN EngineTypes ON Cars.EngineTypeID = EngineTypes.ID " +
                               "WHERE (@make IS NULL OR Makes.Name = @make) " +
                               "AND (@model IS NULL OR Models.Name = @model) " +
                               "AND (@year = 0 OR Cars.Year = @year)";

                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@make", (object)make ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@model", (object)model ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@year", year == 0 ? DBNull.Value : (object)year);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cars.Add(new Car
                    {
                        ID = (int)reader["ID"],
                        Make = reader["Make"].ToString(),
                        Model = reader["Model"].ToString(),
                        Year = (int)reader["Year"],
                        Price = (decimal)reader["Price"],
                        Color = reader["Color"].ToString(),
                        CarType = reader["CarType"].ToString(),
                        EngineType = reader["EngineType"].ToString(),
                        ImagePath = reader["ImagePath"].ToString(),
                        Description = reader["Description"].ToString()
                    });
                }
            }

            return cars;
        }
    }

}
