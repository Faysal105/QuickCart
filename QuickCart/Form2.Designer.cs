namespace QuickCart
{
    partial class Form2
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnManageProduct = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnManageOrder = new System.Windows.Forms.Button();
            this.btnManageSeller = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(155, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(397, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Welcome to Admin Dashboard";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // btnManageProduct
            // 
            this.btnManageProduct.AccessibleName = "btnManageProduct";
            this.btnManageProduct.BackColor = System.Drawing.SystemColors.Window;
            this.btnManageProduct.Location = new System.Drawing.Point(189, 90);
            this.btnManageProduct.Name = "btnManageProduct";
            this.btnManageProduct.Size = new System.Drawing.Size(334, 54);
            this.btnManageProduct.TabIndex = 1;
            this.btnManageProduct.Text = "Manage Product";
            this.btnManageProduct.UseVisualStyleBackColor = false;
            this.btnManageProduct.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(189, 335);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(334, 55);
            this.button2.TabIndex = 2;
            this.button2.Text = "Manage Customer";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnManageOrder
            // 
            this.btnManageOrder.Location = new System.Drawing.Point(189, 168);
            this.btnManageOrder.Name = "btnManageOrder";
            this.btnManageOrder.Size = new System.Drawing.Size(334, 60);
            this.btnManageOrder.TabIndex = 3;
            this.btnManageOrder.Text = "Manage Order";
            this.btnManageOrder.UseVisualStyleBackColor = true;
            this.btnManageOrder.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnManageSeller
            // 
            this.btnManageSeller.Location = new System.Drawing.Point(189, 251);
            this.btnManageSeller.Name = "btnManageSeller";
            this.btnManageSeller.Size = new System.Drawing.Size(334, 59);
            this.btnManageSeller.TabIndex = 4;
            this.btnManageSeller.Text = "Manage Seller";
            this.btnManageSeller.UseVisualStyleBackColor = true;
            this.btnManageSeller.Click += new System.EventHandler(this.btnManageSeller_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(189, 412);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(334, 59);
            this.button5.TabIndex = 5;
            this.button5.Text = "Logout";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(723, 534);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.btnManageSeller);
            this.Controls.Add(this.btnManageOrder);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnManageProduct);
            this.Controls.Add(this.label1);
            this.Name = "Form2";
            this.Text = "Admin Dashboard";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnManageProduct;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnManageOrder;
        private System.Windows.Forms.Button btnManageSeller;
        private System.Windows.Forms.Button button5;
    }
}