namespace Loader
{
    partial class Form1
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
            Login = new Button();
            keyBox = new TextBox();
            ALenda = new TextBox();
            SuspendLayout();
            // 
            // Login
            // 
            Login.Location = new Point(135, 144);
            Login.Name = "Login";
            Login.Size = new Size(75, 23);
            Login.TabIndex = 0;
            Login.Text = "Login";
            Login.UseVisualStyleBackColor = true;
            Login.Click += button1_Click;
            // 
            // keyBox
            // 
            keyBox.BorderStyle = BorderStyle.FixedSingle;
            keyBox.Location = new Point(3, 77);
            keyBox.Name = "keyBox";
            keyBox.Size = new Size(348, 23);
            keyBox.TabIndex = 1;
            keyBox.Text = "License key";
            keyBox.TextAlign = HorizontalAlignment.Center;
            keyBox.TextChanged += keyBox_TextChanged;
            // 
            // ALenda
            // 
            ALenda.BorderStyle = BorderStyle.None;
            ALenda.Enabled = false;
            ALenda.Font = new Font("Segoe UI", 15F);
            ALenda.Location = new Point(125, 12);
            ALenda.Name = "ALenda";
            ALenda.ReadOnly = true;
            ALenda.Size = new Size(100, 27);
            ALenda.TabIndex = 0;
            ALenda.Text = "A Lenda";
            ALenda.TextAlign = HorizontalAlignment.Center;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(353, 179);
            Controls.Add(ALenda);
            Controls.Add(keyBox);
            Controls.Add(Login);
            Name = "Form1";
            Text = "A Lenda";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Login;
        private TextBox keyBox;
        private TextBox ALenda;
    }
}