using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Registration : Form
    {
        public Registration()
        {
            InitializeComponent();

            // Safe wiring (Designer did not wire Register button)
            button1.Click += button1_Click;

            // Default role
            if (comboBox1.SelectedIndex < 0)
                comboBox1.SelectedIndex = 0; // Customer
        }
        private void Registration_Load(object sender, EventArgs e)
        {
            // Optional: set default role
            if (comboBox1.Items.Count > 0 && comboBox1.SelectedIndex < 0)
                comboBox1.SelectedIndex = 0;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // nothing required
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Map UI -> values (your control names are confusing, we follow designer labels)
            string fullName = txtProductID.Text.Trim(); // Name
            string email = txtName.Text.Trim();         // Email
            string phone = textBox1.Text.Trim();        // Phone
            string password = txtPrice.Text;            // Password
            string confirm = txtQuantity.Text;          // Confirm Password
            string role = comboBox1.Text.Trim();        // Customer or Seller

            // Basic validation
            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Name is required.");
                return;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Email is required.");
                return;
            }
            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Phone number is required.");
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Password is required.");
                return;
            }
            if (password != confirm)
            {
                MessageBox.Show("Password and Confirm Password do not match.");
                return;
            }
            if (role != "Customer" && role != "Seller")
            {
                MessageBox.Show("Please select a valid role (Customer/Seller).");
                return;
            }

            // Decide Username strategy:
            // Your login uses Users.Username. We'll use Email as Username (simple + unique)
            string username = email;

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    con.Open();
                    SqlTransaction tx = con.BeginTransaction();

                    try
                    {
                        // 1) Check if username/email already exists
                        using (SqlCommand chk = new SqlCommand(
                            "SELECT COUNT(*) FROM dbo.Users WHERE Username=@u", con, tx))
                        {
                            chk.Parameters.AddWithValue("@u", username);
                            int exists = Convert.ToInt32(chk.ExecuteScalar());
                            if (exists > 0)
                            {
                                tx.Rollback();
                                MessageBox.Show("This email/username is already registered.");
                                return;
                            }
                        }

                        // 2) Insert into Users and get new UserId
                        int newUserId;
                        using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.Users (Username, PasswordHash, UserType, IsActive, CreatedAt)
VALUES (@u, @p, @t, 1, GETDATE());
SELECT SCOPE_IDENTITY();", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@u", username);
                            cmd.Parameters.AddWithValue("@p", password); // plain for now (matches your login)
                            cmd.Parameters.AddWithValue("@t", role);

                            newUserId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // 3) Insert into role table
                        if (role == "Customer")
                        {
                            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.Customers (UserId, CustomerName, Email, Phone)
VALUES (@uid, @n, @e, @ph);", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@uid", newUserId);
                                cmd.Parameters.AddWithValue("@n", fullName);
                                cmd.Parameters.AddWithValue("@e", email);
                                cmd.Parameters.AddWithValue("@ph", phone);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else // Seller
                        {
                            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.Sellers (UserId, SellerName, Email, Phone)
VALUES (@uid, @n, @e, @ph);", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@uid", newUserId);
                                cmd.Parameters.AddWithValue("@n", fullName);
                                cmd.Parameters.AddWithValue("@e", email);
                                cmd.Parameters.AddWithValue("@ph", phone);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();

                        MessageBox.Show("Registration successful. You can login now.");

                        // Go back to login
                        Loginpage login = new Loginpage();
                        login.Show();
                        this.Close();
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
                MessageBox.Show("Registration failed:\n" + ex.Message);
            }
        }
    }
}
