using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MessageBox.Avalonia;
using System.IO;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.Threading;
using Avalonia.Threading;

namespace RFDesign.Pages
{
    public struct SIConfig
    {
        public string name;
        public string[] value;
    }

    public class SystemIntegrityPage : UserControl
    {
        public SystemIntegrityPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.Find<TextBox>("Editor").Text = File.ReadAllText(@"/etc/afick.conf");
        }

        private void Save(object sender, RoutedEventArgs e) => File.WriteAllText(@"/etc/afick.conf", this.Find<TextBox>("Editor").Text);
        private void Run(object sender, RoutedEventArgs e)
        {
            Globals.MainWindowModel.SIChecking = true;
            this.Find<TextBox>("Logs").Text = string.Empty;
            this.Find<TextBox>("Logs").Text += "Началась проверка, дождитесь результатов";
            new Thread(new ThreadStart(() =>
            {
                string result = ShellHelper.Exec("afick -k");
                Dispatcher.UIThread.InvokeAsync(new System.Action(() =>
                {
                    this.Find<TextBox>("Logs").Text += result += "\nОкончание работы проверки";
                    Globals.MainWindowModel.SIChecking = false;
                }));
            })).Start();
        }
    }
}
