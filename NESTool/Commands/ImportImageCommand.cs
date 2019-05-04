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
        private const string _folderImagesKey = "folderImages";

        public override bool CanExecute(object parameter)
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

        public override void Execute(object parameter)
        {
            object[] values = (object[])parameter;
            string filePath = (string)values[0];

            ProjectItem item = null;

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

            string imageFolderPath = Path.Combine(projectModel.ProjectPath, imagesFolder);

            if (!Directory.Exists(imageFolderPath))
            {
                Directory.CreateDirectory(imageFolderPath);
            }

            var quantizer = new PaletteQuantizer
            {
                InputFileName = filePath,
                ColorCache = PaletteQuantizer.EColorCache.OctreeSearch,
                Method = PaletteQuantizer.EMethod.NESQuantizer
            };

            Image outputImage = await quantizer.Convert();

            string outputImagePath = Path.Combine(imageFolderPath, item.DisplayName + ".bmp");

            tileSet.ImagePath = outputImagePath;
            tileSet.ImageWidth = outputImage.Width;
            tileSet.ImageHeight = outputImage.Height;

            item.FileHandler.Save();

            outputImage.Save(outputImagePath, ImageFormat.Bmp);

            SignalManager.Get<UpdateTileSetImageSignal>().Dispatch();
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
