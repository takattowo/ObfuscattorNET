
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSurface;
using ObfuscattorLib;

namespace ObfuscattorNET
{
    public partial class MainForm : Form
    {
        bool raiseError = false;
        int second = 0, transitionSec = 3;
        float showTime = 4f;
        Timer timer = new Timer() { Interval = 1000 };
        public MainForm()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            #region Text Field
            cbbTextfieldStyle.SelectedIndexChanged += (s, ev) =>
            {
                if (cbbTextfieldStyle.SelectedIndex < 0)
                    return;
                txtMainDemo.Style = (MaterialTextfield.TextfieldStyle)cbbTextfieldStyle.SelectedIndex;
            };
            ckbTextfieldEnable.CheckedChanged += (s, ev) => txtMainDemo.Enabled = ckbTextfieldEnable.Checked;
            ckbCountChar.CheckedChanged += (s, ev) => txtMainDemo.CountText = ckbCountChar.Checked;
            ckbSysPassword.CheckedChanged += (s, ev) => txtMainDemo.UseSystemPasswordChar = ckbSysPassword.Checked;
            ckbMultiLine.CheckedChanged += (s, ev) =>
            {
                txtMainDemo.Multiline = ckbMultiLine.Checked;
                if (ckbMultiLine.Checked)
                {
                    txtMainDemo.Location = new Point(55, 50);
                    txtMainDemo.Size = new Size(420, 170);
                }
                else
                {
                    txtMainDemo.Location = new Point(120, 125);
                    txtMainDemo.Size = new Size(300, 67);
                }
            };
            txtTextfieldHint.TextChanged += (s, ev) => txtMainDemo.HintText = txtTextfieldHint.Text;
            txtTextfieldLabel.TextChanged += (s, ev) => txtMainDemo.FloatingLabelText = txtTextfieldLabel.Text;
            ckbShowHelperText.CheckedChanged += (s, ev) =>
            {
                txtHelperText.Enabled = ckbShowHelperText.Checked;
                if (!ckbShowHelperText.Checked)
                    txtMainDemo.HelperText = "";
                else
                    txtMainDemo.HelperText = txtHelperText.Text;
            };
            txtHelperText.TextChanged += (s, ev) =>
            {
                if (ckbShowHelperText.Checked)
                    txtMainDemo.HelperText = txtHelperText.Text;
            };
            btnRaiseError.Click += (s, ev) =>
            {
                if (!raiseError)
                {
                    raiseError = true;
                    Snackbar.MakeSnackbar(this, "You can raise an error with your custom message on Textfield.", "OK");
                }
                txtMainDemo.RaiseError(txtTextfieldError.Text);
            };
            txtMainDemo.TextChanged += (s, ev) => txtMainDemo.RemoveError();
            #endregion

            #region Progressbar
            ckbProgressbarIndetermine.CheckedChanged += (s, ev) => mainProgressBar.IsIndetermine = ckbProgressbarIndetermine.Checked;
            txtProgressbarValue.TextChanged += (s, ev) =>
            {
                if (txtProgressbarValue.hasError)
                    txtProgressbarValue.RemoveError();
                if (Int32.TryParse(txtProgressbarValue.Text, out int value))
                {
                    if (value >= 0 && value <= 100)
                        mainProgressBar.Value = value;
                    else
                        txtProgressbarValue.RaiseError("Value must be between 0 and 100.", false);
                }
                else
                    txtProgressbarValue.RaiseError("Digit only!", false);
            };
            txtProgressbarDelay.TextChanged += (s, ev) =>
             {
                 if (txtProgressbarDelay.hasError)
                     txtProgressbarDelay.RemoveError();
                 if (Int32.TryParse(txtProgressbarDelay.Text, out int value))
                 {
                     if (value > 10)
                         mainProgressBar.ChangeDelay = value;
                     else
                         txtProgressbarDelay.RaiseError("Value must be greater than 10", false);
                 }
                 else
                     txtProgressbarDelay.RaiseError("Digit only!", false);
             };
            
            
            #endregion

