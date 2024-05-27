using CarsManagementSystem;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

public class ComboBoxService
{
    private DatabaseService dbService = new DatabaseService();

    public void LoadComboBoxData(ComboBox comboBox, string tableName, string displayMember, string valueMember, object selectedValue, object makeID = null)
    {
        string query = makeID != null && tableName == "Models"
            ? $"SELECT {valueMember}, {displayMember} FROM {tableName} WHERE MakeID = {makeID}"
            : $"SELECT {valueMember}, {displayMember} FROM {tableName}";

        var table = dbService.ExecuteQuery(query);
        comboBox.DataSource = table;
        comboBox.DisplayMember = displayMember;
        comboBox.ValueMember = valueMember;

        if (selectedValue != null)
        {
            comboBox.SelectedValue = selectedValue;
        }
    }

    public void LoadYears(ComboBox comboBox)
    {
        comboBox.Items.Clear();
        for (int year = 2012; year <= 2024; year++)
        {
            comboBox.Items.Add(year);
        }
        comboBox.SelectedIndex = 0;
    }
}
