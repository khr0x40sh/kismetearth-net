namespace KismetEarth.NET
{
    partial class Pref_Form
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
            this.tabcontrol = new System.Windows.Forms.TabControl();
            this.AppearanceTab = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.ConnectionsTab = new System.Windows.Forms.TabPage();
            this.DebugTab = new System.Windows.Forms.TabPage();
            this.tabcontrol.SuspendLayout();
            this.AppearanceTab.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabcontrol
            // 
            this.tabcontrol.Controls.Add(this.AppearanceTab);
            this.tabcontrol.Controls.Add(this.ConnectionsTab);
            this.tabcontrol.Controls.Add(this.DebugTab);
            this.tabcontrol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabcontrol.Enabled = false;
            this.tabcontrol.Location = new System.Drawing.Point(0, 0);
            this.tabcontrol.Name = "tabcontrol";
            this.tabcontrol.SelectedIndex = 0;
            this.tabcontrol.Size = new System.Drawing.Size(483, 435);
            this.tabcontrol.TabIndex = 0;
            // 
            // AppearanceTab
            // 
            this.AppearanceTab.Controls.Add(this.splitContainer1);
            this.AppearanceTab.Location = new System.Drawing.Point(4, 25);
            this.AppearanceTab.Name = "AppearanceTab";
            this.AppearanceTab.Padding = new System.Windows.Forms.Padding(3);
            this.AppearanceTab.Size = new System.Drawing.Size(475, 406);
            this.AppearanceTab.TabIndex = 0;
            this.AppearanceTab.Text = "Appearance";
            this.AppearanceTab.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBox1);
            this.splitContainer1.Size = new System.Drawing.Size(469, 400);
            this.splitContainer1.SplitterDistance = 156;
            this.splitContainer1.TabIndex = 0;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(156, 388);
            this.listBox1.TabIndex = 0;
            // 
            // ConnectionsTab
            // 
            this.ConnectionsTab.Location = new System.Drawing.Point(4, 25);
            this.ConnectionsTab.Name = "ConnectionsTab";
            this.ConnectionsTab.Padding = new System.Windows.Forms.Padding(3);
            this.ConnectionsTab.Size = new System.Drawing.Size(475, 406);
            this.ConnectionsTab.TabIndex = 1;
            this.ConnectionsTab.Text = "Connections";
            this.ConnectionsTab.UseVisualStyleBackColor = true;
            // 
            // DebugTab
            // 
            this.DebugTab.Location = new System.Drawing.Point(4, 25);
            this.DebugTab.Name = "DebugTab";
            this.DebugTab.Padding = new System.Windows.Forms.Padding(3);
            this.DebugTab.Size = new System.Drawing.Size(475, 406);
            this.DebugTab.TabIndex = 2;
            this.DebugTab.Text = "Debug";
            this.DebugTab.UseVisualStyleBackColor = true;
            // 
            // Pref_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 435);
            this.Controls.Add(this.tabcontrol);
            this.Name = "Pref_Form";
            this.Text = "Pref_Form";
            this.tabcontrol.ResumeLayout(false);
            this.AppearanceTab.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabcontrol;
        private System.Windows.Forms.TabPage AppearanceTab;
        private System.Windows.Forms.TabPage ConnectionsTab;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TabPage DebugTab;

    }
}