            #region Card
            ckbCardMouseInteract.CheckedChanged += (s, ev) => mainCard.MouseInteract = ckbCardMouseInteract.Checked;
            #endregion

            #region Snackbar && Dialog
            txtSnackbarTimeShow.TextChanged += (s, ev) =>
            {
                if (txtSnackbarTimeShow.hasError)
                    txtSnackbarTimeShow.RemoveError();
                if (!float.TryParse(txtSnackbarTimeShow.Text, out showTime))
                    txtSnackbarTimeShow.RaiseError("Digit only", false);
                else
                {
                    if (showTime < 1)
                        txtSnackbarTimeShow.RaiseError("Time show of snackbar must be greater than 1 second.", false);
                }
            };
            btnMakeSnackbar.Click += (s, ev) => Snackbar.MakeSnackbar(this, txtSnackbarMessage.Text, txtSnackbarButtonText.Text, showTime);
            cbbDialogsButton.SelectedIndex = 1;
            btnShowDialog.Click += (s, ev) => Dialog.Show(this, txtDialogMessage.Text, txtDialogTittle.Text, (Buttons)cbbDialogsButton.SelectedIndex,
                ckbDialogDimScreen.Checked);
            ckbDialogDark.CheckedChanged += (s, ev) => Dialog.DarkTheme = ckbDialogDark.Checked;
            ckbSnackbarDark.CheckedChanged += (s, ev) => Snackbar.DarkTheme = ckbSnackbarDark.Checked;
            #endregion
        }
        
        protected async override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            cbbTextfieldStyle.SelectedIndex = 0;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (Dialog.Show(this, "Are you sure you want to exit?", "Exit", Buttons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }
        private void ShowGithub(object sender, EventArgs e)
        {
            Process.Start("https://github.com/princ3od/MaterialSurface");
        }
        private void ShowLinkedIn(object sender, EventArgs e)
        {
            Process.Start("https://www.linkedin.com/in/princ3od/");
        }
        private void CopyMail(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Snackbar.MakeSnackbar(this, "Project has been loaded!", "Ok");

            var project = File.ReadAllText(Application.StartupPath + "\\Projects\\" + btnLastProject.Text + ".obcat");
            Path = project.Split('|')[0];
            Destination = project.Split('|')[1];
            BuildOptions = project.Split('|')[2];
            try
            {
                SetNewPath();
                SetDestinateFolder();
                ProjectExist = true;
                materialTabControl1.SelectedIndex = 3;
            }
            catch
            {
                Dialog.Show(this, "This is not a valid Obfuscattor project!", "Failed ヽ(￣ω￣ )", (Buttons)cbbDialogsButton.SelectedIndex, ckbDialogDimScreen.Checked);
                ProjectExist = false;
                Path = null;
                Destination = null;
                BuildOptions = null;
            }       
        }
        
        private void ChangeTextfieldType(object sender, EventArgs e)
        {
            if (rbtnNormalTextfield.Checked)
                txtMainDemo.FieldType = BoxType.Normal;
            else if (rbtnFilledTextfield.Checked)
                txtMainDemo.FieldType = BoxType.Filled;
            else
                txtMainDemo.FieldType = BoxType.Outlined;
        }
        private void outlinedButton3_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            materialTabControl1.SelectedIndex = 1;
        }
        
        public static Worker Worker { get; set; }
        public static string Path { get; set; }
        private void btnBrowseAssembly_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;

