using ArchitectureLibrary.History;
using NESTool.FileSystem;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;

namespace NESTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ProjectItemFileSystem.Initialize();
            HistoryManager.Initialize();
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = new AssemblyName(args.Name);

            string path = assemblyName.Name + ".dll";

            if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false)
            {
                path = string.Format(@"{0}\{1}", assemblyName.CultureInfo, path);
            }

            using (Stream stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                {
                    return null;
                }

                byte[] assemblyRawBytes = new byte[stream.Length];
                _ = stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);

                return Assembly.Load(assemblyRawBytes);
            }
        }
    }
}
