﻿using NESTool.FileSystem;
using NESTool.History;
using System.Windows;

namespace NESTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ProjectItemFileSystem.Initialize();
            HistoryManager.Initialize();
        }
    }
}
