using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace youtubedlui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private string[] fileTypes = {
            "mp4",
            "mp3"
        };

        private string cmdCommand = "youtube-dl.exe ";
        private string ytdlPath = "";

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("LOADED");
            string lookUp = "\\" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";
            //textBoxYTDLPath.Text = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            //textBoxYTDLPath.Text = lookUp;

            ytdlPath = Environment.ProcessPath.Substring(0,Environment.ProcessPath.IndexOf(lookUp));
            //fill dropdown
            foreach (string type in fileTypes) {
                comboBox1.Items.Add(type);
            }
            //fill filter
            string filter = "";
            for (int i = 0; i < fileTypes.Length; i++) {
                filter += fileTypes[i] + "|*." + fileTypes[i] + ((i != fileTypes.Length-1) ? "|" : "");
            }
            Console.WriteLine(filter);
            saveFileDialogOutputPath.Filter = filter;
            //turn off the clipping tools
            if (!checkBox1.Checked) {
                textBoxStartTime.Enabled = false;
                textBoxEndTime.Enabled = false;
            }
            
            this.ActiveControl = button1;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            //unused
        }

        private void label3_Click(object sender, EventArgs e)
        {
            //unused
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //unused
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //unused
        }

        private void BoxChecked(object sender, EventArgs e)
        {
            //debug
            if (checkBox1.Checked) {
                Console.WriteLine("CHECKED!");
                textBoxStartTime.Enabled = true;
                textBoxEndTime.Enabled = true;
            } else {
                Console.WriteLine("UNCHECKED!");
                textBoxStartTime.Enabled = false;
                textBoxEndTime.Enabled = false;
            }
        }

        private void LinkChanged(object sender, EventArgs e) {
            Console.WriteLine("LINK CHANGED!");
            isReady();
        }

        private void downloadClick(object sender, EventArgs e) {
            var cmdInfo = new System.Diagnostics.ProcessStartInfo();
            cmdInfo.FileName = "CMD.exe";
            cmdInfo.Arguments = buildCommand();
            cmdInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            cmdInfo.WorkingDirectory = ytdlPath;
            Console.WriteLine(buildCommand());
            System.Diagnostics.Process.Start(cmdInfo);
            //EndTimeToDuration();
        }

        //checks if all of the things needed to download are set
        private void isReady() {
            //check if all the requirements are fulfilled
            if (!textBoxLink.Text.Equals("") && comboBox1.SelectedItem != null && !textBoxOutputPath.Text.Equals("")) {
                button1.Enabled = true;
                Console.WriteLine("BUTTON ENABLED!");
            }
        }

        private string buildCommand() {
            string finalCommand = "/C youtube-dl.exe ";

            //select format
            if(comboBox1.Text == "mp4") {
                finalCommand += "-f mp4 ";
            } else if(comboBox1.Text == "mp3") {
                finalCommand += "-x --audio-format mp3 ";
            }
            finalCommand += textBoxLink.Text + " ";

            //clipping
            if (checkBox1.Checked) {
                finalCommand += "--external-downloader ffmpeg --external-downloader-args \"-ss "
                    + textBoxStartTime.Text + " -t " + EndTimeToDuration() + "\" ";
            }

            //output location
            if (!textBoxOutputPath.Text.Equals("")) {
                finalCommand += "--output \"" + saveFileDialogOutputPath.FileName + "\" ";
            }
            return finalCommand;
        }

        private void typeSelected(object sender, EventArgs e) {
            Console.WriteLine("TYPE CHANGED: " + comboBox1.Text);
            saveFileDialogOutputPath.FilterIndex = comboBox1.SelectedIndex+1;
            isReady();
        }

        private void BrowseYTDLEXE(object sender, EventArgs e) {
            //openFileDialogYTDLPath.ShowDialog();
            /*if (folderBrowserDialogYTDLPath.ShowDialog() == DialogResult.OK) {
                textBoxYTDLPath.Text = folderBrowserDialogYTDLPath.SelectedPath;
            }*/
            //unused
        }

        private void YTDLPathOpened(object sender, System.ComponentModel.CancelEventArgs e) {
            //unused
        }

        private void BrowseOutputFile(object sender, EventArgs e) {
            saveFileDialogOutputPath.ShowDialog();
            
        }

        private void OutputPathOpened(object sender, System.ComponentModel.CancelEventArgs e) {
            textBoxOutputPath.Text = saveFileDialogOutputPath.FileName;
        }

        private void folderBrowserDialogYTDLPath_HelpRequest(object sender, EventArgs e) {
            //unused
        }

        private void YTDLPathChanged(object sender, EventArgs e) {
            isReady();
        }

        private void OutputPathChanged(object sender, EventArgs e) {
            isReady();
        }

        private string EndTimeToDuration() {
            double[] start = TimeToArray(textBoxStartTime.Text);
            double[] end = TimeToArray(textBoxEndTime.Text);
            double[] result = new double[3];
            string duration = "";
            //duration = end - start
            //subtract seconds
            if (start[2] > end[2]) {
                if (end[1] == 0) {
                    throw new Exception("Invalid time!");
                }
                end[1]--;
                end[2] += 60;
            }
            result[2] = end[2] - start[2];
            //do the same thing with minutes
            if (start[1] > end[1]) {
                if (end[0] == 0) {
                    throw new Exception("Invalid time!");
                }
                end[0]--;
                end[1] += 60;
            }
            result[1] = end[1] - start[1];
            //hours
            if (start[0] > end[0]) {
                throw new Exception("Invalid time!");
            }
            result[0] = end[0] - start[0];
            //turn the resulting array back into a string and return the result
            duration = (int) result[0] + ":" + (int) result[1] + ":" + result[2];
            Console.WriteLine(duration);
            return duration;
        }

        //convert the time string into a double array
        private double[] TimeToArray(string input) {
            double[] splitTimes;
            //hours
            string time = input;
            int index = time.IndexOf(':');
            int hours = Int32.Parse(time.Substring(0, index));
            Console.WriteLine("HOURS: " + hours);
            //minutes
            index++;
            int index2 = time.IndexOf(':', index + 1);
            int minutes = Int32.Parse((time.Substring(index, index2 - index)));
            Console.WriteLine("MINUTES: " + minutes);
            //seconds - have decimals
            double seconds = Double.Parse(time.Substring(index2 + 1));
            Console.WriteLine("SECONDS: " + seconds);
            //build and return array
            splitTimes = new double[] { hours, minutes, seconds };
            return splitTimes;
        }

        private void WebsiteLinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ProcessStartInfo psInfo = new ProcessStartInfo {
                FileName = "https://jegneg.com/",
                UseShellExecute = true
            };
            Process.Start(psInfo);
        }
    }
}