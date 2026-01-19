namespace QuickCart
{
    partial class Form8
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button5 = new System.Windows.Forms.Button();
            this.btnManageP = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.btnManageProduct = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(157, 272);
            this.button5.Margin = new System.Windows.Forms.Padding(2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(223, 36);
            this.button5.TabIndex = 11;
            this.button5.Text = "Logout";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // btnManageP
            // 
            this.btnManageP.Location = new System.Drawing.Point(189, 181);
            this.btnManageP.Margin = new System.Windows.Forms.Padding(2);
            this.btnManageP.Name = "btnManageP";
            this.btnManageP.Size = new System.Drawing.Size(169, 37);
            this.btnManageP.TabIndex = 10;
            this.btnManageP.Text = "Profile Managment";
            this.btnManageP.UseVisualStyleBackColor = true;
            this.btnManageP.Click += new System.EventHandler(this.btnManageSeller_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(189, 147);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(169, 30);
            this.button3.TabIndex = 9;
            this.button3.Text = "Shipping And  Delivery";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // btnManageProduct
            // 
            this.btnManageProduct.AccessibleName = "btnManageProduct";
            this.btnManageProduct.BackColor = System.Drawing.SystemColors.Window;
            this.btnManageProduct.Location = new System.Drawing.Point(189, 113);
            this.btnManageProduct.Margin = new System.Windows.Forms.Padding(2);
            this.btnManageProduct.Name = "btnManageProduct";
            this.btnManageProduct.Size = new System.Drawing.Size(169, 30);
            this.btnManageProduct.TabIndex = 7;
            this.btnManageProduct.Text = "Manage Product";
            this.btnManageProduct.UseVisualStyleBackColor = false;
            this.btnManageProduct.Click += new System.EventHandler(this.btnManageProduct_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(139, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(261, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "Welcome to Seller Dashboard";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(189, 222);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(169, 37);
            this.button1.TabIndex = 12;
            this.button1.Text = "Payment and Earnings ";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form8
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(539, 319);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.btnManageP);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnManageProduct);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form8";
            this.Text = "Seller Dashboard";
            this.Load += new System.EventHandler(this.Form8_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button btnManageP;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnManageProduct;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}