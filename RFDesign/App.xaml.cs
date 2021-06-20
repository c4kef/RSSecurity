using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RFDesign.ViewModels;
using RFDesign.Views;
using MessageBox.Avalonia;
using System;

namespace RFDesign
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {

            Globals.MainWindowModel = new MainWindowViewModel();
#if !DEBUG
            if (!Globals.IsRoot())
            {
                MessageBoxManager.GetMessageBoxStandardWindow("Информация", "Войдите или запустите программу от имени администратора").Show();
                return;
            }
#endif
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Globals.MainWindow = desktop.MainWindow = new MainWindow
                {
                    DataContext = Globals.MainWindowModel,
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
            {
                
                singleViewLifetime.MainView = new MainView
                {
                    DataContext = Globals.MainWindowModel,
                };
                
            }

            var theme = new Avalonia.Themes.Default.DefaultTheme();
            theme.TryGetResource("Button", out _);

            //var theme1 = new Avalonia.Themes.Fluent.FluentTheme();
            //theme1.TryGetResource("Button", out _);

            base.OnFrameworkInitializationCompleted();
        }
    }
}
