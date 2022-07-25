using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Signals;
using NESTool.UserControls;
using NESTool.Utils;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for BuildProject.xaml
    /// </summary>
    public partial class BuildProjectDialog : Window, ICleanable
    {
        public BuildProjectDialog()
        {
            InitializeComponent();

            #region Signals
            SignalManager.Get<WriteBuildOutputSignal>().Listener += OnWriteBuildOutput;
            #endregion
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowUtility.RemoveIcon(this);
        }

        private void OnWriteBuildOutput(string newLine, OutputMessageType messageType)
        {
            BrushConverter bc = new BrushConverter();

            TextRange tr = new TextRange(tbOutput.Document.ContentEnd, tbOutput.Document.ContentEnd)
            {
                Text = newLine + Environment.NewLine
            };

            string color = "";
            switch (messageType)
            {
                case OutputMessageType.Information: color = "Black"; break;
                case OutputMessageType.Warning: color = "Yellow"; break;
                case OutputMessageType.Error: color = "Red"; break;
            }

            tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertFromString(color));

            tbOutput.ScrollToEnd();
        }

        public void CleanUp()
        {
            #region Signals
            SignalManager.Get<WriteBuildOutputSignal>().Listener -= OnWriteBuildOutput;
            #endregion
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CleanUp();
        }
    }
}
