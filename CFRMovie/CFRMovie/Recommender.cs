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
using System.IO;


namespace CFRMovie
{
    public partial class Recommender : Form
    {
        #region property
        public static string savePath
        {
            get
            {
                return Application.StartupPath + "\\";
            }
        }
#endregion

        #region contanst   
        public const int UserNum = 943;//number of total users 943
        public const int MovieNum = 1682;//number of movies 1682
        public const int TrainSetNum = 780;//number of previous users 780 as training set
        public const int PredictSetNum = 163;
        //number of present users(id:781--943) make recommendations for them
        //define array to store data
        static public int[][] RateTable = new int[UserNum + 1][];
        static public double[][] PearsonTable = new double[PredictSetNum + 1][];
        static public double[] AverageTable = new double[UserNum + 1];
        static public double[][] PredictTable = new double[PredictSetNum + 1][];
        static public double MAE;
        #endregion

        #region initial data

        private void InitialofRateTable()
        {
            //Initialize the matrix of users-movies
            for (int i = 1; i <= UserNum; i++)
            {

                RateTable[i] = new int[MovieNum + 1];
                for (int j = 1; j <= MovieNum; j++)
                    RateTable[i][j] = 0;
            }

            string strSQL = "select * from rating";
            // SqlDataReader drNew = DbHelperSQL.ExecuteReader(strSQL);
            SqlDataReader drNew;
            //SqlConnection connection = new SqlConnection(connectionString);
            SqlConnection con = new SqlConnection("Data Source=PSUCP3-PC\\SQLEXPRESS;Initial Catalog=CFDB;Integrated Security=True");         
            SqlCommand cmd = new SqlCommand(strSQL, con);
            try
            {
                con.Open();
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                drNew = myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }
            if (drNew.HasRows)
            {
                while (drNew.Read())
                {
                    int UserId = int.Parse(drNew[0].ToString());
                    int MovieId = int.Parse(drNew[1].ToString());
                    int RateScore = int.Parse(drNew[2].ToString());
                    //put the data from database into our array
                    RateTable[UserId][MovieId] = RateScore;
                }
            }
          
        }

        private void InitialofPearsonTable()
        {
            //initialize the table of Pearson Correlation 
            for (int i = 1; i <= PredictSetNum; i++)//163
            {
                PearsonTable[i] = new double[TrainSetNum + 1];//780
                for (int j = 1; j <= TrainSetNum; j++)
                    //call funtion PearsonCompute to calculate the Pearson index
                    PearsonTable[i][j] = PearsonCompute(RateTable[i + TrainSetNum], RateTable[j]);
            }          
        }
        /// <summary>

        /// <returns>Pearson</returns>
        private double PearsonCompute(int[] UserI, int[] UserJ)
        {
            //Function to compute Pearson's correlation 
            //<param name="UserI">the ratings array of user I</param>
            //<param name="UserJ">the ratings array of user J</param>
            //<returns>Pearson Correlation</returns>
            double pearson = 0;
            int num = 0;
            double averI = 0, averJ = 0;
            double sum1 = 0, sum2 = 0, sum3 = 0;
            for (int i = 1; i <= MovieNum; i++)
            {
                if (UserI[i] != 0 && UserJ[i] != 0)
                {
                    num++;
                    averI += UserI[i];
                    averJ += UserJ[i];
                }
            }
            if (num != 0)
            {
                averI = averI / num;
                averJ = averJ / num;
            }
            else return 0;
            for (int i = 1; i <= MovieNum; i++)
            {
                if (UserI[i] != 0 && UserJ[i] != 0)
                {
                    sum1 += (UserI[i] - averI) * (UserJ[i] - averJ);
                    sum2 += (UserI[i] - averI) * (UserI[i] - averI);
                    sum3 += (UserJ[i] - averJ) * (UserJ[i] - averJ);
                }
            }
            if (sum2 != 0 && sum3 != 0)
                pearson =sum1 / (Math.Sqrt(sum2 * sum3));
            return pearson;
        }

        private double AverageRate(int[] UserRate)
        {
            double Aver = 0;
            int RateNum = 0;
            double Sum = 0;
            for (int i = 1; i <= MovieNum; i++)
            {
                if (UserRate[i] != 0)
                {
                    Sum += UserRate[i];
                    RateNum++;
                }
            }
            Aver = Sum / RateNum;
            return Aver;
        }
        private void InitialofAverageTable()
        {            
            for (int i = 1; i <= UserNum; i++)
                AverageTable[i] = AverageRate(RateTable[i]);          
        }



