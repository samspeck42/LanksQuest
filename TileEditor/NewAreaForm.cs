using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TileEditor
{
    public partial class NewAreaForm : Form
    {
        public bool OKPressed = false;

        public NewAreaForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            OKPressed = true;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            OKPressed = false;
            Close();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                contentPathTextBox.Text = folderBrowserDialog1.SelectedPath;
        }
    }
}
