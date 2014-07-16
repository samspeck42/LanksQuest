namespace TileEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tileTextureListBox = new System.Windows.Forms.ListBox();
            this.tileTexturePreviewBox = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tileLayoutTab = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.eraseButton = new System.Windows.Forms.RadioButton();
            this.drawButton = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.editForegroundButton = new System.Windows.Forms.RadioButton();
            this.editBackgroundButton = new System.Windows.Forms.RadioButton();
            this.collisionLayoutTab = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.removeTileCollisionButton = new System.Windows.Forms.RadioButton();
            this.addTileCollisionButton = new System.Windows.Forms.RadioButton();
            this.tileCollisionListBox = new System.Windows.Forms.ListBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.fillCellsCheckBox = new System.Windows.Forms.CheckBox();
            this.tileDisplay1 = new TileEditor.TileDisplay();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tileTexturePreviewBox)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tileLayoutTab.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.collisionLayoutTab.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(820, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Location = new System.Drawing.Point(591, 27);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(17, 480);
            this.vScrollBar1.TabIndex = 2;
            this.vScrollBar1.Visible = false;
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Location = new System.Drawing.Point(12, 510);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(576, 17);
            this.hScrollBar1.TabIndex = 3;
            this.hScrollBar1.Visible = false;
            // 
            // tileTextureListBox
            // 
            this.tileTextureListBox.FormattingEnabled = true;
            this.tileTextureListBox.Location = new System.Drawing.Point(6, 120);
            this.tileTextureListBox.Name = "tileTextureListBox";
            this.tileTextureListBox.Size = new System.Drawing.Size(189, 186);
            this.tileTextureListBox.TabIndex = 4;
            this.tileTextureListBox.SelectedIndexChanged += new System.EventHandler(this.tileTextureListBox_SelectedIndexChanged);
            // 
            // tileTexturePreviewBox
            // 
            this.tileTexturePreviewBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tileTexturePreviewBox.Location = new System.Drawing.Point(6, 307);
            this.tileTexturePreviewBox.Name = "tileTexturePreviewBox";
            this.tileTexturePreviewBox.Size = new System.Drawing.Size(160, 160);
            this.tileTexturePreviewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.tileTexturePreviewBox.TabIndex = 5;
            this.tileTexturePreviewBox.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tileLayoutTab);
            this.tabControl1.Controls.Add(this.collisionLayoutTab);
            this.tabControl1.Location = new System.Drawing.Point(611, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(209, 500);
            this.tabControl1.TabIndex = 6;
            // 
            // tileLayoutTab
            // 
            this.tileLayoutTab.BackColor = System.Drawing.Color.Transparent;
            this.tileLayoutTab.Controls.Add(this.panel2);
            this.tileLayoutTab.Controls.Add(this.panel1);
            this.tileLayoutTab.Controls.Add(this.tileTextureListBox);
            this.tileLayoutTab.Controls.Add(this.tileTexturePreviewBox);
            this.tileLayoutTab.Location = new System.Drawing.Point(4, 22);
            this.tileLayoutTab.Name = "tileLayoutTab";
            this.tileLayoutTab.Padding = new System.Windows.Forms.Padding(3);
            this.tileLayoutTab.Size = new System.Drawing.Size(201, 474);
            this.tileLayoutTab.TabIndex = 0;
            this.tileLayoutTab.Text = "Tile Layout";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.fillCellsCheckBox);
            this.panel2.Controls.Add(this.eraseButton);
            this.panel2.Controls.Add(this.drawButton);
            this.panel2.Location = new System.Drawing.Point(6, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(189, 54);
            this.panel2.TabIndex = 9;
            // 
            // eraseButton
            // 
            this.eraseButton.AutoSize = true;
            this.eraseButton.Location = new System.Drawing.Point(60, 3);
            this.eraseButton.Name = "eraseButton";
            this.eraseButton.Size = new System.Drawing.Size(52, 17);
            this.eraseButton.TabIndex = 1;
            this.eraseButton.Text = "Erase";
            this.eraseButton.UseVisualStyleBackColor = true;
            // 
            // drawButton
            // 
            this.drawButton.AutoSize = true;
            this.drawButton.Checked = true;
            this.drawButton.Location = new System.Drawing.Point(4, 3);
            this.drawButton.Name = "drawButton";
            this.drawButton.Size = new System.Drawing.Size(50, 17);
            this.drawButton.TabIndex = 0;
            this.drawButton.TabStop = true;
            this.drawButton.Text = "Draw";
            this.drawButton.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.editForegroundButton);
            this.panel1.Controls.Add(this.editBackgroundButton);
            this.panel1.Location = new System.Drawing.Point(6, 66);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(189, 48);
            this.panel1.TabIndex = 8;
            // 
            // editForegroundButton
            // 
            this.editForegroundButton.AutoSize = true;
            this.editForegroundButton.Location = new System.Drawing.Point(3, 3);
            this.editForegroundButton.Name = "editForegroundButton";
            this.editForegroundButton.Size = new System.Drawing.Size(100, 17);
            this.editForegroundButton.TabIndex = 6;
            this.editForegroundButton.Text = "Edit Foreground";
            this.editForegroundButton.UseVisualStyleBackColor = true;
            // 
            // editBackgroundButton
            // 
            this.editBackgroundButton.AutoSize = true;
            this.editBackgroundButton.Checked = true;
            this.editBackgroundButton.Location = new System.Drawing.Point(3, 26);
            this.editBackgroundButton.Name = "editBackgroundButton";
            this.editBackgroundButton.Size = new System.Drawing.Size(104, 17);
            this.editBackgroundButton.TabIndex = 7;
            this.editBackgroundButton.TabStop = true;
            this.editBackgroundButton.Text = "Edit Background";
            this.editBackgroundButton.UseVisualStyleBackColor = true;
            // 
            // collisionLayoutTab
            // 
            this.collisionLayoutTab.BackColor = System.Drawing.Color.Transparent;
            this.collisionLayoutTab.Controls.Add(this.panel3);
            this.collisionLayoutTab.Controls.Add(this.tileCollisionListBox);
            this.collisionLayoutTab.Location = new System.Drawing.Point(4, 22);
            this.collisionLayoutTab.Name = "collisionLayoutTab";
            this.collisionLayoutTab.Padding = new System.Windows.Forms.Padding(3);
            this.collisionLayoutTab.Size = new System.Drawing.Size(201, 474);
            this.collisionLayoutTab.TabIndex = 1;
            this.collisionLayoutTab.Text = "Collision Layout";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.removeTileCollisionButton);
            this.panel3.Controls.Add(this.addTileCollisionButton);
            this.panel3.Location = new System.Drawing.Point(6, 6);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(189, 27);
            this.panel3.TabIndex = 1;
            // 
            // removeTileCollisionButton
            // 
            this.removeTileCollisionButton.AutoSize = true;
            this.removeTileCollisionButton.Location = new System.Drawing.Point(53, 3);
            this.removeTileCollisionButton.Name = "removeTileCollisionButton";
            this.removeTileCollisionButton.Size = new System.Drawing.Size(65, 17);
            this.removeTileCollisionButton.TabIndex = 1;
            this.removeTileCollisionButton.Text = "Remove";
            this.removeTileCollisionButton.UseVisualStyleBackColor = true;
            // 
            // addTileCollisionButton
            // 
            this.addTileCollisionButton.AutoSize = true;
            this.addTileCollisionButton.Checked = true;
            this.addTileCollisionButton.Location = new System.Drawing.Point(3, 3);
            this.addTileCollisionButton.Name = "addTileCollisionButton";
            this.addTileCollisionButton.Size = new System.Drawing.Size(44, 17);
            this.addTileCollisionButton.TabIndex = 0;
            this.addTileCollisionButton.TabStop = true;
            this.addTileCollisionButton.Text = "Add";
            this.addTileCollisionButton.UseVisualStyleBackColor = true;
            // 
            // tileCollisionListBox
            // 
            this.tileCollisionListBox.FormattingEnabled = true;
            this.tileCollisionListBox.Location = new System.Drawing.Point(6, 39);
            this.tileCollisionListBox.Name = "tileCollisionListBox";
            this.tileCollisionListBox.Size = new System.Drawing.Size(189, 147);
            this.tileCollisionListBox.TabIndex = 0;
            this.tileCollisionListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tileCollisionListBox_DrawItem);
            // 
            // fillCellsCheckBox
            // 
            this.fillCellsCheckBox.AutoSize = true;
            this.fillCellsCheckBox.Location = new System.Drawing.Point(4, 27);
            this.fillCellsCheckBox.Name = "fillCellsCheckBox";
            this.fillCellsCheckBox.Size = new System.Drawing.Size(63, 17);
            this.fillCellsCheckBox.TabIndex = 2;
            this.fillCellsCheckBox.Text = "Fill Cells";
            this.fillCellsCheckBox.UseVisualStyleBackColor = true;
            // 
            // tileDisplay1
            // 
            this.tileDisplay1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tileDisplay1.Location = new System.Drawing.Point(12, 27);
            this.tileDisplay1.Name = "tileDisplay1";
            this.tileDisplay1.Size = new System.Drawing.Size(576, 480);
            this.tileDisplay1.TabIndex = 0;
            this.tileDisplay1.Text = "tileDisplay1";
            this.tileDisplay1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tileDisplay1_MouseClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 528);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.tileDisplay1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tileTexturePreviewBox)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tileLayoutTab.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.collisionLayoutTab.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TileDisplay tileDisplay1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ListBox tileTextureListBox;
        private System.Windows.Forms.PictureBox tileTexturePreviewBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tileLayoutTab;
        private System.Windows.Forms.TabPage collisionLayoutTab;
        private System.Windows.Forms.RadioButton editBackgroundButton;
        private System.Windows.Forms.RadioButton editForegroundButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton drawButton;
        private System.Windows.Forms.RadioButton eraseButton;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ListBox tileCollisionListBox;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton removeTileCollisionButton;
        private System.Windows.Forms.RadioButton addTileCollisionButton;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.CheckBox fillCellsCheckBox;
    }
}

