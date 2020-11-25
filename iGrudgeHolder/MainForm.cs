using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iRacingSdkWrapper;
using YamlDotNet.Serialization;

namespace iGrudgeHolder
{
    public partial class MainForm : Form
    {
        private SdkWrapper _wrapper;
        private CommentDirectory _commentDirectory = new CommentDirectory(Application.LocalUserAppDataPath);
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _wrapper = new SdkWrapper();
            _wrapper.SessionInfoUpdated += OnSessionInfoUpdated;
            _wrapper.Start();
        }

        private void OnSessionInfoUpdated(object sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
        {
            driversGridView.Rows.Clear();

            var deserializer = new Deserializer();
            var sr = new StringReader(e.SessionInfo.RawYaml);
            var o = deserializer.Deserialize<dynamic>(sr);
            var drivers = o["DriverInfo"]["Drivers"];
            foreach (var driver in drivers)
            {
                if (driver["CarIsPaceCar"] == "1" || driver["IsSpectator"] == "1")
                {
                    continue;
                }

                string userIdStr = driver["UserID"];
                int userIdInt = Convert.ToInt32(userIdStr);
                string comment = _commentDirectory[userIdInt];
                var commentFirstLine = GetFirstLine(comment);
                object[] row = { userIdStr, driver["UserName"], driver["IRating"], driver["LicString"], commentFirstLine};
                driversGridView.Rows.Add(row);
            }
        }

        private static string GetFirstLine(string str)
        {
            string firstLine;
            using (var reader = new StringReader(str))
            {
                firstLine = reader.ReadLine();
            }

            return firstLine;
        }

        private void UpdateComment(int userId)
        {
            foreach (DataGridViewRow row in driversGridView.Rows)
            {
                if (row.Cells["UserId"].Value as string == userId.ToString())
                {
                    row.Cells["Comment"].Value = GetFirstLine(_commentDirectory[userId]);
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _wrapper.Stop();
        }

        private void driversGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int userId = Convert.ToInt32(driversGridView.Rows[e.RowIndex].Cells["UserId"].Value);
            string userName = driversGridView.Rows[e.RowIndex].Cells["Name"].Value as string;
            userIdLabel.Text = userId.ToString();
            userNameLabel.Text = userName;
            commentTextBox.Text = _commentDirectory[userId];
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(userIdLabel.Text);
            _commentDirectory[userId] = commentTextBox.Text;
            UpdateComment(userId);
        }
    }
}
