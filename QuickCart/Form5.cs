using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string query = @"
SELECT
    o.OrderId,
    c.CustomerName,
    ISNULL(STRING_AGG(p.ProductName, ', '), '') AS ProductName,
    ISNULL(SUM(oi.Quantity), 0) AS Quantity,
    o.TotalAmount
FROM dbo.Orders o
INNER JOIN dbo.Customers c ON c.CustomerId = o.CustomerId
LEFT JOIN dbo.OrderItems oi ON oi.OrderId = o.OrderId
LEFT JOIN dbo.Products p ON p.ProductId = oi.ProductId
GROUP BY o.OrderId, c.CustomerName, o.TotalAmount
ORDER BY o.OrderId DESC;";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            int orderId;

            if (txtOrderID.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Order ID");
                return;
            }

            if (!int.TryParse(txtOrderID.Text.Trim(), out orderId))
            {
                MessageBox.Show("Invalid Order ID");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string query = @"
SELECT
    o.OrderId,
    c.CustomerName,
    ISNULL(STRING_AGG(p.ProductName, ', '), '') AS ProductName,
    ISNULL(SUM(oi.Quantity), 0) AS Quantity,
    o.TotalAmount
FROM dbo.Orders o
INNER JOIN dbo.Customers c ON c.CustomerId = o.CustomerId
LEFT JOIN dbo.OrderItems oi ON oi.OrderId = o.OrderId
LEFT JOIN dbo.Products p ON p.ProductId = oi.ProductId
WHERE o.OrderId = @id
GROUP BY o.OrderId, c.CustomerName, o.TotalAmount;";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", orderId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Order not found");
                        ClearFields();
                        LoadOrders();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching order:\n" + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int orderId;

            if (txtOrderID.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Order ID");
                return;
            }

            if (!int.TryParse(txtOrderID.Text.Trim(), out orderId))
            {
                MessageBox.Show("Invalid Order ID");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this order?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes) return;

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    con.Open();

                    
                    string query = "DELETE FROM dbo.Orders WHERE OrderId = @id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", orderId);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        MessageBox.Show("Order not found");
                        return;
                    }
                }

                MessageBox.Show("Order Deleted Successfully");
                ClearFields();
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting order:\n" + ex.Message);
            }
        }

        
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            FillFieldsFromGrid(e.RowIndex);
        }

        
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            FillFieldsFromGrid(e.RowIndex);
        }

        private void FillFieldsFromGrid(int rowIndex)
        {
            try
            {
                if (rowIndex < 0) return;

                DataGridViewRow row = dataGridView1.Rows[rowIndex];

                txtOrderID.Text = row.Cells["OrderId"].Value.ToString();
                txtCustomerName.Text = row.Cells["CustomerName"].Value.ToString();
                txtProductName.Text = row.Cells["ProductName"].Value.ToString();
                txtQuantity.Text = row.Cells["Quantity"].Value.ToString();
                txtTotalAmount.Text = row.Cells["TotalAmount"].Value.ToString();
            }
            catch
            {
                
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            LoadOrders();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Form2 dashboard = new Form2();
            dashboard.Show();
            this.Close();
        }

        private void ClearFields()
        {
            txtOrderID.Clear();
            txtCustomerName.Clear();
            txtProductName.Clear();
            txtQuantity.Clear();
            txtTotalAmount.Clear();
            txtOrderID.Focus();
        }
    }
}
