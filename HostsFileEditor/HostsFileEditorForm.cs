using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace HostsFileEditor {
    
    /// <summary>
    /// Main and only form of the Hosts file editor app
    /// <param name="author">Ionut Voda ionut.voda@zitec.ro</param>
    /// </summary>
    public partial class HostsFileEditorForm : Form {

        protected string pathToHostsFile = Environment.GetEnvironmentVariable("windir") + "\\System32\\drivers\\etc\\hosts";

        public HostsFileEditorForm() {
            InitializeComponent();
        }

        /// <summary>
        /// Opens and loads the valid entries from the hosts files.
        /// 
        /// Only the lines following these patterns are loaded:
        /// xxx.yyy.zzz.qqq host-name (i.e 127.0.0.1 localhost)
        /// #xxx.yyy.zzz.qqq host-name (i.e #127.0.0.1 localhost)
        /// </summary>
        private void LoadHostsFile() {
            StreamReader FileReader = new StreamReader(pathToHostsFile);
            System.Text.RegularExpressions.Regex linePattern
                = new Regex(@"(^([#][\d]{1,3}|[\d]{1,3})\.[\d]{1,3}\.[\d]{1,3}\.[\d]{1,3})(\s+)(.+)");
            string line;
            while ((line = FileReader.ReadLine()) != null) {
                if (linePattern.IsMatch(line) == true) {
                    hostsList.Text += line + "\r\n";
                }
            }
            FileReader.Close();
        }

        /// <summary>
        /// OnLoad event that checks for the existence of hosts file
        /// and loads it if found
        /// </summary>
        private void HostsFileEditorForm_Load(object sender, EventArgs e) {
            // check if the hosts file exists
            if (File.Exists(pathToHostsFile) == false) {
                DialogResult warningMessageResult = MessageBox.Show("No hosts file was detected on " + pathToHostsFile
                    + "\r\nPlease make sure you have such a file present on your system.", "Zitec Hosts File Editor",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (warningMessageResult == DialogResult.OK) {
                    this.Close();
                }
            }
            LoadHostsFile();
        }

        /// <summary>
        /// Reload the hosts lists
        /// </summary>
        private void lnkLoadList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            // make sure the text box is empty
            if (hostsList.TextLength > 0) {
                hostsList.Clear();
            }

            // load the hosts file
            LoadHostsFile();

            // enable the "save" button
            btnSave.Enabled = true;
        }

        /// <summary>
        /// Hosts file save handler. It will overwrite the hosts
        /// file by performing a back-up first
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e) {
            // before saving do a back-up of the hosts file
            string shortDate = DateTime.UtcNow.ToString("yyyyMMddHmmss");
            File.Copy(pathToHostsFile, pathToHostsFile + "_" + shortDate + ".bak");

            // save the new content to hosts file
            FileStream hostsFile = new FileStream(pathToHostsFile, FileMode.Truncate);
            StreamWriter hostsFileWriter = new StreamWriter(hostsFile);
            hostsFileWriter.Write(hostsList.Text);
            hostsFileWriter.Close();
            hostsFile.Close();

            // clear the textbox
            hostsList.Clear();

            // disable save button
            btnSave.Enabled = false;

            // show a success message
            MessageBox.Show("The hosts file data was successfully saved.\r\nAlso, a backup for the existing file was performed.",
                "Hosts file saved!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
