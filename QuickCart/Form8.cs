using System;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Form8 : Form
    {
        private int _userId;

        public Form8() : this(0) { }

        public Form8(int userId)
        {
            InitializeComponent();
            _userId = userId;

            
            button3.Click += button3_Click; 
        }

        private void btnManageProduct_Click(object sender, EventArgs e)
        {
            Form7 manageProduct = new Form7(_userId);
            manageProduct.Show();
            this.Hide();
        }

        private void btnManageSeller_Click(object sender, EventArgs e)
        {
            seller_profile_management profile = new seller_profile_management(_userId);
            profile.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form9 shipping = new Form9(_userId);
            shipping.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form10 payment = new Form10(_userId);
            payment.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Loginpage login = new Loginpage();
            login.Show();
            this.Close();
        }

        private void Form8_Load(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
    }
}
