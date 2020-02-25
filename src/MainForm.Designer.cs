namespace WolfNameCreator
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DrawColorCodesCheckBox = new System.Windows.Forms.CheckBox();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.BackColor = System.Drawing.SystemColors.Control;
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem,
            this.SaveToolStripMenuItem,
            this.SaveNewToolStripMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(356, 24);
            this.MenuStrip.TabIndex = 2;
            this.MenuStrip.Text = "MenuStrip";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.OpenToolStripMenuItem.Text = "Open";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.SaveToolStripMenuItem.Text = "Save";
            this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // SaveNewToolStripMenuItem
            // 
            this.SaveNewToolStripMenuItem.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.SaveNewToolStripMenuItem.Name = "SaveNewToolStripMenuItem";
            this.SaveNewToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.SaveNewToolStripMenuItem.Text = "Save New";
            this.SaveNewToolStripMenuItem.Click += new System.EventHandler(this.SaveNewToolStripMenuItem_Click);
            // 
            // DrawColorCodesCheckBox
            // 
            this.DrawColorCodesCheckBox.AutoSize = true;
            this.DrawColorCodesCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.DrawColorCodesCheckBox.Location = new System.Drawing.Point(173, 4);
            this.DrawColorCodesCheckBox.Name = "DrawColorCodesCheckBox";
            this.DrawColorCodesCheckBox.Size = new System.Drawing.Size(109, 17);
            this.DrawColorCodesCheckBox.TabIndex = 3;
            this.DrawColorCodesCheckBox.Text = "Draw color codes";
            this.DrawColorCodesCheckBox.UseVisualStyleBackColor = true;
            this.DrawColorCodesCheckBox.CheckedChanged += new System.EventHandler(this.DrawColorCodesCheckBox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 95);
            this.Controls.Add(this.DrawColorCodesCheckBox);
            this.Controls.Add(this.MenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Wolf Name Creator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveNewToolStripMenuItem;
        private System.Windows.Forms.CheckBox DrawColorCodesCheckBox;
    }
}