            var openFileDialog = new OpenFileDialog
            {
                Title = "Browse a .NET assembly",
                Filter = ".NET assembly|*.exe; *.dll; *.netmodule",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Path = openFileDialog.FileName;
                SetNewPath();
            }
        }

        void SetNewPath()
        {
            try
            {
                Worker = new Worker(Path);
                var list = Worker.LoadDependenciesList();

                lbProjectNameSelect.Text = "Selected Software: " + Worker.ReturnAssemblyName().Split(',').First();

                lbAssemblyPath.Text = Worker.ReturnAssemblyName();
                lbAssemblyResolve.Text = string.Empty;

                for (int i = 0; i < list.Count; i++)
                {
                    lbAssemblyResolve.AppendText(list[i]);
                    if (i < list.Count - 1)
                        lbAssemblyResolve.AppendText(Environment.NewLine);
                }

                lbAssemblyResolve.SelectionStart = 0;
                lbAssemblyResolve.ScrollToCaret();

                cardNewProject.Visible = false;
                materialCard_selected.Visible = true;

                ProjectExist = true;
            }
            catch (Exception ee)
            {
                Dialog.Show(this, ee.ToString(), "Failed ヽ(￣ω￣ )", (Buttons)cbbDialogsButton.SelectedIndex, ckbDialogDimScreen.Checked);
                this.ActiveControl = null;
                Worker = new Worker(string.Empty);
                materialCard_selected.Visible = false;
                cardNewProject.Visible = true;

                ProjectExist = false;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Worker = new Worker(string.Empty);
            materialCard_selected.Visible = false;
            cardNewProject.Visible = true;
        }
        
        public static string Destination { get; set; }
        private void btnSetDestinationAssembly_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                Destination = folderBrowserDialog1.SelectedPath;
                SetDestinateFolder();
            }
        }

        void SetDestinateFolder()
        {
            try
            {       
                Worker.SetDestination(Destination);
                labelDestiPath.ForeColor = Color.Green;
                labelDestiPath.Text = Destination;
            }
            catch (Exception ee)
            {
                labelDestiPath.ForeColor = Color.OrangeRed;
                labelDestiPath.Text = "Click 'Set destination' to choose a destination asssembly.";
                Dialog.Show(this, ee.ToString(), "Failed ヽ(￣ω￣ )", (Buttons)cbbDialogsButton.SelectedIndex, ckbDialogDimScreen.Checked);
            }
        }

        private void outlinedButton4_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            materialTabControl1.SelectedIndex = 3;
            cardProjectSettingDestination.Visible = true;
            cardObfuscateOptions.Visible = false;
        }

        private void btnContinueToEncrypt_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(labelDestiPath.Text))
            {
                Dialog.Show(this, "The folder you selected is invalid.", "Failed ヽ(￣ω￣ )", (Buttons)cbbDialogsButton.SelectedIndex, ckbDialogDimScreen.Checked);
                return;
            }

            cardProjectSettingDestination.Visible = false;
            cardObfuscateOptions.Visible = true;
        }

        private void labelDestiPath_TextChanged(object sender, EventArgs e)
        {
            btnContinueToEncrypt.Visible = false;
            try
            {
                if (Directory.Exists(labelDestiPath.Text))
                {
                    var name = Path.Split('\\').Last();
                    var des = Destination + "\\" + name;
                    
                    if (des != Path)
                        btnContinueToEncrypt.Visible = true;
                    else
                        Dialog.Show(this, "The folder you selected is not valid.", "Failed ヽ(￣ω￣ )", (Buttons)cbbDialogsButton.SelectedIndex, ckbDialogDimScreen.Checked);
                }

                //get the folder path except filename from Path
            } catch { }
        }
        
        private void btnReSetDestination_Click(object sender, EventArgs e)
        {
            cardProjectSettingDestination.Visible = true;
            cardObfuscateOptions.Visible = false;
        }
        
        public string BuildOptions { get; set; }
        private async void btnBuild_Click(object sender, EventArgs e)
        {
            BuildOptions = null;
            if (btnO1.Checked)
                BuildOptions += "1";
            if (btnO2.Checked)
                BuildOptions += "2";
            if (btnO3.Checked)
                BuildOptions += "3";
            if (btnO4.Checked)
                BuildOptions += "4";
            if (btnO5.Checked)
                BuildOptions += "5";
            if (btnO6.Checked)
                BuildOptions += "6";
            if (btnO7.Checked)
                BuildOptions += "7";
            if (btnO8.Checked)
                BuildOptions += "8";
            if (btnO9.Checked)
                BuildOptions += "9";
            
            if(string.IsNullOrEmpty(BuildOptions) || string.IsNullOrWhiteSpace(BuildOptions))
            {
                Dialog.Show(this, "You forgot to select option! - -", "Failed ヽ(￣ω￣ )", (Buttons)cbbDialogsButton.SelectedIndex, ckbDialogDimScreen.Checked);
                return;
            }

            try { 
            pnLoading.BringToFront();
            await Task.Run(() => Worker.ExecuteObfuscations(BuildOptions));
            await Task.Run(() => Worker.Save());
            await Task.Delay(2000);

                label45.Text = "Build succeeded!";
                label45.ForeColor = Color.ForestGreen;
                label43.Text = "Your assembly has been magically created by the Wizard Cat!";
                
                btnSave.Enabled = true;
                btnSave.Text = "Save project";
                cardBuildCompleted.Visible = true;         
                cardProjectSettingDestination.Visible = false;
                cardObfuscateOptions.Visible = false;

            pnLoading.SendToBack();
            }
            catch (Exception x)
            {
                label45.Text = "Build failed!";
                label45.ForeColor = Color.Crimson;
                label43.Text = "Something has gone wrong, the cat could not finish her work. Please check the log for informations.";
                
                btnSave.Enabled = true;
                btnSave.Text = "Save project";
                cardBuildCompleted.Visible = true;
                cardProjectSettingDestination.Visible = false;
                cardObfuscateOptions.Visible = false;
                
                pnLoading.SendToBack();
                
                Dialog.Show(this, x.ToString(), "Failed ヽ(￣ω￣ )", (Buttons)cbbDialogsButton.SelectedIndex, ckbDialogDimScreen.Checked);
            }
        }

        private void btnClearOptions_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;

            btnO1.Checked = false;
            btnO2.Checked = false;
            btnO3.Checked = false;
            btnO4.Checked = false;
            btnO5.Checked = false;
            btnO6.Checked = false;
            btnO7.Checked = false;
            btnO8.Checked = false;
            btnO9.Checked = false;
        }

        private void btnSelectAllOptions_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;

            btnO1.Checked = true;
            btnO2.Checked = true;
            btnO3.Checked = true;
            btnO4.Checked = true;
            btnO5.Checked = true;
            btnO6.Checked = true;
            btnO7.Checked = true;
            btnO8.Checked = true;
            btnO9.Checked = true;
        }

        private void btnReContinueToBuild_Click(object sender, EventArgs e)
        {
            Worker = new Worker(Path);
            Worker.SetDestination(Destination);
            
            cardBuildCompleted.Visible = false;
            cardProjectSettingDestination.Visible = false;
            cardObfuscateOptions.Visible = true;
        }

        private void btnSeeLogs_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            logTimer.Enabled = true;
        }

        private void logTimer_Tick(object sender, EventArgs e)
        {
            logTimer.Enabled = false;

            System.IO.File.WriteAllText(Destination + "\\log.txt", Worker.Log);
            
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(@Destination + "\\log.txt");
            process.Exited += new EventHandler(ProcessExited);
            process.Start();     
        }

        void ProcessExited(object sender, System.EventArgs e)
        {
            File.Delete(@Destination + "\\log.txt");
        }

        void LoadForLastestProject()
        {
            //get last modified single file with .obcat extension in C:\
            string pattern = "*.obcat";
            var dirInfo = new DirectoryInfo(@Application.StartupPath + "\\Projects");
            FileInfo file = null;
            try
            {
                file = (from f in dirInfo.GetFiles(pattern) orderby f.LastWriteTime descending select f).First();
            }
            catch { }

            if (file != null)
            {
                label47.Visible = false;
                btnLastProject.Text = file.Name.Replace(".obcat", "");
                btnLastProject.Visible = true;
            }
            else
            {
                label47.Visible = true;
                btnLastProject.Visible = false;
            }          
        }

        private void btn_Prj_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            var btn = sender as Button;
            var project = File.ReadAllText(Application.StartupPath + "\\Projects\\" + btn.Text + ".obcat");
            Path = project.Split('|')[0];
            Destination = project.Split('|')[1];
            BuildOptions = project.Split('|')[2];
            try
            {
                SetNewPath();
                SetDestinateFolder();
                ProjectExist = true;
                materialTabControl1.SelectedIndex = 3;

                Snackbar.MakeSnackbar(this, "Project has been loaded!", "Ok");
            }
            catch
            {
                Dialog.Show(this, "This is not a valid Obfuscattor project!", "Failed ヽ(￣ω￣ )", (Buttons)cbbDialogsButton.SelectedIndex, ckbDialogDimScreen.Checked);
                ProjectExist = false;
                Path = null;
                Destination = null;
                BuildOptions = null;
            }
        }

        public static bool ProjectExist { get; set; } = false;
        private void materialTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if selected tab is 1 then close program
            if (materialTabControl1.SelectedIndex == 0)
            {
                LoadForLastestProject();
            }

            if (materialTabControl1.SelectedIndex == 1)
            {
                cardNewProject.Visible = true;
                materialCard_selected.Visible = false;
            }

            if (materialTabControl1.SelectedIndex == 2)
            {
                //add the mount of buttons to flowLayoutPanel1 according to amount of .obcat file in C:\
                string pattern = "*.obcat";
                var dirInfo = new DirectoryInfo(@Application.StartupPath + "\\Projects");
                FileInfo[] files = dirInfo.GetFiles(pattern);
                flowLayoutPanel1.Controls.Clear();

                if(files == null || files.Length < 1)
                {
                    label48.Visible = true;
                    return;
                }
                
                label48.Visible = false;
                foreach (FileInfo file in files)
                {
                    ContainedButton btn = new ContainedButton();
                    btn.PrimaryColor = Color.OrangeRed;
                    btn.Text = file.Name.Replace(".obcat", "");
                    btn.Name = file.Name.Replace(".obcat", "");
                    btn.Click += new EventHandler(btn_Prj_Click);

                    flowLayoutPanel1.Controls.Add(btn);
                }
                

            }

            if (materialTabControl1.SelectedIndex == 3)
            {
                pnByPass.BringToFront();
                pnByPass.Visible = !ProjectExist;

                if(ProjectExist)
                {
                    if (!string.IsNullOrEmpty(BuildOptions))
                    {
                        if (BuildOptions.Contains("1"))
                            btnO1.Checked = true;
                        if (BuildOptions.Contains("2"))
                            btnO2.Checked = true;
                        if (BuildOptions.Contains("3"))
                            btnO3.Checked = true;
                        if (BuildOptions.Contains("4"))
                            btnO4.Checked = true;
                        if (BuildOptions.Contains("5"))
                            btnO5.Checked = true;
                        if (BuildOptions.Contains("6"))
                            btnO6.Checked = true;
                        if (BuildOptions.Contains("7"))
                            btnO7.Checked = true;
                        if (BuildOptions.Contains("8"))
                            btnO8.Checked = true;
                        if (BuildOptions.Contains("9"))
                            btnO9.Checked = true;
                    }

                    if (string.IsNullOrEmpty(Destination))
                    {
                        cardProjectSettingDestination.Visible = true;
                        cardObfuscateOptions.Visible = false;  
                    }
                    else
                    {
                        cardProjectSettingDestination.Visible = false;
                        cardObfuscateOptions.Visible = true;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            try
            {
                File.WriteAllText(Application.StartupPath + "\\Projects\\" + Worker.ReturnAssemblyName().Split(',').First() + ".obcat", Path + "|" + Destination + "|" + BuildOptions);
                //btnReContinueToBuild_Click(null, null);
                Snackbar.MakeSnackbar(this, "Project has been saved!", "Ok");

                btnSave.Enabled = false;
                btnSave.Text = "Project saved";               
            }
            catch (Exception eee)
            {
                Dialog.Show(this, eee.ToString(), "Failed ヽ(￣ω￣ )", (Buttons)cbbDialogsButton.SelectedIndex, ckbDialogDimScreen.Checked);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(Application.StartupPath + "\\Projects"))
                Directory.CreateDirectory(Application.StartupPath + "\\Projects");
            
            LoadForLastestProject();
        }

        private void btnO5_CheckedChanged(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }

        private void btnLaunchTestProgram_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            try
            {
                Process.Start(Path);
            }
            catch (Exception eee)
            {
                Dialog.Show(this, eee.ToString(), "Failed ヽ(￣ω￣ )", (Buttons)cbbDialogsButton.SelectedIndex, ckbDialogDimScreen.Checked);
            }
        }

        private void btnBrowseForProject_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;

            var openFileDialog = new OpenFileDialog
            {
                Title = "Browse a Obfuscattor project",
                Filter = "Obfuscattor project|*.obcat",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var project = File.ReadAllText(openFileDialog.FileName);
                Path = project.Split('|')[0];
                Destination = project.Split('|')[1];
                BuildOptions = project.Split('|')[2];
                try
                {
                    SetNewPath();
                    SetDestinateFolder();
                    ProjectExist = true;
                    materialTabControl1.SelectedIndex = 3;
                    Snackbar.MakeSnackbar(this, "Project has been loaded!", "Ok");
                    File.Copy(openFileDialog.FileName, Application.StartupPath + "\\Projects\\" + openFileDialog.SafeFileName, true);
                }
                catch
                {
                    Dialog.Show(this, "This is not a valid Obfuscattor project!", "Failed ヽ(￣ω￣ )", (Buttons)cbbDialogsButton.SelectedIndex, ckbDialogDimScreen.Checked);
                    ProjectExist = false;
                    Path = null;
                    Destination = null;
                    BuildOptions = null;
                }
            }
        }

        private void btnWhatTheFuck_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Dialog.Show(this, "Please just Google it.", "Why are you so lazy? ヽ(￣ω￣ )", (Buttons)cbbDialogsButton.SelectedIndex, ckbDialogDimScreen.Checked);
        }

        private void btnOpenProjectSaveFolder_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            
            try
            {
                Process.Start(Application.StartupPath + "\\Projects");
            }
            catch (Exception eee)
            {
                Dialog.Show(this, eee.ToString(), "Failed ヽ(￣ω￣ )", (Buttons)cbbDialogsButton.SelectedIndex, ckbDialogDimScreen.Checked);
            }
        }

        private void ChangeCardType(object sender, EventArgs e)
        {

        }

        private void ChangeComboBoxType(object sender, EventArgs e)
        {

        }

        private void ChangeChipType(object sender, EventArgs e)
        {

        }

        private void label11OpenAsFolder_Click(object sender, EventArgs e)
        {
            Process.Start(Destination);
        }

        private void ChangeProgressbarType(object sender, EventArgs e)
        {
            if (rbtnProgressbarNormal.Checked)
            {
                mainProgressBar.Location = new Point(90, 160);
                mainProgressBar.Size = new Size(350, 8);
                mainProgressBar.Type = MaterialProgressbar.ProgressBarType.Normal;
            }
            else
            {
                mainProgressBar.Location = new Point(240, 140);
                mainProgressBar.Size = new Size(60, 60);
                mainProgressBar.Type = MaterialProgressbar.ProgressBarType.Cicular;
            }
        }
    }
}
