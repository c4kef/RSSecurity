using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RFDesign
{
    class ShellHelper
    {
        public static string Exec(string command)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = @"/bin/bash";
            psi.Arguments = $"-c \"{command}\"";
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            var process = Process.Start(psi);

            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();

            return output;
        }
    }
}
