using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Loginpage : Form
    {
        public Loginpage()
        {
            InitializeComponent();
        }

        private void Loginpage_Load(object sender, EventArgs e)
        {
            // Optional: make sure combo has a valid default
            if (userType.Items.Count > 0 && userType.SelectedIndex < 0)
                userType.SelectedIndex = 0;
        }

        private void login_Click(object sender, EventArgs e)
        {
            string username = txtusername.Text.Trim();
            string password = txtpassword.Text.Trim();

            if (username == "" || password == "")
            {
                MessageBox.Show("Please enter username and password");
                return;
            }

            // Normalize selected user type from ComboBox (it has "Admin " with trailing space)
            string selectedType = (userType.Text ?? "").Trim().ToLower();

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    con.Open();

                    string query = @"
SELECT UserId, UserType, IsActive
FROM dbo.Users
WHERE Username = @username AND PasswordHash = @password;";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read())
                            {
                                MessageBox.Show("Invalid Username or Password", "Login Failed");
                                return;
                            }

                            int userId = Convert.ToInt32(dr["UserId"]);
                            string dbType = (dr["UserType"]?.ToString() ?? "").Trim().ToLower();
                            bool isActive = dr["IsActive"] != DBNull.Value && Convert.ToBoolean(dr["IsActive"]);

                            if (!isActive)
                            {
                                MessageBox.Show("This account is disabled. Contact admin.");
                                return;
                            }

                         
                            if (!string.IsNullOrWhiteSpace(selectedType) && selectedType != dbType)
                            {
                                MessageBox.Show($"UserType mismatch!\nSelected: {selectedType}\nAccount Type: {dbType}");
                                return;
                            }

                            MessageBox.Show("Login Successful", "Success");

                            if (dbType == "admin")
                            {
                                Form2 adminDashboard = new Form2();
                                adminDashboard.Show();
                            }
                            else if (dbType == "seller")
                            {
                                Form8 sellerDashboard = new Form8(userId);
                                sellerDashboard.Show();
                            }
                            else if (dbType == "customer")
                            {
                                Form11 customerDashboard = new Form11(userId);
                                customerDashboard.Show();
                            }
                            else
                            {
                                MessageBox.Show("User type not recognized: " + dbType);
                                return;
                            }

                            this.Hide();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
        }

        private void txtusername_TextChanged(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void userType_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}
