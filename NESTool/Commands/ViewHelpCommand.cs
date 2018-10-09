using ArchitectureLibrary.Commands;
using System.Diagnostics;

namespace NESTool.Commands
{
    public class ViewHelpCommand : Command
    {
        public override void Execute(object parameter)
        {
            Process.Start("https://github.com/DarkKodKod/NESTool");
        }
    }
}
