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
    public partial class DeleteUser : Form
    {
        public DeleteUser()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DeleteRecords();
            MessageBox.Show("Deleted!");
        }
        void DeleteRecords()
        {
            Connection con = new CFRMovie.Connection();
            string sql = string.Format(@"Delete FROM dbo.[user] WHERE dbo.[user].userid={0}", textBox1.Text);
            SqlCommand cmd = new SqlCommand(sql, con.ActiveCon());
            cmd.ExecuteNonQuery();
        }
    }
}
