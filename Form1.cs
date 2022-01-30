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
using Microsoft.Extensions.Configuration;

namespace ADO_NET
{
    public partial class Form1 : Form
    {
        
        private SqlConnection connection;
        private IConfiguration configuration;
        public Form1()
        {
            String currentDir = Application.StartupPath;
            int binIndex = currentDir.IndexOf("\\bin\\");
            String projectPath =
                binIndex == -1
                ? currentDir
                : currentDir.Substring(0, binIndex);
                currentDir.Substring(0, binIndex);
            try
            {
                configuration =
                    new ConfigurationBuilder()
                    .AddJsonFile(projectPath + "\\appsettings.json")
                    .Build();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                MessageBox.Show("Configuration not found");
                Close();
                Application.Exit();
                return;
            }
            InitializeComponent();
        }
     

        private void button1_Click_1(object sender, EventArgs e)
        {
            String ConnectionStringName = null;
            //which radiobutton is checked?
            if (radioButton1.Checked)
            {
                ConnectionStringName = radioButton1.Text; // "numDb"
            }
            if(radioButton2.Checked)
            {
                ConnectionStringName = radioButton2.Text;
            }
            if (ConnectionStringName == null)
            {
                MessageBox.Show("Select connection string");
                return;
            }
            //if old connection is alive
            if (connection?.State == ConnectionState.Open)
            {
                connection.Close();

            }
            connection = new SqlConnection(
                configuration.GetConnectionString("numDb")
            );
            try
            {
                connection.Open();
                listBox1.Items.Add("Connected");
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            buttonCreate.Enabled = true;
            buttonInsert.Enabled = true;
            buttonSelect.Enabled = true;
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            // создание таблицы
            cmd.CommandText = @"
                CREATE TABLE nums(
                  id  UNIQUEIDENTIFIER PRIMARY KEY,
                  num INT
                ) ";
            try
            {
                cmd.ExecuteNonQuery();
                listBox1.Items.Add("CREATE TABLE Ok");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            using(SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO Nums"
                    + " VALUES( NEWID(), "
                    + DateTime.Now.Millisecond
                    + ")";
                try
                {
                    cmd.ExecuteNonQuery();
                    listBox1.Items.Add("INSERT Ok");
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            if(connection == null)
            {
                listBox1.Items.Add("No connection");
                return;
            }
            using (SqlCommand cmd = 
                new SqlCommand("SELECT * FROM nums",connection))
            {
                try
                {
                    SqlDataReader result =
                        cmd.ExecuteReader();
                    listBox1.Items.Add("-----------------");
                    while (result.Read())
                    {
                        listBox1.Items.Add(
                            String.Format(
                                "{0} - {1}",
                                result.GetGuid("id"),
                                result.GetInt32(1)
                                ));
                    }
                    result.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }

        private void buttonORM_Click(object sender, EventArgs e)
        {
            ContextDb ctx = new ContextDb(connection);
            foreach (var num in ctx.Nums)
            {
                listBox1.Items.Add(num);
            }
            dataGridView1.DataSource = ctx.Nums;
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
                listBox1.Items.Add("Connection closed!");
            }
        }
    }
}
