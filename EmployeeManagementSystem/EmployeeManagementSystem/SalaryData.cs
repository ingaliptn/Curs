using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementSystem
{
    class SalaryData
    {

        public string EmployeeID { set; get; } // 0
        public string Name { set; get; } // 1
        public string Gender { set; get; } // 2
        public string Contact { set; get; } // 3
        public string Position { set; get; } // 4
        public int Salary { set; get; } // 5

        //SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\WINDOWS 10\Documents\employee.mdf;Integrated Security=True;Connect Timeout=30");

        public List<SalaryData> salaryEmployeeListData()
        {
            return null;

        }

    }
}
