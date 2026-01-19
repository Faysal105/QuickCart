using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string query = "SELECT ProductId, ProductName, Price, Stock FROM dbo.Products";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products:\n" + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            decimal price;
            int stock;

            if (name == "" || txtPrice.Text.Trim() == "" || txtQuantity.Text.Trim() == "")
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            if (!decimal.TryParse(txtPrice.Text.Trim(), out price))
            {
                MessageBox.Show("Invalid price");
                return;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out stock))
            {
                MessageBox.Show("Invalid stock quantity");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                   
                    string query = "INSERT INTO dbo.Products (ProductName, Price, Stock, IsActive) VALUES (@name, @price, @stock, 1)";
                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@stock", stock);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Product Added Successfully");
                LoadProducts();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product:\n" + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int productId;
            decimal price;
            int stock;

            if (txtProductID.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Product ID");
                return;
            }

            if (!int.TryParse(txtProductID.Text.Trim(), out productId))
            {
                MessageBox.Show("Invalid Product ID");
                return;
            }

            if (txtName.Text.Trim() == "" || txtPrice.Text.Trim() == "" || txtQuantity.Text.Trim() == "")
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            if (!decimal.TryParse(txtPrice.Text.Trim(), out price))
            {
                MessageBox.Show("Invalid price");
                return;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out stock))
            {
                MessageBox.Show("Invalid stock quantity");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string query = "UPDATE dbo.Products SET ProductName=@name, Price=@price, Stock=@stock WHERE ProductId=@id";
                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.Parameters.AddWithValue("@id", productId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Product Updated Successfully");
                LoadProducts();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating product:\n" + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int productId;

            if (txtProductID.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Product ID");
                return;
            }

            if (!int.TryParse(txtProductID.Text.Trim(), out productId))
            {
                MessageBox.Show("Invalid Product ID");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string query = "DELETE FROM dbo.Products WHERE ProductId=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", productId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Product Deleted Successfully");
                LoadProducts();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting product:\n" + ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            int productId;

            if (txtProductID.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Product ID");
                return;
            }

            if (!int.TryParse(txtProductID.Text.Trim(), out productId))
            {
                MessageBox.Show("Invalid Product ID");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string query = "SELECT ProductId, ProductName, Price, Stock FROM dbo.Products WHERE ProductId=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", productId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching product:\n" + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtProductID.Text = row.Cells["ProductId"].Value.ToString();
                txtName.Text = row.Cells["ProductName"].Value.ToString();
                txtPrice.Text = row.Cells["Price"].Value.ToString();
                txtQuantity.Text = row.Cells["Stock"].Value.ToString();
            }
        }

        private void ClearFields()
        {
            txtProductID.Clear();
            txtName.Clear();
            txtPrice.Clear();
            txtQuantity.Clear();
            txtName.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            LoadProducts();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form2 dashboard = new Form2();
            dashboard.Show();
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}
