using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SQLite;

namespace ProyekAkhir_DummyKasir {
    public partial class Form2 : Form {
        public Form2() {
            InitializeComponent();
        }

        static SQLiteConnection CreateConnection() {
            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; Compress = True;");
            // Open the connection:
            try {
                sqlite_conn.Open();
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return sqlite_conn;
        }

        private void button2_Click(object sender, EventArgs e) {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void button1_Click(object sender, EventArgs e) {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();

            try {
                var cmd = new SQLiteCommand(sqlite_conn);
                cmd.CommandText = "INSERT INTO Inventory(nama, harga, barcode) VALUES(@nama, @harga, @barcode)";
                cmd.Parameters.AddWithValue("@nama", textBox2.Text);
                cmd.Parameters.AddWithValue("@harga", textBox3.Text);
                cmd.Parameters.AddWithValue("@barcode", textBox1.Text);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            } catch(Exception tambah_ex) {
                MessageBox.Show(tambah_ex.ToString());
            }

            MessageBox.Show("Data berhasil ditambah!");
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox1.Focus();

            sqlite_conn.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)Keys.Enter) {
                textBox2.Focus();
            }
        }
    }
}
