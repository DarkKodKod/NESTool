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
using System.Windows;

namespace NESTool.Commands
{
    public class ImportImageCommand : Command
    {
        private const string _folderTileSetsKey = "folderTileSets";

        public override async void Execute(object parameter)
        {
            string filePath = parameter as string;

            string name = Path.GetFileNameWithoutExtension(filePath);

            ProjectItem item = CreateTileSetElement(name);

            if (item.FileHandler.FileModel is TileSetModel tileSet)
            {
                // todo. create dictory for the new image output file

                tileSet.Image = filePath;

                // todo. link the new image in the tileset element


                var quantizer = new PaletteQuantizer();
                quantizer.FileName = filePath;
                quantizer.ColorCount = PaletteQuantizer.EColor.Color4;
                quantizer.ColorCache = PaletteQuantizer.EColorCache.OctreeSearch;
                Image gato = await quantizer.Convert();
                ProjectModel projectModel = ModelManager.Get<ProjectModel>();
                gato.Save(Path.Combine(projectModel.ProjectPath, "gato.png"), ImageFormat.Png);



                item.FileHandler.Save();
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
}
