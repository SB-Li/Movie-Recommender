using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CFRMovie
{
    public partial class Users : Form
    {
        public Users()
        {
            InitializeComponent();
        }

        private void Users_Load(object sender, EventArgs e)
        {
            PrintUser();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        void PrintUser()
        {
            Connection con = new CFRMovie.Connection();
            string sql = "SELECT * FROM dbo.[user]";
            DataSet ds = new DataSet();
            SqlDataAdapter command = new SqlDataAdapter(sql, con.ActiveCon());
            command.Fill(ds, "ds");
            this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
            

        }
    }
}
