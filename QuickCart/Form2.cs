using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickCart
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

       
        private void Form2_Load(object sender, EventArgs e)
        {
         
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 manageProduct = new Form3();
            manageProduct.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Loginpage login = new Loginpage();
            login.Show();
            this.Hide();
        }

        private void btnManageSeller_Click(object sender, EventArgs e)
        {
            Form4 manageSeller = new Form4();
            manageSeller.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form5 manageOrder = new Form5();
            manageOrder.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form6 manageCustomer = new Form6();
            manageCustomer.Show();
            this.Hide();
        }
    }
}
