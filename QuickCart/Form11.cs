using System;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Form11 : Form
    {
        private int _userId;

        public Form11(int userId)
        {
            InitializeComponent();
            _userId = userId;
        }

        public Form11() : this(0) { }

        private void Form11_Load(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            Form12 profile = new Form12(_userId);
            profile.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form13 browsing = new Form13(_userId);
            browsing.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form14 cart = new Form14(_userId);
            cart.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Loginpage login = new Loginpage();
            login.Show();
            this.Close();
        }
    }
}
