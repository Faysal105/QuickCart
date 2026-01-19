using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Form13 : Form
    {
        private int _userId;
        private int _customerId;

        public Form13() : this(0) { }

        public Form13(int userId)
        {
            InitializeComponent();
            _userId = userId;

            btnSearch.Click += btnSearch_Click;
            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            button5.Click += button5_Click; 
            button1.Click += button1_Click;
            dataGridView1.CellClick += dataGridView1_CellClick;

            this.Load += Form13_Load;
        }

        private void Form13_Load(object sender, EventArgs e)
        {
            if (!TryLoadCustomerId())
            {
                MessageBox.Show("Customer not found. Please login again.");
                return;
            }

            LoadProducts(null);
        }

        private bool TryLoadCustomerId()
        {
            if (_userId <= 0) return false;

            using (SqlConnection con = DataAccess.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 CustomerId FROM dbo.Customers WHERE UserId=@uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", _userId);
                    object obj = cmd.ExecuteScalar();
                    if (obj == null) return false;
                    _customerId = Convert.ToInt32(obj);
                    return _customerId > 0;
                }
            }
        }

        private void LoadProducts(string filter)
        {
            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string q = @"
SELECT ProductId, ProductName, Price, Stock
FROM dbo.Products
WHERE IsActive = 1 AND Stock > 0
";

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                      
                        int pid;
                        if (int.TryParse(filter.Trim(), out pid))
                        {
                            q += " AND ProductId = @pid";
                            cmd.Parameters.AddWithValue("@pid", pid);
                        }
                        else
                        {
                            q += " AND ProductName LIKE @name";
                            cmd.Parameters.AddWithValue("@name", "%" + filter.Trim() + "%");
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

        private int GetOrCreateActiveCartId(SqlConnection con, SqlTransaction tx)
        {
            
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 CartId
FROM dbo.Carts
WHERE CustomerId=@cid AND Status='Active'
ORDER BY CartId DESC;", con, tx))
            {
                cmd.Parameters.AddWithValue("@cid", _customerId);
                object obj = cmd.ExecuteScalar();
                if (obj != null) return Convert.ToInt32(obj);
            }

            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.Carts (CustomerId, Status, CreatedAt)
VALUES (@cid, 'Active', GETDATE());
SELECT SCOPE_IDENTITY();", con, tx))
            {
                cmd.Parameters.AddWithValue("@cid", _customerId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadProducts(txtProductID.Text.Trim());
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int productId, qty;

            if (!int.TryParse(txtProductID.Text.Trim(), out productId))
            {
                MessageBox.Show("Enter/select a valid ProductId.");
                return;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out qty) || qty <= 0)
            {
                MessageBox.Show("Enter valid quantity (>0).");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    con.Open();
                    SqlTransaction tx = con.BeginTransaction();

                    try
                    {
                       
                        decimal price;
                        int stock;

                        using (SqlCommand cmd = new SqlCommand(@"
SELECT Price, Stock
FROM dbo.Products
WHERE ProductId=@pid AND IsActive=1;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@pid", productId);

                            using (SqlDataReader r = cmd.ExecuteReader())
                            {
                                if (!r.Read())
                                {
                                    tx.Rollback();
                                    MessageBox.Show("Product not found or inactive.");
                                    return;
                                }

                                price = Convert.ToDecimal(r["Price"]);
                                stock = Convert.ToInt32(r["Stock"]);
                            }
                        }

                        if (qty > stock)
                        {
                            tx.Rollback();
                            MessageBox.Show("Not enough stock.");
                            return;
                        }

                        
                        int cartId = GetOrCreateActiveCartId(con, tx);

                        
                        using (SqlCommand cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM dbo.CartItems WHERE CartId=@cid AND ProductId=@pid)
BEGIN
    UPDATE dbo.CartItems
    SET Quantity = Quantity + @q,
        UnitPrice = @price
    WHERE CartId=@cid AND ProductId=@pid;
END
ELSE
BEGIN
    INSERT INTO dbo.CartItems (CartId, ProductId, Quantity, UnitPrice)
    VALUES (@cid, @pid, @q, @price);
END
", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@cid", cartId);
                            cmd.Parameters.AddWithValue("@pid", productId);
                            cmd.Parameters.AddWithValue("@q", qty);
                            cmd.Parameters.AddWithValue("@price", price);
                            cmd.ExecuteNonQuery();
                        }

                        
                        using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.Products
SET Stock = Stock - @q
WHERE ProductId=@pid AND Stock >= @q;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@pid", productId);
                            cmd.Parameters.AddWithValue("@q", qty);

                            int rows = cmd.ExecuteNonQuery();
                            if (rows == 0)
                            {
                                tx.Rollback();
                                MessageBox.Show("Stock changed. Try again.");
                                return;
                            }
                        }

                        tx.Commit();
                        LoadProducts(null);
                        MessageBox.Show("Added to cart. Stock updated.");
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding to cart:\n" + ex.Message);
            }
        }


       
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int productId;
            if (!int.TryParse(txtProductID.Text.Trim(), out productId))
            {
                MessageBox.Show("Enter/select a ProductId.");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    con.Open();
                    SqlTransaction tx = con.BeginTransaction();

                    try
                    {
                        
                        int cartId = 0;
                        using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 CartId
FROM dbo.Carts
WHERE CustomerId=@cid AND Status='Active'
ORDER BY CartId DESC;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@cid", _customerId);
                            object obj = cmd.ExecuteScalar();
                            if (obj != null) cartId = Convert.ToInt32(obj);
                        }

                        if (cartId == 0)
                        {
                            tx.Rollback();
                            MessageBox.Show("No active cart found.");
                            return;
                        }

                        int cartQty = 0;
                        using (SqlCommand cmd = new SqlCommand(@"
SELECT Quantity
FROM dbo.CartItems
WHERE CartId=@cart AND ProductId=@pid;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@cart", cartId);
                            cmd.Parameters.AddWithValue("@pid", productId);

                            object obj = cmd.ExecuteScalar();
                            if (obj == null)
                            {
                                tx.Rollback();
                                MessageBox.Show("Item not found in cart.");
                                return;
                            }
                            cartQty = Convert.ToInt32(obj);
                        }

                        using (SqlCommand cmd = new SqlCommand(@"
DELETE FROM dbo.CartItems
WHERE CartId=@cart AND ProductId=@pid;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@cart", cartId);
                            cmd.Parameters.AddWithValue("@pid", productId);
                            cmd.ExecuteNonQuery();
                        }

                        
                        using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.Products
SET Stock = Stock + @q
WHERE ProductId=@pid;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@pid", productId);
                            cmd.Parameters.AddWithValue("@q", cartQty);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        LoadProducts(null);
                        MessageBox.Show("Removed from cart. Stock restored.");
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing from cart:\n" + ex.Message);
            }
        }


        private void button5_Click(object sender, EventArgs e)
        {
            Form11 dash = new Form11(_userId);
            dash.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtProductID.Clear();
            txtName.Clear();
            txtPrice.Clear();
            txtQuantity.Clear();
            LoadProducts(null);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];
            txtProductID.Text = row.Cells["ProductId"].Value.ToString();
            txtName.Text = row.Cells["ProductName"].Value.ToString();
            txtPrice.Text = row.Cells["Price"].Value.ToString();
            txtQuantity.Text = "1";
        }
    }
}
