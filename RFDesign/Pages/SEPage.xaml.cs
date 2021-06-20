using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MessageBox.Avalonia;
using System.IO;

namespace RFDesign.Pages
{
    public class SEPage : UserControl
    {

        public SEPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SEPTypeChanged(object sender, SelectionChangedEventArgs e) => Globals.MainWindowModel.SESetType(sender as ComboBox);
    }
}