        private void OutPutPearsonTable()
        {
            
            FileStream fs = new FileStream(savePath + "PearsonCoeffition.txt", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);

            for (int i = 1; i <= PredictSetNum; i++)
                for (int j = 1; j <= TrainSetNum; j++)
                {
                    sw.WriteLine("{0}\t{1}\t{2} ", i + TrainSetNum, j, PearsonTable[i][j]);
                }
            sw.Close();
        }
        private void OutputAverTable()
        {
           
            FileStream fs = new FileStream(savePath + "UserAverage.txt", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);

            for (int i = 1; i <= UserNum; i++)
            {
                sw.WriteLine("{0}\t{1} ", i + 1, AverageTable[i]);
            }
            sw.Close();
        }

        private void OutputRateTable()
        {
           
            FileStream fs = new FileStream(savePath + "RateTable.txt", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);


            for (int i = 1; i <= UserNum; i++)
            {
                for (int j = 1; j <= MovieNum; j++)
                    sw.WriteLine("{0}\t{1}\t{2}", i, j, RateTable[i][j]);

            }
            sw.Close();
           
        }

        private double PredictCompute(int UserId, int MovieI)
        {
            //compute the predicted ratings for movie I by userId
            double predict = 0;
            double sum1 = 0;
            double sum2 = 0;
            int num = 0;
            for (int i = 1; i <= TrainSetNum; i++)
            {
                if (RateTable[i][MovieI] != 0)
                {
                    sum1 += (RateTable[i][MovieI] - AverageTable[i]) * PearsonTable[UserId][i];
                    sum2 += Math.Abs(PearsonTable[UserId][i]);
                    num++;
                }
            }
            if (num != 0)
                predict = AverageTable[UserId] + (sum1 / sum2);
            else predict = AverageTable[UserId];
            return predict;
        }

