using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OrnaLibs.Application
{
    public partial struct Application
    {
        private void RegistrationWindows()
        {
            using (var reg = GetUninstallPath().CreateSubKey(Name, true))
            {
                reg.SetValue("Publisher", CompanyName, RegistryValueKind.String);
                reg.SetValue("EstimatedSize", new FileInfo(AppPath).Length / 1024, RegistryValueKind.DWord);
                reg.SetValue("InstallLocation", Path.GetDirectoryName(AppPath), RegistryValueKind.String);
                if (!(Version is null))
                    reg.SetValue("DisplayVersion", Version.ToString(), RegistryValueKind.String);
                if (!string.IsNullOrWhiteSpace(DisplayName))
                    reg.SetValue("DisplayName", DisplayName, RegistryValueKind.String);
                if (!string.IsNullOrWhiteSpace(IconPath))
                    reg.SetValue("DisplayIcon", IconPath, RegistryValueKind.String);
                if (!string.IsNullOrWhiteSpace(Configurator))
                    reg.SetValue("ModifyPath", Configurator, RegistryValueKind.String);
                if (!string.IsNullOrWhiteSpace(Uninstaller))
                    reg.SetValue("UninstallString", Uninstaller, RegistryValueKind.String);
            }
        }

        private void UnregistrationWindows()
        {
            GetUninstallPath().DeleteSubKey(Name, false);
        }

        private RegistryKey GetUninstallPath()
        {
            var path = new StringBuilder();
            path.Append(@"SOFTWARE\");
            if (!Environment.Is64BitProcess && Environment.Is64BitOperatingSystem) 
                path.Append("WOW6432Node\\");
            path.Append(@"Microsoft\Windows\CurrentVersion\Uninstall\");
            return Registry.LocalMachine.OpenSubKey(path.ToString(), true);
        }

        private void CreateWindowsService()
        {
            var cmd = new StringBuilder();
            cmd.Append("sc create {id} start= auto binpath= {path} displayname= {name}");
            cmd.Replace("{id}", Name);
            cmd.Replace("{path}", ServicePath);
            cmd.Replace("{name}", DisplayName);
            ExecCommandLine(cmd.ToString());
        }

        private void DeleteWindowsService()
        {
            var cmd = new StringBuilder();
            cmd.Append("sc stop {id} && sc delete {id}");
            cmd.Replace("{id}", Name);
            ExecCommandLine(cmd.ToString());
        }

        private void ExecCommandLine(string command)
        {
            var info = new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/q /c \"{command}\"",
                Verb = "runas",
                CreateNoWindow = true,
                UseShellExecute = true
            };
            Process.Start(info).WaitForExit();
        }
    }
}