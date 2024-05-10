using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.Building;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NESTool.Commands;

public class BuildProjectCommand : Command
{
    private bool _building = false;

    public override bool CanExecute(object? parameter)
    {
        return !_building;
    }

    public override async void Execute(object? parameter)
    {
        if (_building)
        {
            return;
        }

        _building = true;

        if (!CheckValidOutputFolder())
        {
            OutputError("Invalid output folder");

            _building = false;
            return;
        }

        OutputInfo("Build started");

        List<Task> tasks = [];

        OutputInfo("Building banks...");
        Task task = Task.Factory.StartNew(() => { BanksBuilding.Execute(); });
        tasks.Add(task);

        OutputInfo("Building tiles definitions...");
        task = Task.Factory.StartNew(() => { TilesDefinitionsBuilding.Execute(); });
        tasks.Add(task);

        OutputInfo("Building backgrounds...");
        task = Task.Factory.StartNew(() => { BackgroundsBuilding.Execute(); });
        tasks.Add(task);

        OutputInfo("Building meta sprites...");
        task = Task.Factory.StartNew(() => { MetaSpritesBuilding.Execute(); });
        tasks.Add(task);

        OutputInfo("Building palettes...");
        task = Task.Factory.StartNew(() => { PalettesBuilding.Execute(); });
        tasks.Add(task);

        await Task.WhenAll(tasks.ToArray());

        OutputInfo("Build completed", "Green");

        _building = false;

        RaiseCanExecuteChanged();
    }

    private bool CheckValidOutputFolder()
    {
        ProjectModel projectModel = ModelManager.Get<ProjectModel>();

        try
        {
            string result = Path.GetFullPath(projectModel.Build.OutputFilePath);

            return Directory.Exists(result);
        }
        catch
        {
            return false;
        }
    }

    private void OutputInfo(string message, string color = "")
    {
        SignalManager.Get<WriteBuildOutputSignal>().Dispatch(message, OutputMessageType.Information, color);
    }

    private void OutputError(string message)
    {
        SignalManager.Get<WriteBuildOutputSignal>().Dispatch(message, OutputMessageType.Error, "");
    }
}
