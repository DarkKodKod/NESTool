using ArchitectureLibrary.Commands;
using System.Diagnostics;

namespace NESTool.Commands;

public class OpenLinkCommand : Command
{
    public override void Execute(object? parameter)
    {
        string? url = parameter as string;

        if (url != null)
        {
            Process.Start(url);
        }
    }
}
