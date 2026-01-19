using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Form12 : Form
    {
        private int _userId;
        private int _customerId;

        public Form12() : this(0) { }

        public Form12(int userId)
        {
            InitializeComponent();
            _userId = userId;

            btnUpdate.Click += btnUpdate_Click;
            btnClear.Click += btnClear_Click;
            btnBack.Click += btnBack_Click;

            this.Load += Form12_Load;
        }

        private void Form12_Load(object sender, EventArgs e)
        {
            if (!TryLoadCustomerId())
            {
                MessageBox.Show("Customer profile not found for this user.");
                return;
            }

            LoadProfile();
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

        private void LoadProfile()
        {
            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string q = "SELECT CustomerId, CustomerName, Email, Phone FROM dbo.Customers WHERE CustomerId=@cid;";
                    SqlCommand cmd = new SqlCommand(q, con);
                    cmd.Parameters.AddWithValue("@cid", _customerId);

                    DataTable dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;

                    if (dt.Rows.Count > 0)
                    {
                        DataRow r = dt.Rows[0];
                        txtCustomerName.Text = r["CustomerName"].ToString();
                        txtProductName.Text = r["Email"] == DBNull.Value ? "" : r["Email"].ToString();
                        txtQuantity.Text = r["Phone"] == DBNull.Value ? "" : r["Phone"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading profile:\n" + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string name = txtCustomerName.Text.Trim();
            string email = txtProductName.Text.Trim();
            string phone = txtQuantity.Text.Trim();

            if (name == "")
            {
                MessageBox.Show("Customer Name is required.");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.Customers
SET CustomerName=@n, Email=@e, Phone=@p
WHERE CustomerId=@cid;", con))
                    {
                        cmd.Parameters.AddWithValue("@n", name);
                        cmd.Parameters.AddWithValue("@e", email == "" ? (object)DBNull.Value : email);
                        cmd.Parameters.AddWithValue("@p", phone == "" ? (object)DBNull.Value : phone);
                        cmd.Parameters.AddWithValue("@cid", _customerId);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Profile updated.");
                LoadProfile();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating profile:\n" + ex.Message);
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
            Form11 dash = new Form11(_userId);
            dash.Show();
            this.Close();
        }
    }
}
