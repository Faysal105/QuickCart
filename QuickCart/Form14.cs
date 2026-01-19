using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Form14 : Form
    {
        private int _userId;
        private int _customerId;

        public Form14() : this(0) { }

        public Form14(int userId)
        {
            InitializeComponent();
            _userId = userId;

            btnSearch.Click += btnSearch_Click;
            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            button5.Click += button5_Click;   
            button1.Click += button1_Click;   
            dataGridView1.CellClick += dataGridView1_CellClick;

            this.Load += Form14_Load;
        }

        private void Form14_Load(object sender, EventArgs e)
        {
            if (!TryLoadCustomerId())
            {
                MessageBox.Show("Customer not found. Please login again.");
                return;
            }

            LoadCart();
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

        private int GetActiveCartId(SqlConnection con)
        {
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 CartId
FROM dbo.Carts
WHERE CustomerId=@cid AND Status='Active'
ORDER BY CartId DESC;", con))
            {
                cmd.Parameters.AddWithValue("@cid", _customerId);
                object obj = cmd.ExecuteScalar();
                return obj == null ? 0 : Convert.ToInt32(obj);
            }
        }

        private void LoadCart()
        {
            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    con.Open();
                    int cartId = GetActiveCartId(con);
                    if (cartId == 0)
                    {
                        dataGridView1.DataSource = null;
                        textBox1.Text = "0";
                        return;
                    }

                    string q = @"
SELECT
    ci.CartItemId,
    ci.ProductId,
    p.ProductName,
    ci.Quantity,
    ci.UnitPrice,
    (ci.Quantity * ci.UnitPrice) AS LineTotal
FROM dbo.CartItems ci
JOIN dbo.Products p ON p.ProductId = ci.ProductId
WHERE ci.CartId = @cart
ORDER BY ci.CartItemId DESC;";

                    SqlCommand cmd = new SqlCommand(q, con);
                    cmd.Parameters.AddWithValue("@cart", cartId);

                    DataTable dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;

                    
                    decimal total = 0m;
                    foreach (DataRow r in dt.Rows)
                        total += Convert.ToDecimal(r["LineTotal"]);

                    textBox1.Text = total.ToString("0.00");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cart:\n" + ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadCart();
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
                        
                        int cartId;
                        using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 CartId FROM dbo.Carts
WHERE CustomerId=@cid AND Status='Active'
ORDER BY CartId DESC;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@cid", _customerId);
                            object obj = cmd.ExecuteScalar();
                            if (obj == null)
                            {
                                using (SqlCommand ins = new SqlCommand(@"
INSERT INTO dbo.Carts (CustomerId, Status, CreatedAt)
VALUES (@cid, 'Active', GETDATE());
SELECT SCOPE_IDENTITY();", con, tx))
                                {
                                    ins.Parameters.AddWithValue("@cid", _customerId);
                                    cartId = Convert.ToInt32(ins.ExecuteScalar());
                                }
                            }
                            else cartId = Convert.ToInt32(obj);
                        }

                       
                        decimal price;
                        int stock;
                        using (SqlCommand cmd = new SqlCommand(@"
SELECT Price, Stock FROM dbo.Products
WHERE ProductId=@pid AND IsActive=1;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@pid", productId);
                            using (var r = cmd.ExecuteReader())
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

                        
                        using (SqlCommand cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM dbo.CartItems WHERE CartId=@cart AND ProductId=@pid)
BEGIN
    UPDATE dbo.CartItems
    SET Quantity=@q, UnitPrice=@price
    WHERE CartId=@cart AND ProductId=@pid;
END
ELSE
BEGIN
    INSERT INTO dbo.CartItems (CartId, ProductId, Quantity, UnitPrice)
    VALUES (@cart, @pid, @q, @price);
END
", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@cart", cartId);
                            cmd.Parameters.AddWithValue("@pid", productId);
                            cmd.Parameters.AddWithValue("@q", qty);
                            cmd.Parameters.AddWithValue("@price", price);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        MessageBox.Show("Cart updated.");
                        LoadCart();
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
                MessageBox.Show("Error updating cart:\n" + ex.Message);
            }
        }

        
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int productId;

           
            if (!int.TryParse(textBox2.Text.Trim(), out productId))
            {
                
                if (!int.TryParse(txtProductID.Text.Trim(), out productId))
                {
                    MessageBox.Show("Enter ProductId to remove (use Remove box or ProductId).");
                    return;
                }
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    con.Open();
                    int cartId = GetActiveCartId(con);
                    if (cartId == 0)
                    {
                        MessageBox.Show("No active cart found.");
                        return;
                    }

                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM dbo.CartItems WHERE CartId=@cart AND ProductId=@pid;", con))
                    {
                        cmd.Parameters.AddWithValue("@cart", cartId);
                        cmd.Parameters.AddWithValue("@pid", productId);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                        {
                            MessageBox.Show("Item not found in cart.");
                            return;
                        }
                    }
                }

                MessageBox.Show("Removed.");
                LoadCart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing item:\n" + ex.Message);
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
            textBox1.Clear();
            textBox2.Clear();
            LoadCart();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];
            txtProductID.Text = row.Cells["ProductId"].Value.ToString();
            txtName.Text = row.Cells["ProductName"].Value.ToString();
            txtPrice.Text = row.Cells["UnitPrice"].Value.ToString();
            txtQuantity.Text = row.Cells["Quantity"].Value.ToString();
        }

        private void Checkout_Click(object sender, EventArgs e)
        {
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

                       
                        DataTable items = new DataTable();
                        using (SqlCommand cmd = new SqlCommand(@"
SELECT ProductId, Quantity, UnitPrice
FROM dbo.CartItems
WHERE CartId=@cart;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@cart", cartId);
                            new SqlDataAdapter(cmd).Fill(items);
                        }

                        if (items.Rows.Count == 0)
                        {
                            tx.Rollback();
                            MessageBox.Show("Cart is empty. Add products first.");
                            return;
                        }

                       
                        decimal totalAmount = 0m;
                        foreach (DataRow r in items.Rows)
                        {
                            int qty = Convert.ToInt32(r["Quantity"]);
                            decimal price = Convert.ToDecimal(r["UnitPrice"]);
                            totalAmount += qty * price;
                        }

                       
                        int orderId = 0;
                        using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.Orders (CustomerId, OrderDate, Status, TotalAmount)
VALUES (@cid, GETDATE(), 'Pending', @total);
SELECT SCOPE_IDENTITY();", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@cid", _customerId);
                            cmd.Parameters.AddWithValue("@total", totalAmount);

                            orderId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                      
                        foreach (DataRow r in items.Rows)
                        {
                            int productId = Convert.ToInt32(r["ProductId"]);
                            int qty = Convert.ToInt32(r["Quantity"]);
                            decimal unitPrice = Convert.ToDecimal(r["UnitPrice"]);

                            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.OrderItems (OrderId, ProductId, Quantity, UnitPrice)
VALUES (@oid, @pid, @q, @p);", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@oid", orderId);
                                cmd.Parameters.AddWithValue("@pid", productId);
                                cmd.Parameters.AddWithValue("@q", qty);
                                cmd.Parameters.AddWithValue("@p", unitPrice);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        
                        using (SqlCommand cmd = new SqlCommand(@"
IF NOT EXISTS (SELECT 1 FROM dbo.Deliveries WHERE OrderId=@oid)
BEGIN
    INSERT INTO dbo.Deliveries (OrderId, DeliveryStatus, UpdatedAt)
    VALUES (@oid, 'Pending', GETDATE());
END", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@oid", orderId);
                            cmd.ExecuteNonQuery();
                        }

                      
                        using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.Carts
SET Status='CheckedOut'
WHERE CartId=@cart;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@cart", cartId);
                            cmd.ExecuteNonQuery();
                        }

                        
                        using (SqlCommand cmd = new SqlCommand(@"
DELETE FROM dbo.CartItems
WHERE CartId=@cart;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@cart", cartId);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();

                        MessageBox.Show($"Checkout successful!\nOrder ID: {orderId}\nTotal: {totalAmount:0.00}");

                        
                        LoadCart();
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
                MessageBox.Show("Checkout failed:\n" + ex.Message);
            }
        }

    }
}
