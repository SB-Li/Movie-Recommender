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
    public partial class SearchUsers : Form
    {
        public SearchUsers()
        {
            InitializeComponent();
        }

        private void UserID_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SearchPrint();
        }

        void SearchPrint()
        {
            Connection con = new CFRMovie.Connection();
            string sql = "SELECT * FROM dbo.[user] WHERE dbo.[user].userid=" + textBox1.Text;
            DataSet ds = new DataSet();
            SqlDataAdapter command = new SqlDataAdapter(sql, con.ActiveCon());
            command.Fill(ds, "ds");
            this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
