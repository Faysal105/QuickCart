using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class seller_profile_management : Form
    {
        private int _userId = 0;
        private int _sellerId = 0;

        public seller_profile_management()
        {
            InitializeComponent();
            btnUpdate.Click += btnUpdate_Click;
            btnClear.Click += btnClear_Click;
            btnBack.Click += btnBack_Click;
        }

       
        public seller_profile_management(int userId) : this()
        {
            _userId = userId;
        }

        private void seller_profile_management_Load(object sender, EventArgs e)
        {
          
            if (_userId <= 0)
            {
                
                MessageBox.Show("Seller UserId not provided. Open like: new seller_profile_management(userId)");
                return;
            }

            LoadSellerProfile();
        }

        private void LoadSellerProfile()
        {
            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    con.Open();

                   
                    using (SqlCommand cmd1 = new SqlCommand(
                        "SELECT TOP 1 SellerId FROM dbo.Sellers WHERE UserId=@uid", con))
                    {
                        cmd1.Parameters.AddWithValue("@uid", _userId);

                        object obj = cmd1.ExecuteScalar();
                        if (obj == null)
                        {
                            _sellerId = 0;
                            dataGridView1.DataSource = null;

                            txtCustomerName.Clear();
                            txtProductName.Clear();
                            txtQuantity.Clear();

                            MessageBox.Show("No seller profile found for this user.");
                            return;
                        }

                        _sellerId = Convert.ToInt32(obj);
                    }

                  
                    using (SqlCommand cmd2 = new SqlCommand(@"
SELECT SellerId, SellerName, Email, Phone
FROM dbo.Sellers
WHERE SellerId=@sid;", con))
                    {
                        cmd2.Parameters.AddWithValue("@sid", _sellerId);

                        DataTable dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd2))
                        {
                            da.Fill(dt);
                        }

                        dataGridView1.AutoGenerateColumns = true;
                        dataGridView1.DataSource = dt;

                        if (dt.Rows.Count > 0)
                        {
                            DataRow r = dt.Rows[0];
                            
                            txtCustomerName.Text = r["SellerName"].ToString();
                            txtProductName.Text = r["Email"] == DBNull.Value ? "" : r["Email"].ToString();
                            txtQuantity.Text = r["Phone"] == DBNull.Value ? "" : r["Phone"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LoadSellerProfile Error:\n" + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_userId <= 0 || _sellerId <= 0)
            {
                MessageBox.Show("Seller not loaded. Open this form with a valid userId.");
                return;
            }

            string sellerName = txtCustomerName.Text.Trim();
            string email = txtProductName.Text.Trim();
            string phone = txtQuantity.Text.Trim();

            if (sellerName == "")
            {
                MessageBox.Show("Seller Name is required.");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.Sellers
SET SellerName=@name,
    Email=@email,
    Phone=@phone
WHERE SellerId=@sid;", con))
                    {
                        cmd.Parameters.AddWithValue("@name", sellerName);
                        cmd.Parameters.AddWithValue("@email", email == "" ? (object)DBNull.Value : email);
                        cmd.Parameters.AddWithValue("@phone", phone == "" ? (object)DBNull.Value : phone);
                        cmd.Parameters.AddWithValue("@sid", _sellerId);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                        {
                            MessageBox.Show("Update failed. No rows affected.");
                            return;
                        }
                    }
                }

                MessageBox.Show("Profile updated successfully.");
                LoadSellerProfile();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update Error:\n" + ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtCustomerName.Clear();
            txtProductName.Clear();
            txtQuantity.Clear();
            txtCustomerName.Focus();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Form8 dash = new Form8(_userId);
            dash.Show();
            this.Hide();
        }
    }
}
