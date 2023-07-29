using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using Nett;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NESTool.FileSystem
{
    public static class ProjectItemFileSystem
    {
        public static void Initialize()
        {
            SignalManager.Get<RegisterFileHandlerSignal>().Listener += OnRegisterFileHandler;
            SignalManager.Get<RenameFileSignal>().Listener += OnRenameFile;
            SignalManager.Get<MoveElementSignal>().Listener += OnMoveElement;
        }

        private static void OnRenameFile(ProjectItem item)
        {
            if (item.FileHandler == null)
            {
                return;
            }

            FileHandler fileHandler = item.FileHandler;

            if (fileHandler.FileModel != null)
            {
                string itemPath = Path.Combine(fileHandler.Path, fileHandler.Name + fileHandler.FileModel.FileExtension);
                string itemNewPath = Path.Combine(fileHandler.Path, item.DisplayName + fileHandler.FileModel.FileExtension);

                if (File.Exists(itemPath))
                {
                    File.Move(itemPath, itemNewPath);
                }
            }
            else
            {
                // Is a folder
                string itemPath = Path.Combine(fileHandler.Path, fileHandler.Name);
                string itemNewPath = Path.Combine(fileHandler.Path, item.DisplayName);

                if (Directory.Exists(itemPath))
                {
                    Directory.Move(itemPath, itemNewPath);

                    UpdatePath(item, itemNewPath);
                }
            }

            fileHandler.Name = item.DisplayName;
        }

        private static async Task<AFileModel> ReadFileAndLoadModelAsync(string filePath, ProjectItemType type)
        {
            byte[] content = await ReadTextAsync(filePath).ConfigureAwait(false);

            return await ReadFileModel(type, content).ConfigureAwait(false);
        }

        private static async Task<byte[]> ReadTextAsync(string filePath)
        {
            byte[] result;

            using (FileStream sourceStream = File.Open(filePath, FileMode.Open))
            {
                result = new byte[sourceStream.Length];

                _ = await sourceStream.ReadAsync(result, 0, (int)sourceStream.Length).ConfigureAwait(false);
            }

            return result;
        }

        private static Task<AFileModel> ReadFileModel(ProjectItemType type, byte[] content)
        {
            switch (type)
            {
                case ProjectItemType.Bank:
                    return Task<AFileModel>.Factory.StartNew(() => Toml.ReadStream<BankModel>(new MemoryStream(content)));
                case ProjectItemType.Character:
                    return Task<AFileModel>.Factory.StartNew(() => Toml.ReadStream<CharacterModel>(new MemoryStream(content)));
                case ProjectItemType.Map:
                    return Task<AFileModel>.Factory.StartNew(() => Toml.ReadStream<MapModel>(new MemoryStream(content)));
                case ProjectItemType.TileSet:
                    return Task<AFileModel>.Factory.StartNew(() => Toml.ReadStream<TileSetModel>(new MemoryStream(content)));
                case ProjectItemType.Palette:
                    return Task<AFileModel>.Factory.StartNew(() => Toml.ReadStream<PaletteModel>(new MemoryStream(content)));
                case ProjectItemType.World:
                    return Task<AFileModel>.Factory.StartNew(() => Toml.ReadStream<WorldModel>(new MemoryStream(content)));
                case ProjectItemType.Entity:
                    return Task<AFileModel>.Factory.StartNew(() => Toml.ReadStream<EntityModel>(new MemoryStream(content)));
                case ProjectItemType.None:
                default:
                    return Task.FromResult<AFileModel>(null);
            }
        }

        private static async void OnRegisterFileHandler(ProjectItem item, string path)
        {
            FileHandler fileHandler = new FileHandler() { Name = item.DisplayName, Path = path };

            item.FileHandler = fileHandler;

            if (item.IsFolder)
            {
                return;
            }

            AFileModel model = Util.FileModelFactory(item.Type);

            if (model == null)
            {
                return;
            }

            string itemPath = Path.Combine(fileHandler.Path, fileHandler.Name + model.FileExtension);

            if (!File.Exists(itemPath))
            {
                return;
            }

            fileHandler.FileModel = await ReadFileAndLoadModelAsync(itemPath, item.Type);

            if (string.IsNullOrEmpty(fileHandler.FileModel.GUID))
            {
                fileHandler.FileModel.GUID = Guid.NewGuid().ToString();
            }

            if (ProjectFiles.Handlers.ContainsKey(fileHandler.FileModel.GUID))
            {
                return;
            }

            ProjectFiles.Handlers.Add(fileHandler.FileModel.GUID, fileHandler);

            SignalManager.Get<ProjectItemLoadedSignal>().Dispatch(fileHandler.FileModel.GUID);

            ProjectFiles.ObjectsLoading--;

            if (ProjectFiles.ObjectsLoading <= 0)
            {
                SignalManager.Get<FinishedLoadingProjectSignal>().Dispatch();
            }
        }

        public static string GetValidFolderName(string path, string name)
        {
            int counter = 1;
            string outName = name;

            string folderPath = Path.Combine(path, name);

            while (Directory.Exists(folderPath))
            {
                outName = name + "_" + counter;

                folderPath = Path.Combine(path, outName);

                counter++;
            }

            return outName;
        }

        public static string GetValidFileName(string path, string name, string extension)
        {
            int counter = 1;
            string outName = name;

            string filePath = Path.Combine(path, name + extension);

            while (File.Exists(filePath))
            {
                outName = name + "_" + counter;

                filePath = Path.Combine(path, outName + extension);

                counter++;
            }

            return outName;
        }

        public static void CreateElement(ProjectItem item, string path, string name)
        {
            if (item.IsFolder)
            {
                CreateFolderElement(item, path, name);
            }
            else
            {
                CreateFileElement(item, path, name);
            }
        }

        private static void CreateFolderElement(ProjectItem item, string path, string name)
        {
            string folderPath = Path.Combine(path, name);

            foreach (ProjectItem itm in item.Items)
            {
                if (itm.IsFolder)
                {
                    CreateFolderElement(itm, folderPath, itm.DisplayName);
                }
                else
                {
                    CreateFileElement(itm, folderPath, itm.DisplayName);
                }
            }

            CreateFileElement(item, path, name);
        }

        public static void CreateFileElement(ProjectItem item, string path, string name)
        {
            item.FileHandler = new FileHandler() { Name = name, Path = path };

            if (!item.IsFolder)
            {
                AFileModel model = Util.FileModelFactory(item.Type);

                if (model != null)
                {
                    string filePath = Path.Combine(path, name + model.FileExtension);

                    Toml.WriteFile(model, filePath);

                    item.FileHandler.FileModel = model;

                    ProjectFiles.Handlers.Add(item.FileHandler.FileModel.GUID, item.FileHandler);
                }
            }
            else
            {
                string folderPath = Path.Combine(path, name);

                _ = Directory.CreateDirectory(folderPath);
            }
        }

        private static void OnMoveElement(ProjectItem targetElement, ProjectItem draggedElement)
        {
            string destFolder;

            if (targetElement.IsFolder)
            {
                destFolder = Path.Combine(targetElement.FileHandler.Path, targetElement.FileHandler.Name);
            }
            else
            {
                destFolder = Path.Combine(targetElement.FileHandler.Path);
            }

            if (draggedElement.IsFolder)
            {
                string originalPath = Path.Combine(draggedElement.FileHandler.Path, draggedElement.FileHandler.Name);
                string destinationPath = Path.Combine(destFolder, draggedElement.FileHandler.Name);

                Directory.Move(originalPath, destinationPath);

                UpdatePath(draggedElement, destinationPath);
            }
            else
            {
                string fileName = draggedElement.FileHandler.Name + draggedElement.FileHandler.FileModel.FileExtension;

                string originalFile = Path.Combine(draggedElement.FileHandler.Path, fileName);
                string destinationFile = Path.Combine(destFolder, fileName);

                File.Move(originalFile, destinationFile);
            }

            draggedElement.FileHandler.Path = destFolder;
            draggedElement.FileHandler.Name = draggedElement.DisplayName;
        }

        private static void UpdatePath(ProjectItem rootElement, string destinationPath)
        {
            foreach (ProjectItem item in rootElement.Items)
            {
                item.FileHandler.Path = destinationPath;

                if (item.IsFolder)
                {
                    UpdatePath(item, Path.Combine(destinationPath, item.DisplayName));
                }
            }
        }

        private static void DeleteFolders(ProjectItem item)
        {
            foreach (ProjectItem itm in item.Items)
            {
                if (itm.IsFolder)
                {
                    DeleteFolders(itm);
                }
                else
                {
                    DeleteElement(itm.FileHandler);
                }
            }

            DeleteElement(item.FileHandler);
        }

        private static void DeleteElement(FileHandler fileHandler)
        {
            if (fileHandler.FileModel != null)
            {
                string itemPath = Path.Combine(fileHandler.Path, fileHandler.Name + fileHandler.FileModel.FileExtension);

                if (File.Exists(itemPath))
                {
                    File.Delete(itemPath);
                }

                _ = ProjectFiles.Handlers.Remove(fileHandler.FileModel.GUID);
            }
            else
            {
                // Is a folder
                string itemPath = Path.Combine(fileHandler.Path, fileHandler.Name);

                if (Directory.Exists(itemPath))
                {
                    Directory.Delete(itemPath, true);
                }
            }
        }

        // todo: when deleting a folder we have to iterate all the sub folders deleting everything!
        public static void DeteElement(ProjectItem item)
        {
            if (item.IsFolder)
            {
                DeleteFolders(item);
            }
            else
            {
                DeleteElement(item.FileHandler);
            }
        }
    }
}