        private void InitialofPredictTable()
        {
            //Put the predicted ratings we calculated into the database table [CFDB].[dbo].[predictratings] 
            Connection con = new CFRMovie.Connection();
            for (int i = 1; i <= PredictSetNum; i++)
            {
                PredictTable[i] = new double[MovieNum + 1];
                for (int j = 1; j <= MovieNum; j++)
                    PredictTable[i][j] = 0;
            }
            for (int i = 1; i <= PredictSetNum; i++)
            {
                for (int j = 1; j <= MovieNum; j++)
                {           
                    if (RateTable[i + TrainSetNum][j] != 0)
                    { 
                        PredictTable[i][j] = PredictCompute(i, j);
                        if (PredictTable[i][j] > 0)
                        {
                            int reali = i + 780;
                            string sql = string.Format(@"IF EXISTS (SELECT * FROM [CFDB].[dbo].[predictratings] 
                                WHERE [CFDB].[dbo].[predictratings].userid= {0} and [CFDB].[dbo].[predictratings].movieid= {1}) 
                                UPDATE [CFDB].[dbo].[predictratings] set [CFDB].[dbo].[predictratings].predictrating = {2} 
                                WHERE [CFDB].[dbo].[predictratings].userid= {0} and [CFDB].[dbo].[predictratings].movieid= {1} 
                                ELSE INSERT INTO [CFDB].[dbo].[predictratings] ([userid] ,[movieid],[predictrating]) 
                                VALUES ({0},{1},{2})", reali, j, PredictTable[i][j]);                          
                            SqlCommand cmd = new SqlCommand(sql, con.ActiveCon());
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }          
        }

        private void OutputPredictTable()
        {      
            FileStream fs = new FileStream(savePath + "PredictTable.txt", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);

            for (int i = 1; i <= PredictSetNum; i++)
            {
                for (int j = 1; j <= MovieNum; j++)
                {
                    if (PredictTable[i][j] != 0)
                        sw.WriteLine("{0}\t{1}\t{2}", i + TrainSetNum, j, PredictTable[i][j]);
                }
            }
            sw.Close();      
        }
        
        private int NewUserSimlarUserFind(int userid, int userage, string usergender, string useroccupation)
        {
            //KNN algorithm, find the nearest user's id
            int similaruserid = 0;
            double mindistance = 100.0;
            double agedistance = 0;
            double genderdistance = 0;
            double occupationdistance = 0;         
            List<int> AgeTable = new List<int>();
            List<string> GenderTable = new List<string>();
            List<string> OccupationTable = new List<string>();
            string strSQL = "select * from dbo.[user] WHERE dbo.[user].userid>780";
            SqlDataReader drNew;
            SqlConnection con = new SqlConnection("Data Source=PSUCP3-PC\\SQLEXPRESS;Initial Catalog=CFDB;Integrated Security=True");
            SqlCommand cmd = new SqlCommand(strSQL, con);
            try
            {
                con.Open();
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                drNew = myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }
            if (drNew.HasRows)
            {
                while (drNew.Read())
                {
                    int id = int.Parse(drNew[0].ToString());
                    int age = int.Parse(drNew[1].ToString());
                    string gender = drNew[2].ToString();
                    string occupation = drNew[3].ToString();
                    AgeTable.Add (age);
                    GenderTable.Add (gender);
                    OccupationTable.Add (occupation);
                }
            }
            for (int id = 780; id <= 942; id++)
            {
                int age = AgeTable[id-780];
                string gender = GenderTable[id-780];
                string occupation = OccupationTable[id-780];
                if (string.Equals(gender, usergender))
                    genderdistance = 0;
                else genderdistance = 1;
                if (string.Equals(occupation, useroccupation))
                    occupationdistance = 0;
                else occupationdistance = 1;
                agedistance = (age - userage) * (age - userage) ;
                double distance = Math.Sqrt(agedistance/ 100.0 + genderdistance + occupationdistance);
                //distance calculation
                if (distance <= mindistance)
                {
                    mindistance = distance;
                    similaruserid = id + 1;
                }               
            }
            return similaruserid;
        }

        private int MaxOldUserid()
        {
            //Find the max userid in database <rating>,that is the max userid of old users with ratings history
            int maxolduserid = 0;
            Connection con = new CFRMovie.Connection();
            SqlDataReader drNew;
            SqlConnection con2 = new SqlConnection("Data Source=PSUCP3-PC\\SQLEXPRESS;Initial Catalog=CFDB;Integrated Security=True");
            SqlCommand cmd = new SqlCommand(@"select userid from dbo.[rating]", con2);
            try
            {
                con2.Open();
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                drNew = myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }
            if (drNew.HasRows)
            {
                while (drNew.Read())
                {
                    maxolduserid = int.Parse(drNew[0].ToString());
                }
            }
            return maxolduserid;
        }
        private void PrintPredictRatingsInOder()
        {
            //Print out the predicted ratings in order in database table dbo.[predictratings], selected by userid
            int maxolduserid = MaxOldUserid();
            int userid = int.Parse(textBox1.Text);
            int similaruserid = 0;
            if (userid < 1 && userid > 945)
            {
                MessageBox.Show("invalid user id!");
            }
            if (userid <= maxolduserid)//see if the user has rating history, if so ......
            { 
            Connection con = new CFRMovie.Connection();
            string sql = string.Format(@"SELECT * FROM dbo.[predictratings] WHERE dbo.[predictratings].userid = {0} ORDER BY dbo.[predictratings].predictrating DESC", textBox1.Text);
            DataSet ds = new DataSet();
            SqlDataAdapter command = new SqlDataAdapter(sql, con.ActiveCon());
            command.Fill(ds, "ds");
            this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
            }
            if (userid > maxolduserid)//see if the user has rating history, if not ......
            {            
                Connection con = new CFRMovie.Connection();
                string strSQL = string.Format(@"select * from dbo.[user] WHERE dbo.[user].userid={0}", textBox1.Text);
                SqlDataReader drNew;
                SqlConnection con2 = new SqlConnection("Data Source=PSUCP3-PC\\SQLEXPRESS;Initial Catalog=CFDB;Integrated Security=True");
                SqlCommand cmd = new SqlCommand(strSQL, con2);
                try
                {
                    con2.Open();
                    SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    drNew = myReader;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
                if (drNew.HasRows)
                {
                    while (drNew.Read())
                    {
                        int id = int.Parse(drNew[0].ToString());
                        int age = int.Parse(drNew[1].ToString());
                        string gender = drNew[2].ToString();
                        string occupation = drNew[3].ToString();
                        similaruserid = NewUserSimlarUserFind(id, age, gender, occupation);
                    }
                }
                string sql = string.Format(@"SELECT * FROM dbo.[predictratings] WHERE dbo.[predictratings].userid = {0} ORDER BY dbo.[predictratings].predictrating DESC", similaruserid);
                DataSet ds = new DataSet();
                SqlDataAdapter command = new SqlDataAdapter(sql, con.ActiveCon());
                command.Fill(ds, "ds");
                this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
            }
        }
       
        #endregion
        public Recommender()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitialofRateTable();
            InitialofPearsonTable();
            InitialofAverageTable();
            InitialofPredictTable();

            OutputRateTable();
            OutputAverTable();
            OutPutPearsonTable();
            OutputPredictTable();

            PrintPredictRatingsInOder();
            MessageBox.Show("finished");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }     
    }
}
