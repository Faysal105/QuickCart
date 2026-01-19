using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Form9 : Form
    {
        private int _userId;
        private int _sellerId;

        public Form9() : this(0) { }

        public Form9(int userId)
        {
            InitializeComponent();
            _userId = userId;

            this.Load += Form9_Load;
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

        private void Form9_Load(object sender, EventArgs e)
        {
            if (!TryLoadSellerId())
            {
                MessageBox.Show("Seller not found. Please login again.");
                return;
            }
            if (button1 != null) button1.Click += button1_Click;
            if (button2 != null) button2.Click += button2_Click; 
            if (button4 != null) button4.Click += button4_Click; 
            if (button5 != null) button5.Click += button5_Click; 
            if (button6 != null) button6.Click += button6_Click;

            dataGridView1.CellClick += dataGridView1_CellClick;

            comboBox1.Items.Clear();
            comboBox1.Items.Add("Pending");
            comboBox1.Items.Add("Shipped");
            comboBox1.Items.Add("Delivered");
            comboBox1.Items.Add("Cancelled");
            comboBox1.SelectedIndex = 0;

            LoadOrders(null);
        }

        private void LoadOrders(string mode)
        {
            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string q = @"
SELECT DISTINCT
    o.OrderId,
    c.CustomerName,
    o.TotalAmount,
    o.Status AS OrderStatus,
    ISNULL(d.DeliveryStatus, 'Pending') AS DeliveryStatus,
    d.UpdatedAt
FROM dbo.Orders o
JOIN dbo.Customers c ON c.CustomerId = o.CustomerId
JOIN dbo.OrderItems oi ON oi.OrderId = o.OrderId
JOIN dbo.Products p ON p.ProductId = oi.ProductId
LEFT JOIN dbo.Deliveries d ON d.OrderId = o.OrderId
WHERE p.SellerId = @sid
";

                    if (mode == "DONE")
                        q += "AND ISNULL(d.DeliveryStatus,'') = 'Delivered' \n";
                    else if (mode == "PENDING")
                        q += "AND ISNULL(d.DeliveryStatus,'Pending') <> 'Delivered' AND ISNULL(d.DeliveryStatus,'Pending') <> 'Cancelled' \n";

                    q += "ORDER BY o.OrderId DESC;";

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
                MessageBox.Show("Error loading orders:\n" + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e) => LoadOrders(null);
        private void button4_Click(object sender, EventArgs e) => LoadOrders("DONE");
        private void button5_Click(object sender, EventArgs e) => LoadOrders("PENDING");

        private void button2_Click(object sender, EventArgs e)
        {
            int orderId;
            if (!int.TryParse(textBox1.Text.Trim(), out orderId))
            {
                MessageBox.Show("Select a valid Order ID.");
                return;
            }

            string deliveryStatus = comboBox1.Text.Trim();
            if (deliveryStatus == "")
            {
                MessageBox.Show("Select a delivery status.");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    con.Open();

                    
                    using (SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM dbo.Orders WHERE OrderId=@id", con))
                    {
                        chk.Parameters.AddWithValue("@id", orderId);
                        if (Convert.ToInt32(chk.ExecuteScalar()) == 0)
                        {
                            MessageBox.Show("Order not found.");
                            return;
                        }
                    }

                    
                    string upsert = @"
IF EXISTS (SELECT 1 FROM dbo.Deliveries WHERE OrderId=@oid)
BEGIN
    UPDATE dbo.Deliveries SET DeliveryStatus=@st, UpdatedAt=GETDATE() WHERE OrderId=@oid;
END
ELSE
BEGIN
    INSERT INTO dbo.Deliveries (OrderId, DeliveryStatus, UpdatedAt)
    VALUES (@oid, @st, GETDATE());
END
";
                    using (SqlCommand cmd = new SqlCommand(upsert, con))
                    {
                        cmd.Parameters.AddWithValue("@oid", orderId);
                        cmd.Parameters.AddWithValue("@st", deliveryStatus);
                        cmd.ExecuteNonQuery();
                    }
                    if (deliveryStatus.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                    {
                        using (SqlCommand cmd2 = new SqlCommand("UPDATE dbo.Orders SET Status='Completed' WHERE OrderId=@id", con))
                        {
                            cmd2.Parameters.AddWithValue("@id", orderId);
                            cmd2.ExecuteNonQuery();
                        }
                    }
                    else if (deliveryStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                    {
                        using (SqlCommand cmd2 = new SqlCommand("UPDATE dbo.Orders SET Status='Cancelled' WHERE OrderId=@id", con))
                        {
                            cmd2.Parameters.AddWithValue("@id", orderId);
                            cmd2.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Delivery status updated.");
                LoadOrders(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating delivery:\n" + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            textBox1.Text = row.Cells["OrderId"].Value.ToString();
            comboBox1.Text = row.Cells["DeliveryStatus"].Value.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form8 sellerDash = new Form8(_userId);
            sellerDash.Show();
            this.Hide();
        }
    }
}
