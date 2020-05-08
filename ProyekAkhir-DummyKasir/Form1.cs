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
using Microsoft.VisualBasic;

namespace ProyekAkhir_DummyKasir {
    public partial class FormUtama : Form {

        int jumlah_barang = 0;
        int harga_bayar = 0;
        bool updated = false;
        public FormUtama() {
            InitializeComponent();
        }

        static SQLiteConnection CreateConnection() {
            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; Compress = True; ");
            // Open the connection:
            try {
                sqlite_conn.Open();
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return sqlite_conn;
        }

        private void Form1_Load(object sender, EventArgs e) {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();

            string stm = "SELECT SQLITE_VERSION()";
            var cmd = new SQLiteCommand(stm, sqlite_conn);
            string version = cmd.ExecuteScalar().ToString();

            toolStripStatusLabel2.Text = version;

            sqlite_conn.Close();
            Refresh_data();
            Update_label(true);
        }

        private void Hapus_keranjang(bool bayar = false) {
            if (bayar) {
                SQLiteConnection sqlite_conn;
                sqlite_conn = CreateConnection();

                try {
                    var cmd = new SQLiteCommand(sqlite_conn);
                    cmd.CommandText = "DROP TABLE IF EXISTS Keranjang";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = @"CREATE TABLE Keranjang ( id INTEGER PRIMARY KEY AUTOINCREMENT, deskripsi TEXT NOT NULL,
                    jumlah INTEGER NOT NULL, 'harga satuan' INTEGER NOT NULL, total INTEGER NOT NULL, barcode TEXT NOT NULL )";
                    cmd.ExecuteNonQuery();
                } catch (SQLiteException hapus_ex) {
                    MessageBox.Show(hapus_ex.ToString());
                }

                sqlite_conn.Close();

                Refresh_data();
                Update_label(true);
            } else {
                SQLiteConnection sqlite_conn;
                sqlite_conn = CreateConnection();

                var confirmResult = MessageBox.Show("Anda yakin akan menghapus keranjang ?",
                                         "Hapus Keranjang ?",
                                         MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes) {
                    try {
                        var cmd = new SQLiteCommand(sqlite_conn);
                        cmd.CommandText = "DROP TABLE IF EXISTS Keranjang";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = @"CREATE TABLE Keranjang ( id INTEGER PRIMARY KEY AUTOINCREMENT, deskripsi TEXT NOT NULL,
                    jumlah INTEGER NOT NULL, 'harga satuan' INTEGER NOT NULL, total INTEGER NOT NULL, barcode TEXT NOT NULL )";
                        cmd.ExecuteNonQuery();
                    } catch (SQLiteException hapus_ex) {
                        MessageBox.Show(hapus_ex.ToString());
                    }

                } else {

                }

                sqlite_conn.Close();

                Refresh_data();
                Update_label(true);
            }
            
        }

        private void button1_Click(object sender, EventArgs e) {
            Hapus_keranjang();
        }

        private void button2_Click(object sender, EventArgs e) {
            Form2 frm = new Form2();
            frm.Show();
        }

        private void Update_Keranjang() {
            
        }

        private void button4_Click(object sender, EventArgs e) {
            //string final_format = Konversi_duit(textBox1.Text);
            //Console.WriteLine($"Rp. {final_format}");

            //string dataSource = "database.db";
            //using (SQLiteConnection connection = new SQLiteConnection()) {
            //    connection.ConnectionString = "Data Source=" + dataSource;
            //    connection.Open();
            //    using (SQLiteCommand command = new SQLiteCommand(connection)) {
            //        command.CommandText =
            //            "update Keranjang set jumlah = jumlah+1 where barcode=:bar";
            //        command.Parameters.Add("bar", DbType.String).Value = textBox1.Text;
            //        command.ExecuteNonQuery();
            //    }
            //}

            //Update_label(false);

        }

