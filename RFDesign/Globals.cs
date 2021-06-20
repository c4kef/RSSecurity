using Avalonia.Controls;
using RFDesign.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RFDesign
{
    //Globals links
    class Globals
    {
        public static Window MainWindow;
        public static MainWindowViewModel MainWindowModel;

        public static bool IsRoot() => ShellHelper.Exec("whoami").ToLower().Split('\n')[0] == "root";
    }
}
