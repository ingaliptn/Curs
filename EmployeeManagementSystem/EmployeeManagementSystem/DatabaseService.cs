using System;
using System.Data;
using System.Data.SqlClient;

public class DatabaseService
{
    private readonly string connectionString = "Data Source=localhost;Initial Catalog=CarSearchSystem;Integrated Security=True";

    public string ConnectionString => connectionString;

    public DataTable ExecuteQuery(string query)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand(query, connection);
            var adapter = new SqlDataAdapter(command);
            var table = new DataTable();
            adapter.Fill(table);
            return table;
        }
    }

    public void ExecuteNonQuery(string query, Action<SqlCommand> parameterize)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand(query, connection);
            parameterize?.Invoke(command);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    public object ExecuteScalar(string query, Action<SqlCommand> parameterize)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand(query, connection);
            parameterize?.Invoke(command);
            connection.Open();
            return command.ExecuteScalar();
        }
    }
}
