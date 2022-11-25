using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSVUtility.Core;
using CSVUtility.Core.Helpers;
using log4net.Config;

namespace CSVUtilityUI
{
    public partial class MainForm : Form
    {
        private readonly UtilityEngine engine;

        public MainForm()
        {
            InitializeComponent();
            XmlConfigurator.Configure();
            var outputHandler = new OutputHandler();
            this.engine = new UtilityEngine(outputHandler);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = @"Csv files (*.csv)|*.csv",
                Title = @"Open Csv file"
            };

            fileDialog.FileOk += FileDialog_FileOk;

            var dialogResult = fileDialog.ShowDialog();
        }

        private void FileDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (!(sender is OpenFileDialog openFileDialog)) return;
            txtFilePath.Text = openFileDialog.FileName;
            engine.CsvFilePath = openFileDialog.FileName;
            btnProcess.Enabled = true;
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            statusLabel.Text = @"In-progress...";
            engine?.Action();
            statusLabel.Text = @"Process Completed.";
        }
    }
}
