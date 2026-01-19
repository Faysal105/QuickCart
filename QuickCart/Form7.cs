using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Form7 : Form
    {
        private int _userId;
        private int _sellerId;
        public Form7() : this(0) { }

        public Form7(int userId)
        {
            InitializeComponent();
            _userId = userId;

           
            button1.Click += button1_Click;  
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += btnClear_Click;
            btnBack.Click += btnBack_Click;

            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            if (!TryLoadSellerId())
            {
                MessageBox.Show("Seller not found for this user. Please login again or contact admin.");
                return;
            }

            LoadProducts(null);
        }

        private bool TryLoadSellerId()
        {
            if (_userId <= 0) return false;

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT TOP 1 SellerId FROM dbo.Sellers WHERE UserId=@uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", _userId);
                    con.Open();

                    object obj = cmd.ExecuteScalar();
                    if (obj == null) return false;

                    _sellerId = Convert.ToInt32(obj);
                    return _sellerId > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading seller session:\n" + ex.Message);
                return false;
            }
        }

        
        private void LoadProducts(string filter)
        {
            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string q = @"
SELECT ProductId, ProductName, Price, Stock, IsActive
FROM dbo.Products
WHERE SellerId = @sid
";

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@sid", _sellerId);

                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        filter = filter.Trim();
                        if (int.TryParse(filter, out int pid))
                        {
                            q += " AND ProductId = @pid";
                            cmd.Parameters.AddWithValue("@pid", pid);
                        }
                        else
                        {
                            q += " AND ProductName LIKE @name";
                            cmd.Parameters.AddWithValue("@name", "%" + filter + "%");
                        }
                    }

                    q += " ORDER BY ProductId DESC;";
                    cmd.CommandText = q;

                    DataTable dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products:\n" + ex.Message);
            }
        }

        
        private void btnSearch_Click(object sender, EventArgs e)
        {
           
            LoadProducts(textBox2.Text);
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            if (_sellerId <= 0)
            {
                MessageBox.Show("Seller session invalid. Please login again.");
                return;
            }

            string name = txtProductName.Text.Trim();
            if (name == "")
            {
                MessageBox.Show("Product Name is required.");
                return;
            }

            if (!decimal.TryParse(txtTotalAmount.Text.Trim(), out decimal price))
            {
                MessageBox.Show("Total Amount (Price) must be a valid number.");
                return;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out int stock))
            {
                MessageBox.Show("Quantity (Stock) must be a valid integer.");
                return;
            }

            if (stock < 0)
            {
                MessageBox.Show("Stock cannot be negative.");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.Products (SellerId, ProductName, Price, Stock, IsActive)
VALUES (@sid, @name, @price, @stock, 1);", con))
                {
                    cmd.Parameters.AddWithValue("@sid", _sellerId);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@stock", stock);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Product added successfully.");
                ClearFields();
                LoadProducts(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product:\n" + ex.Message);
            }
        }

        // ===== UPDATE =====
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text.Trim(), out int productId))
            {
                MessageBox.Show("Select a product first (Product ID required).");
                return;
            }

            string name = txtProductName.Text.Trim();
            if (name == "")
            {
                MessageBox.Show("Product Name is required.");
                return;
            }

            if (!decimal.TryParse(txtTotalAmount.Text.Trim(), out decimal price))
            {
                MessageBox.Show("Total Amount (Price) must be a valid number.");
                return;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out int stock))
            {
                MessageBox.Show("Quantity (Stock) must be a valid integer.");
                return;
            }

            if (stock < 0)
            {
                MessageBox.Show("Stock cannot be negative.");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.Products
SET ProductName=@name, Price=@price, Stock=@stock
WHERE ProductId=@pid AND SellerId=@sid;", con))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.Parameters.AddWithValue("@pid", productId);
                    cmd.Parameters.AddWithValue("@sid", _sellerId);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        MessageBox.Show("Update failed. (Not found or not your product)");
                        return;
                    }
                }

                MessageBox.Show("Product updated successfully.");
                ClearFields();
                LoadProducts(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating product:\n" + ex.Message);
            }
        }

        
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text.Trim(), out int productId))
            {
                MessageBox.Show("Select a product first (Product ID required).");
                return;
            }

            DialogResult dr = MessageBox.Show(
                "Are you sure you want to delete this product?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (dr != DialogResult.Yes) return;

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
DELETE FROM dbo.Products
WHERE ProductId=@pid AND SellerId=@sid;", con))
                {
                    cmd.Parameters.AddWithValue("@pid", productId);
                    cmd.Parameters.AddWithValue("@sid", _sellerId);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        MessageBox.Show("Delete failed. (Not found or not your product)");
                        return;
                    }
                }

                MessageBox.Show("Product deleted successfully.");
                ClearFields();
                LoadProducts(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting product:\n" + ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            LoadProducts(null);
        }

        private void ClearFields()
        {
            textBox1.Clear();        
            txtProductName.Clear();  
            txtTotalAmount.Clear();  
            txtQuantity.Clear();     
            txtProductName.Focus();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Form8 dash = new Form8(_userId);
            dash.Show();
            this.Close();
        }

        
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            
            textBox1.Text = row.Cells["ProductId"].Value?.ToString() ?? "";
            txtProductName.Text = row.Cells["ProductName"].Value?.ToString() ?? "";
            txtTotalAmount.Text = row.Cells["Price"].Value?.ToString() ?? "";
            txtQuantity.Text = row.Cells["Stock"].Value?.ToString() ?? "";
        }
    }
}
