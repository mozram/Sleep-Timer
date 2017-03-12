using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
using System.Windows.Threading;

namespace Sleep_Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        TimeSpan time;

        // Import function that allows this program to sleep and hibernate
        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        // Import function that allows this program to logoff
        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        // Import function that allows this program to lock
        [DllImport("user32")]
        public static extern void LockWorkStation();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // get command type
            string command = (bool)rdbtnSleep.IsChecked ? "sleep" : (bool)rdbtnHibernate.IsChecked? "hibernate" : (bool)rdbtnShutdown.IsChecked ? "shutdown" : (bool)rdbtnRestart.IsChecked ? "restart" : (bool)rdbtnLogoff.IsChecked ? "logoff" : (bool)rdbtnLock.IsChecked ? "lock" : "error";
            if(command == "error")
            {
                MessageBox.Show("Failed to set command!");
            }

            int minutes = 0;

            // parse string to int. If failed show message
            if(!int.TryParse(txtboxMinutes.Text, out minutes))
            {
                MessageBox.Show("Invalid value for minutes!");
                return;
            }

            time = TimeSpan.FromSeconds(minutes * 60);
            timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                lblCounter.Content = time.ToString("c");
                if (time == TimeSpan.Zero)
                {
                    //MessageBox.Show("Done counting!");
                    timer.Stop();
                    switch (command)
                    {
                        case "sleep":
                            // Standby
                            SetSuspendState(false, true, true);
                            break;
                        case "hibernate":
                            // Hibernate
                            SetSuspendState(true, true, true);
                            break;
                        case "shutdown":
                            // Shutdown pc
                            Process.Start("shutdown", "/s /t 0");
                            break;
                        case "restart":
                            // Restart pc
                            Process.Start("shutdown", "/r /t 0");
                            break;
                        case "logoff":
                            ExitWindowsEx(0, 0);
                            break;
                        case "lock":
                            LockWorkStation();
                            break;
                        default:
                            MessageBox.Show("Command error!");
                            break;
                    }
                }

                time = time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);

            timer.Start();

        }

        private void txtboxMinutes_GotFocus(object sender, RoutedEventArgs e)
        {
            int result;
            if(!int.TryParse(txtboxMinutes.Text, out result))
            {
                txtboxMinutes.Text = "";
            }
                    
        }

        private void txtboxMinutes_LostFocus(object sender, RoutedEventArgs e)
        {
            int result;
            if (!int.TryParse(txtboxMinutes.Text, out result))
            {
                txtboxMinutes.Text = "Enter time in minutes";
            }
        }
    }


}