        private string Konversi_duit(string duit) {
            try {
                int format_int = Int32.Parse(duit);
                string format_duit = string.Format("{0:#,0}", format_int);

                return format_duit;
            } catch (OverflowException) {
                MessageBox.Show("Pastikan Jumlah uang ditulis dengan benar!");
            }
            return "";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
            int total = 0;
            string desc = "";
            int harga = 0;
            string barc = "";

            if (e.KeyChar == (char)Keys.Escape) {
                jumlah_barang = Int32.Parse(textBox1.Text);
                textBox1.Text = "";
            } else if (e.KeyChar == (char)Keys.Enter) {
                try {
                    SQLiteConnection sqlite_conn;
                    sqlite_conn = CreateConnection();

                    SQLiteConnection tulis;
                    tulis = CreateConnection();

                    SQLiteConnection update1;
                    update1 = CreateConnection();

                    //Select Item -> Barcode
                    string stm = "SELECT * FROM Inventory where barcode=@barcode";
                    var cmd = new SQLiteCommand(stm, sqlite_conn);
                    cmd.Parameters.Add(new SQLiteParameter("@barcode", textBox1.Text));
                    SQLiteDataReader rdr = cmd.ExecuteReader();

                    if (!rdr.HasRows) {
                        MessageBox.Show("Barcode tidak ditemukan!");
                        textBox1.Text = "";
                        textBox1.Focus();
                        return;
                    }

                    //Tambah ke Keranjang
                    while (rdr.Read()) {
                        //Console.WriteLine($"{rdr.GetInt32(0)} {rdr.GetString(1)} {rdr.GetInt32(2)} {rdr.GetString(3)}");
                        desc = rdr.GetString(1);
                        harga = rdr.GetInt32(2);
                        barc = rdr.GetString(3);
                    }

                    //Console.WriteLine($"{desc} {harga} {barc}");
                    //Console.WriteLine($"{jumlah_barang}");

                    string baca_keranjang = "SELECT * FROM Keranjang where barcode=@barcode";
                    var cmd_baca_keranjang = new SQLiteCommand(baca_keranjang, tulis);
                    cmd_baca_keranjang.Parameters.Add(new SQLiteParameter("@barcode", textBox1.Text));
                    SQLiteDataReader rdr_keranjang = cmd_baca_keranjang.ExecuteReader();

                    if (!rdr_keranjang.HasRows) {
                        try {
                            var cmd_tulis = new SQLiteCommand(tulis);
                            cmd_tulis.CommandText = "INSERT INTO Keranjang('deskripsi', 'jumlah', 'harga satuan', 'total', 'barcode') " +
                                "VALUES(@desc, @jml, @satuan, @tot, @barc)";
                            cmd_tulis.Parameters.AddWithValue("@desc", desc);
                            if (jumlah_barang == 0) {
                                cmd_tulis.Parameters.AddWithValue("@jml", 1);
                                total = harga * 1;
                            } else {
                                cmd_tulis.Parameters.AddWithValue("@jml", jumlah_barang);
                                total = harga * jumlah_barang;
                            }
                            cmd_tulis.Parameters.AddWithValue("@satuan", harga);
                            cmd_tulis.Parameters.AddWithValue("@tot", total);
                            cmd_tulis.Parameters.AddWithValue("@barc", barc);
                            cmd_tulis.Prepare();
                            cmd_tulis.ExecuteNonQuery();
                            jumlah_barang = 0;
                        } catch (Exception tulis_ex) {
                            MessageBox.Show(tulis_ex.ToString());
                        }
                    } else {
                        
                        //var cmd_update = new SQLiteCommand(update1);
                        //cmd_update.CommandText = "UPDATE Keranjang set jumlah = jumlah+1 WHERE barcode=@barc";
                        //cmd_update.Parameters.AddWithValue("@barc", textBox1);
                        //cmd_update.Prepare();
                        //cmd_update.ExecuteNonQuery();
                        
                    }

                    sqlite_conn.Close();
                    tulis.Close();
                    update1.Close();

                    

                    textBox1.Text = "";
                    label6.Text = harga.ToString();
                    textBox1.Focus();

                    Refresh_data();
                    Update_label(false);
                    
                } catch (SQLiteException sqli_ex) {
                    MessageBox.Show(sqli_ex.ToString());
                }

                //button4.PerformClick();

            }
        }

        private void Refresh_data() {
            SQLiteConnection baca;
            baca = CreateConnection();

            DataTable dt = new DataTable();
            SQLiteDataAdapter adapt = new SQLiteDataAdapter("select * from Keranjang", baca);
            adapt.Fill(dt);
            dataGridView1.DataSource = dt;

            baca.Close();
        }

        private void Update_label(bool baru) {
            int tot = 0;
            if (baru) {
                label6.Text = "0";
                label2.Text = "Rp.0";
                label10.Text = "Rp. 0";
                label11.Text = "Rp. 0";
            } else {
                SQLiteConnection baca;
                baca = CreateConnection();

                string stm = "SELECT sum(total) FROM Keranjang";
                var cmd = new SQLiteCommand(stm, baca);
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read()) {
                    tot = rdr.GetInt32(0);
                }

                label2.Text = tot.ToString();
                label10.Text = tot.ToString();
                label11.Text = tot.ToString();
                harga_bayar = tot;

                baca.Close();
            }
        }

        private void FormUtama_FormClosing(object sender, FormClosingEventArgs e) {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();

            try {
                var cmd = new SQLiteCommand(sqlite_conn);
                cmd.CommandText = "DROP TABLE IF EXISTS Keranjang";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"CREATE TABLE Keranjang ( id INTEGER PRIMARY KEY AUTOINCREMENT, deskripsi TEXT NOT NULL,
                    jumlah INTEGER NOT NULL, 'harga satuan' INTEGER NOT NULL, total INTEGER NOT NULL, barcode TEXT NOT NULL )";
                cmd.ExecuteNonQuery();
            } catch (SQLiteException hapus_ex) {
                MessageBox.Show(hapus_ex.ToString());
            }

            sqlite_conn.Close();
        }

        private void button5_Click(object sender, EventArgs e) {
            int kembalian = 0;
            var bayar = Microsoft.VisualBasic.Interaction.InputBox("Masukan jumlah uang tunai", "Pembayaran Tunai");
            //Console.WriteLine(bayar);
            kembalian = Int32.Parse(bayar) - harga_bayar;
            MessageBox.Show("Kembalian Pelanggan : " + kembalian.ToString(), "Kembalian");

            Hapus_keranjang(true);
        }

        private void textBox1_Enter(object sender, EventArgs e) {
           
        }

        private void button3_Click(object sender, EventArgs e) {
            
        }
    }
}
