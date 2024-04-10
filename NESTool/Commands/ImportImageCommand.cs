using ArchitectureLibrary.Commands;
using ArchitectureLibrary.History.Signals;
using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.HistoryActions;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;
using System.Windows;

namespace NESTool.Commands;

[SupportedOSPlatform("windows")]
public class ImportImageCommand : Command
{
    private const string _folderTileSetsKey = "folderTileSets";
    private const string _folderImagesKey = "folderImages";

    public override bool CanExecute(object? parameter)
    {
        if (parameter == null)
        {
            return false;
        }

        object[] values = (object[])parameter;
        string filePath = (string)values[0];

        if (string.IsNullOrEmpty(filePath))
        {
            return false;
        }

        if (!File.Exists(filePath))
        {
            return false;
        }

        return true;
    }

    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        object[] values = (object[])parameter;
        string filePath = (string)values[0];

        ProjectItem? item = null;

        if (values.Length > 1)
        {
            item = (ProjectItem)values[1];
        }

        if (item == null)
        {
            string name = Path.GetFileNameWithoutExtension(filePath);

            item = CreateTileSetElement(name);
        }

        if (item.FileHandler.FileModel is TileSetModel tileSet)
        {
            ProcessImage(item, tileSet, filePath);
        }
    }

    private async void ProcessImage(ProjectItem item, TileSetModel tileSet, string filePath)
    {
        string imagesFolder = (string)Application.Current.FindResource(_folderImagesKey);

        ProjectModel projectModel = ModelManager.Get<ProjectModel>();

        string imageFolderFullPath = Path.Combine(projectModel.ProjectPath, imagesFolder);

        if (!Directory.Exists(imageFolderFullPath))
        {
            _ = Directory.CreateDirectory(imageFolderFullPath);
        }

        PaletteQuantizer quantizer = new PaletteQuantizer
        {
            InputFileName = filePath,
            ColorCache = PaletteQuantizer.EColorCache.OctreeSearch,
            Method = PaletteQuantizer.EMethod.NESQuantizer
        };

        // todo: If the source image is the same as the output image, Crash!!
        Image? outputImage = await quantizer.Convert();

        string outputImagePath = Path.Combine(imageFolderFullPath, item.DisplayName + ".bmp");

        tileSet.ImagePath = Path.Combine(imagesFolder, item.DisplayName + ".bmp");

        if (outputImage != null)
        {
            tileSet.ImageWidth = outputImage.Width;
            tileSet.ImageHeight = outputImage.Height;

            item.FileHandler.Save();

            outputImage.Save(outputImagePath, ImageFormat.Bmp);

            SignalManager.Get<UpdateTileSetImageSignal>().Dispatch();
        }
    }

    private ProjectItem CreateTileSetElement(string name)
    {
        ProjectModel projectModel = ModelManager.Get<ProjectModel>();

        string tileSets = (string)Application.Current.FindResource(_folderTileSetsKey);

        string path = Path.Combine(projectModel.ProjectPath, tileSets);

        name = ProjectItemFileSystem.GetValidFileName(
            path,
            name,
            Util.GetExtensionByType(ProjectItemType.TileSet));

        ProjectItem newElement = new ProjectItem()
        {
            DisplayName = name,
            IsFolder = false,
            IsRoot = false,
            Type = ProjectItemType.TileSet
        };

        SignalManager.Get<RegisterHistoryActionSignal>().Dispatch(new CreateNewElementHistoryAction(newElement));

        SignalManager.Get<FindAndCreateElementSignal>().Dispatch(newElement);

        ProjectItemFileSystem.CreateFileElement(newElement, path, name);

        return newElement;
    }
}
