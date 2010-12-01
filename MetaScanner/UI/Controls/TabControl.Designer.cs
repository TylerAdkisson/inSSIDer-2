namespace inSSIDer.UI.Controls
{
    partial class ETabControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cms1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveToExternalWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cms1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cms1
            // 
            this.cms1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveToExternalWindowToolStripMenuItem});
            this.cms1.Name = "cms1";
            this.cms1.Size = new System.Drawing.Size(193, 26);
            // 
            // moveToExternalWindowToolStripMenuItem
            // 
            this.moveToExternalWindowToolStripMenuItem.Name = "moveToExternalWindowToolStripMenuItem";
            this.moveToExternalWindowToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.moveToExternalWindowToolStripMenuItem.Text = "Move to external window";
            this.moveToExternalWindowToolStripMenuItem.Click += new System.EventHandler(this.moveToExternalWindowToolStripMenuItem_Click);
            // 
            // ETabControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Lime;
            this.Name = "ETabControl";
            this.Size = new System.Drawing.Size(300, 221);
            this.cms1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip cms1;
        private System.Windows.Forms.ToolStripMenuItem moveToExternalWindowToolStripMenuItem;
    }
}
