﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SQLite;

namespace ProyekAkhir_DummyKasir
{
    public partial class FormUtama : Form
    {
        public FormUtama()
        {
            InitializeComponent();
        }

        static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            try{
                sqlite_conn.Open();
            }catch (Exception ex){
                Console.WriteLine(ex);
            }
            return sqlite_conn;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();

            string stm = "SELECT SQLITE_VERSION()";
            var cmd = new SQLiteCommand(stm, sqlite_conn);
            string version = cmd.ExecuteScalar().ToString();

            toolStripStatusLabel2.Text = version;

            sqlite_conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.Show();
        }
    }
}