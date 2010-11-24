namespace inSSIDer.UI.Forms
{
    partial class frmTest
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
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl2 = new inSSIDer.UI.Controls.ETabControl();
            this.tabControl1 = new inSSIDer.UI.Controls.ETabControl();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 200);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.AllowDrop = true;
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.BackColor = System.Drawing.Color.Black;
            this.tabControl2.ForeColor = System.Drawing.Color.Lime;
            this.tabControl2.Location = new System.Drawing.Point(12, 249);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.Size = new System.Drawing.Size(685, 227);
            this.tabControl2.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.AllowDrop = true;
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.BackColor = System.Drawing.Color.Black;
            this.tabControl1.ForeColor = System.Drawing.Color.Lime;
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Size = new System.Drawing.Size(685, 182);
            this.tabControl1.TabIndex = 0;
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 488);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.tabControl1);
            this.Name = "frmTest";
            this.Text = "frmTest";
            this.ResumeLayout(false);

        }

        #endregion

        private inSSIDer.UI.Controls.ETabControl tabControl1;
        private inSSIDer.UI.Controls.ETabControl tabControl2;
        private System.Windows.Forms.Button button1;


    }
}