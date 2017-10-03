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
    public partial class UpdateAddUser : Form
    {
        public UpdateAddUser()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddRecords();
            MessageBox.Show("Added!");
        }
        void AddRecords()
        {
            Connection con = new CFRMovie.Connection();
            string sql = string.Format(@"INSERT INTO[dbo].[user]([userid],[age] ,[gender] ,[occupation],[zipcode]) 
                VALUES ({0},{1},'{2}','{3}',{4})", textBox1.Text, textBox2.Text, textBox3.Text, 
                textBox4.Text, textBox5.Text);
            SqlCommand cmd = new SqlCommand(sql, con.ActiveCon());
            cmd.ExecuteNonQuery();
        }
        void UpdateRecords()
        {
            Connection con = new CFRMovie.Connection();
            string sql = string.Format(@"UPDATE [dbo].[user] SET [userid] = {0},[age] = {1} ,[gender] = '{2}',
                [occupation] = '{3}' ,[zipcode] ={4} WHERE[userid] = {0}", textBox1.Text, textBox2.Text, 
                textBox3.Text, textBox4.Text, textBox5.Text);
            SqlCommand cmd = new SqlCommand(sql, con.ActiveCon());         
            cmd.ExecuteNonQuery();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            UpdateRecords();
            MessageBox.Show("Updated!");
        }
    }
}
