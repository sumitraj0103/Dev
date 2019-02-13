using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InstallNotePad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Path of the Exe which we want to Install
        public string ExePath = ".\\Software\\npp.7.6.3.Installer.exe";
        public MainWindow()
        {
           
            InitializeComponent();
        }

        private void Window_Content(object sender, EventArgs e)
        {
            string InstallSoft = "NotePad";
            BackgroundWorker worker = new BackgroundWorker();
            MyLabel.Content = "Installing "+InstallSoft+"...";
            MyLabel.Foreground = Brushes.White;
            MyLabel.FontSize = 20;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            // worker.RunWorkerCompleted += worker_InstallUpdate;
            worker.ProgressChanged += worker_ProgressChanged;
 

            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(100);
            }

        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage.Equals(40))
            {
                DeployApplications(ExePath);

            }
            else if(e.ProgressPercentage.Equals(99)){

                MessageBoxResult result = MessageBox.Show("Installation Completed!",
                "Status", MessageBoxButton.OK);
                if (result == MessageBoxResult.OK)
                {
                    this.Close();
                }
                
            }
            InstallStatus.Value = e.ProgressPercentage;    
        }
        void worker_InstallUpdate(object sender,RunWorkerCompletedEventArgs e)
        {
            //DeployApplications("C:\\Project\\Software\\npp.7.6.3.Installer.exe");
            MessageBox.Show("Completed the Installation! Click Ok to Close");
            this.Close(); 
        }
        public static void DeployApplications(string executableFilePath)
        {
            PowerShell powerShell = null;
            Console.WriteLine(" ");
            Console.WriteLine("Deploying application...");
            try
            {
                using (powerShell = PowerShell.Create())
                {
                    //here “executableFilePath” need to use in place of “  
                    //'C:\\ApplicationRepository\\FileZilla_3.14.1_win64-setup.exe'”  
                    //but I am using the path directly in the script.  

                    powerShell.AddScript("$setup=Start-Process " + executableFilePath + " -ArgumentList '/S' -Verb 'runas' -Wait");


                    Collection<PSObject> PSOutput = powerShell.Invoke();
                    foreach (PSObject outputItem in PSOutput)
                    {

                        if (outputItem != null)
                        {

                            Console.WriteLine(outputItem.BaseObject.GetType().FullName);
                            Console.WriteLine(outputItem.BaseObject.ToString() + "\n");
                        }
                    }

                    if (powerShell.Streams.Error.Count > 0)
                    {
                        string temp = powerShell.Streams.Error.First().ToString();
                        Console.WriteLine("Error: {0}", temp);

                    }
                    else
                        Console.WriteLine("Installation has completed successfully.");



                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: {0}", ex.InnerException);
                //throw;  
            }
            finally
            {
                if (powerShell != null)
                    powerShell.Dispose();
            }

        }
    }
}
