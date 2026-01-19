using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Form10 : Form
    {
        private int _userId;
        private int _sellerId;

        public Form10() : this(0) { }

        public Form10(int userId)
        {
            InitializeComponent();
            _userId = userId;

            this.Load += Form10_Load;
            button1.Click += button1_Click; 
            button2.Click += button2_Click; 
            button4.Click += button4_Click; 
            button5.Click += button5_Click; 
        }

        private bool TryLoadSellerId()
        {
            if (_userId <= 0) return false;

            using (SqlConnection con = DataAccess.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 SellerId FROM dbo.Sellers WHERE UserId=@uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", _userId);
                    object obj = cmd.ExecuteScalar();
                    if (obj == null) return false;
                    _sellerId = Convert.ToInt32(obj);
                    return _sellerId > 0;
                }
            }
        }

        private void Form10_Load(object sender, EventArgs e)
        {
            if (!TryLoadSellerId())
            {
                MessageBox.Show("Seller not found. Please login again.");
                return;
            }

            LoadCompletedSales(); 
        }

        private void LoadCompletedSales()
        {
            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string q = @"
SELECT
    o.OrderId,
    o.OrderDate,
    c.CustomerName,
    p.ProductName,
    oi.Quantity,
    oi.UnitPrice,
    (oi.Quantity * oi.UnitPrice) AS LineTotal
FROM dbo.Orders o
JOIN dbo.Customers c ON c.CustomerId = o.CustomerId
JOIN dbo.OrderItems oi ON oi.OrderId = o.OrderId
JOIN dbo.Products p ON p.ProductId = oi.ProductId
WHERE o.Status = 'Completed'
  AND p.SellerId = @sid
ORDER BY o.OrderId DESC;";

                    SqlCommand cmd = new SqlCommand(q, con);
                    cmd.Parameters.AddWithValue("@sid", _sellerId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading completed sales:\n" + ex.Message);
            }
        }

        private decimal GetTotalEarnings()
        {
            using (SqlConnection con = DataAccess.GetConnection())
            {
                con.Open();
                string q = @"
SELECT ISNULL(SUM(oi.Quantity * oi.UnitPrice), 0)
FROM dbo.OrderItems oi
JOIN dbo.Products p ON p.ProductId = oi.ProductId
JOIN dbo.Orders o ON o.OrderId = oi.OrderId
WHERE o.Status = 'Completed'
  AND p.SellerId = @sid;";

                using (SqlCommand cmd = new SqlCommand(q, con))
                {
                    cmd.Parameters.AddWithValue("@sid", _sellerId);
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                decimal total = GetTotalEarnings();
                MessageBox.Show("Total Earnings (Completed Orders): " + total.ToString("0.00"));
                LoadCompletedSales();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating earnings:\n" + ex.Message);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            LoadCompletedSales();
        }     
        private void button4_Click(object sender, EventArgs e)
        {
            LoadCompletedSales();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form8 dash = new Form8(_userId);
            dash.Show();
            this.Hide();
        }
        private void button1_Click_1(object sender, EventArgs e) { }
        private void button4_Click_1(object sender, EventArgs e) { }
    }
}
