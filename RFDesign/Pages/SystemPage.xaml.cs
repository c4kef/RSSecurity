using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MessageBox.Avalonia;
using System.IO;

namespace RFDesign.Pages
{
    public class SystemPage : UserControl
    {

        public SystemPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void SelectedDateChanged(object sender, DatePickerSelectedValueChangedEventArgs e) => Globals.MainWindowModel.DateChanged(e);

        public void SelectTimeChanged(object sender, TimePickerSelectedValueChangedEventArgs e) => Globals.MainWindowModel.TimeChanged(e);
    }
}
