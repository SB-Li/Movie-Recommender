using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace CFRMovie
{
    public class Connection
    {
        SqlConnection con = new SqlConnection("Data Source=PSUCP3-PC\\SQLEXPRESS;Initial Catalog=CFDB;Integrated Security=True"); 
       // SqlConnection con = new SqlConnection("Data Source=PSUCP3-PC\\PSUCP3;Initial Catalog=CFDB;Integrated Security=True");
        public SqlConnection ActiveCon()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            return con;
        }
    }
}
