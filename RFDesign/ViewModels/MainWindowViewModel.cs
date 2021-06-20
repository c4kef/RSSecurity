using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using MessageBox.Avalonia;
using ReactiveUI;
using RFDesign.Pages;

namespace RFDesign.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _isMenuItemChecked;

        #region SI Page
        private bool _siChecking;

        public bool SIChecking
        {
            get
            {
                return _siChecking;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _siChecking, value);
            }
        }
        #endregion

        #region System Page
        private bool _isSyncTimeAuto = true;
        private TimeSpan _isHoursAndMinutes = DateTime.Now.TimeOfDay;
        private DateTime _isMonthlysAndDaysAndYears = DateTime.Now;
        #endregion

        #region SE Setup
        private string _isCheckedSEStatus;
        private int _isTypedSE;
        #endregion

        public MainWindowViewModel()
        {
            ToggleMenuItemCheckedCommand = ReactiveCommand.Create(() =>
            {
                IsMenuItemChecked = !IsMenuItemChecked;
            });
        }

        #region System Page

        //Protect files from RWD (TODO)
        public bool IsSetProtectFiles
        {
            get
            {
                return !ShellHelper.Exec("dnf list fileprotect").Contains("Ошибка:");
            }

            set
            {
                new Thread(new ThreadStart(() =>
                {
                    if (value)
                        ShellHelper.Exec("dnf install fileprotect");
                    else
                        ShellHelper.Exec("dnf remove fileprotect");
                })).Start();

                MessageBoxManager.GetMessageBoxStandardWindow("Важно", "Может потребоваться перезагрузка").Show();
            }
        }

        //Set time auto
        public bool IsSyncTimeAuto
        {
            get
            {
                return _isSyncTimeAuto;
            }

            set
            {
                new Thread(new ThreadStart(() =>
                {
                    if (value)
                    {
                        ShellHelper.Exec("systemctl enable ntpd");
                        ShellHelper.Exec("systemctl start ntpd");
                        ShellHelper.Exec("ntpdate -u pool.ntp.org");
                    }
                    else
                    {
                        ShellHelper.Exec("systemctl disable ntpd");
                        ShellHelper.Exec("systemctl stop ntpd");
                        ShellHelper.Exec("ntpdate -u pool.ntp.org");
                    }
                })).Start();

                this.RaiseAndSetIfChanged(ref _isSyncTimeAuto, value);
            }
        }

        public void DateChanged(DatePickerSelectedValueChangedEventArgs e)
        {
            if (_isSyncTimeAuto)
                return;

            new Thread(new ThreadStart(() =>
            {
                ShellHelper.Exec($"date +%Y%m%d -s \"{e.NewDate.Value.Year}{e.NewDate.Value.ToString("MM")}{e.NewDate.Value.ToString("dd")}\"");
            })).Start();

                this.RaiseAndSetIfChanged(ref _isMonthlysAndDaysAndYears, DateTime.Parse(e.NewDate.Value.ToString()));
        }

        public void TimeChanged(TimePickerSelectedValueChangedEventArgs e)
        {
            if (_isSyncTimeAuto)
                return;

            new Thread(new ThreadStart(() =>
            {
                ShellHelper.Exec($"date +%T -s \"{e.NewTime.Value.Hours}:{e.NewTime.Value.Minutes}\"");
            })).Start();

            this.RaiseAndSetIfChanged(ref _isHoursAndMinutes, e.NewTime.Value);
        }

        //Set time minutes and hours
        public TimeSpan GetHM
        {
            get
            {
                return _isHoursAndMinutes;
            }
        }

        //Set time monthly, days and years
        public DateTimeOffset GetMDY
        {
            get
            {
                return _isMonthlysAndDaysAndYears;
            }
        }

        public bool IsMenuItemChecked
        {
            get { return _isMenuItemChecked; }
            set { this.RaiseAndSetIfChanged(ref _isMenuItemChecked, value); }
        }
        #endregion

        #region SE Setup
        public bool IsSEEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(_isCheckedSEStatus))
                    _isCheckedSEStatus = (!ShellHelper.Exec("sestatus").Split('\n')[0].Contains("disabled")).ToString();

                return bool.Parse(_isCheckedSEStatus);
            }
            set
            {
                string path = @"/etc/selinux/config";
                string[] lines = File.ReadAllLines(path);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("SELINUX=") && !lines[i].Contains("#"))
                    {
                        lines[i] = $"SELINUX={(value ? "enforcing" : "disabled")}";
                        break;
                    }
                }

                File.WriteAllLines(path, lines);
                MessageBoxManager.GetMessageBoxStandardWindow("Внимание", "Для применения перезагрузитесь").Show();
            }
        }

        public int IsSelectedType
        {
            get
            {
                if (_isTypedSE == -1)
                {
                    string path = @"/etc/selinux/config";
                    string[] lines = File.ReadAllLines(path);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("SELINUXTYPE=") && !lines[i].Contains("#"))
                        {
                            _isTypedSE = GetIndexSEType(lines[i].Replace("SELINUXTYPE=", ""));
                            break;
                        }
                    }
                }
                
                return _isTypedSE;
            }
        }

        public void SESetType(ComboBox sender)
        {
            if (!IsSEEnabled)
                return;

            string path = @"/etc/selinux/config";
            string[] lines = File.ReadAllLines(path);

            _isTypedSE = sender.SelectedIndex;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("SELINUXTYPE=") && !lines[i].Contains("#"))
                {
                    lines[i] = $"SELINUXTYPE={(sender.SelectedItem as ComboBoxItem).Content as string}";
                    break;
                }
            }

            MessageBoxManager.GetMessageBoxStandardWindow("Внимание", "Для применения перезагрузитесь").Show();
        }

        //PASTA LA VISTO FIX IT!
        private int GetIndexSEType(string answer)
        {
            if (answer.Contains("targeted"))
                return 0;
            else if (answer.Contains("strict"))
                return 1;
            else if (answer.Contains("mls"))
                return 2;

            return -1;
        }

        #endregion

        public ReactiveCommand<Unit, Unit> ToggleMenuItemCheckedCommand { get; }
    }
}
