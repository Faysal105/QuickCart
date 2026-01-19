using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            LoadSellers();
        }

        private void LoadSellers()
        {
            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string query = @"
SELECT 
    s.SellerId,
    s.SellerName,
    s.Email,
    s.Phone,
    u.Username
FROM dbo.Sellers s
INNER JOIN dbo.Users u ON u.UserId = s.UserId
ORDER BY s.SellerId DESC;";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sellers:\n" + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (name == "")
            {
                MessageBox.Show("Please enter seller name");
                return;
            }

            string password = "1234";

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    con.Open();

                    SqlTransaction tr = con.BeginTransaction();

                    try
                    {
                        string baseUsername = name.Replace(" ", "").ToLower();
                        string username = baseUsername;

                        int count = 1;
                        while (true)
                        {
                            SqlCommand check = new SqlCommand("SELECT COUNT(*) FROM dbo.Users WHERE Username=@u", con, tr);
                            check.Parameters.AddWithValue("@u", username);

                            int exists = Convert.ToInt32(check.ExecuteScalar());
                            if (exists == 0) break;

                            username = baseUsername + count;
                            count++;
                        }

                        int newUserId;

                        string q1 = @"
INSERT INTO dbo.Users (Username, PasswordHash, UserType, IsActive, CreatedAt)
VALUES (@uname, @pass, 'Seller', 1, GETDATE());
SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmd1 = new SqlCommand(q1, con, tr))
                        {
                            cmd1.Parameters.AddWithValue("@uname", username);
                            cmd1.Parameters.AddWithValue("@pass", password);
                            newUserId = Convert.ToInt32(cmd1.ExecuteScalar());
                        }

                        string q2 = @"
INSERT INTO dbo.Sellers (UserId, SellerName, Email, Phone)
VALUES (@uid, @name, @email, @phone);";

                        using (SqlCommand cmd2 = new SqlCommand(q2, con, tr))
                        {
                            cmd2.Parameters.AddWithValue("@uid", newUserId);
                            cmd2.Parameters.AddWithValue("@name", name);

                            if (email == "") cmd2.Parameters.AddWithValue("@email", DBNull.Value);
                            else cmd2.Parameters.AddWithValue("@email", email);

                            if (phone == "") cmd2.Parameters.AddWithValue("@phone", DBNull.Value);
                            else cmd2.Parameters.AddWithValue("@phone", phone);

                            cmd2.ExecuteNonQuery();
                        }

                        tr.Commit();

                        MessageBox.Show("Seller added successfully\nUsername: " + username + "\nPassword: 1234");
                        LoadSellers();
                        ClearFields();
                    }
                    catch (Exception ex2)
                    {
                        tr.Rollback();
                        MessageBox.Show("Error adding seller:\n" + ex2.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding seller:\n" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (txtID.Text.Trim() == "")
            {
                MessageBox.Show("Please select a seller");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string query = "UPDATE dbo.Sellers SET SellerName=@name, Email=@email, Phone=@phone WHERE SellerId=@id";
                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());

                    if (txtEmail.Text.Trim() == "") cmd.Parameters.AddWithValue("@email", DBNull.Value);
                    else cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());

                    if (txtPhone.Text.Trim() == "") cmd.Parameters.AddWithValue("@phone", DBNull.Value);
                    else cmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());

                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtID.Text.Trim()));

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Seller updated successfully");
                LoadSellers();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating seller:\n" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (txtID.Text.Trim() == "")
            {
                MessageBox.Show("Please select a seller");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    con.Open();

                    int sellerId = Convert.ToInt32(txtID.Text.Trim());

                    int userId = 0;
                    using (SqlCommand cmdGet = new SqlCommand("SELECT UserId FROM dbo.Sellers WHERE SellerId=@id", con))
                    {
                        cmdGet.Parameters.AddWithValue("@id", sellerId);
                        object obj = cmdGet.ExecuteScalar();
                        if (obj != null) userId = Convert.ToInt32(obj);
                    }

                    using (SqlCommand cmd1 = new SqlCommand("DELETE FROM dbo.Sellers WHERE SellerId=@id", con))
                    {
                        cmd1.Parameters.AddWithValue("@id", sellerId);
                        cmd1.ExecuteNonQuery();
                    }

                    if (userId > 0)
                    {
                        using (SqlCommand cmd2 = new SqlCommand("DELETE FROM dbo.Users WHERE UserId=@uid", con))
                        {
                            cmd2.Parameters.AddWithValue("@uid", userId);
                            cmd2.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Seller deleted successfully");
                LoadSellers();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting seller:\n" + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (txtID.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Seller ID");
                return;
            }

            try
            {
                using (SqlConnection con = DataAccess.GetConnection())
                {
                    string query = @"
SELECT 
    s.SellerId,
    s.SellerName,
    s.Email,
    s.Phone,
    u.Username
FROM dbo.Sellers s
INNER JOIN dbo.Users u ON u.UserId = s.UserId
WHERE s.SellerId=@id;";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtID.Text.Trim()));

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching seller:\n" + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtID.Text = row.Cells["SellerId"].Value.ToString();
                txtName.Text = row.Cells["SellerName"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value == DBNull.Value ? "" : row.Cells["Email"].Value.ToString();
                txtPhone.Text = row.Cells["Phone"].Value == DBNull.Value ? "" : row.Cells["Phone"].Value.ToString();
            }
        }

        private void ClearFields()
        {
            txtID.Clear();
            txtName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtName.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            LoadSellers();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form2 dashboard = new Form2();
            dashboard.Show();
            this.Hide();
        }
    }
}
