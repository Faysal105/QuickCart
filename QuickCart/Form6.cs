using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string query = @"
SELECT 
    c.CustomerId,
    c.CustomerName,
    c.Email,
    c.Phone,
    u.Username
FROM dbo.Customers c
INNER JOIN dbo.Users u ON u.UserId = c.UserId
ORDER BY c.CustomerId DESC;";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;

                    dataGridView1.ReadOnly = true;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.AllowUserToAddRows = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers:\n" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 dashboard = new Form2();
            dashboard.Show();
            this.Close();
        }
    }
}
