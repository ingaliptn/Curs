using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementSystem
{
    class EmployeeData
    {

        public int ID { set; get; } // 0
        public string EmployeeID { set; get; } // 1
        public string Name { set; get; } // 2
        public string Gender { set; get; } // 3
        public string Contact { set; get; } // 4
        public string Position { set; get; } // 5
        public string Image { set; get; } // 6
        public int Salary { set; get; } // 7
        public string Status { set; get; } // 8


        //SqlConnection connect = new SqlConnection(@"Data Source=localhost;Initial Catalog=CarSearchSystem;Integrated Security=True;Connect Timeout=30");
    

        public List<EmployeeData> employeeListData()
        {
            return null;
        }

        public List<EmployeeData> salaryEmployeeListData()
        {
            return null;
        }
    }
}
