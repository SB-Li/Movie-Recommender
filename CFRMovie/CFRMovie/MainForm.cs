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
    
    public partial class MainForm : Form
    {
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void userInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void scanAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Users u = new Users();
            u.Show();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateAddUser u = new CFRMovie.UpdateAddUser();
            u.Show();
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchUsers u = new CFRMovie.SearchUsers();
            u.Show();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteUser u = new CFRMovie.DeleteUser();
            u.Show();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Recommender u = new Recommender();
            u.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
