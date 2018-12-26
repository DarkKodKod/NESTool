using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using Nett;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace NESTool.FileSystem
{
    public static class ProjectItemFileSystem
    {
        private const string _metaExtensionKey = "extensionMetaFile";

        private static string _metaExtension = "";
        private static Dictionary<string, FileHandler> FileStructure = new Dictionary<string, FileHandler>();

        public static  void Initialize()
        {
            SignalManager.Get<RegisterFileHandlerSignal>().AddListener(OnRegisterFileHandler);
        }

        private static string GetMetaExtension()
        {
            if (string.IsNullOrEmpty(_metaExtension))
            {
                _metaExtension = (string)Application.Current.FindResource(_metaExtensionKey);
            }

            return _metaExtension;
        }
            
        private static void OnRegisterFileHandler(ProjectItem item)
        {
            item.FileHandler = new FileHandler() { Name = item.DisplayName, Path = item.ParentFolder };

            if (!item.IsFolder)
            {
                RegisterItemFile(ref item);
            }

            RegisterMetaFile(ref item);

            FileStructure.Add(item.FileHandler.Meta.GUID, item.FileHandler);
        }

        private static void RegisterItemFile(ref ProjectItem item)
        {
            AFileModel model = Util.FileModelFactory(item.Type);

            string itemPath = Path.Combine(item.ParentFolder, item.DisplayName + model.FileExtension);

            if (File.Exists(itemPath))
            {
                switch (item.Type)
                {
                    case ProjectItemType.Bank:
                        item.FileHandler.FileModel = Toml.ReadFile<BankModel>(itemPath);
                        break;
                    case ProjectItemType.Character:
                        item.FileHandler.FileModel = Toml.ReadFile<CharacterModel>(itemPath);
                        break;
                    case ProjectItemType.Map:
                        item.FileHandler.FileModel = Toml.ReadFile<MapModel>(itemPath);
                        break;
                    case ProjectItemType.TileSet:
                        item.FileHandler.FileModel = Toml.ReadFile<TileSetModel>(itemPath);
                        break;
                    case ProjectItemType.PatternTable:
                        item.FileHandler.FileModel = Toml.ReadFile<PatternTableModel>(itemPath);
                        break;
                }
            }
        }

        private static void RegisterMetaFile(ref ProjectItem item)
        {
            string metaPath = Path.Combine(item.ParentFolder, item.DisplayName + GetMetaExtension());

            if (File.Exists(metaPath))
            {
                item.FileHandler.Meta = Toml.ReadFile<MetaFileModel>(metaPath);
            }
            else
            {
                item.FileHandler.Meta = CreateMetaFile(item.DisplayName, item.ParentFolder);
            }
        }

        public static void CreateFileElement(ref ProjectItem item)
        {
            item.FileHandler = new FileHandler() { Name = item.DisplayName, Path = item.ParentFolder };

            if (!item.IsFolder)
            {
                AFileModel model = Util.FileModelFactory(item.Type);

                string filePath = Path.Combine(item.ParentFolder, item.DisplayName + model.FileExtension);

                Toml.WriteFile(model, filePath);

                item.FileHandler.FileModel = model;
            }
            else
            {
                string folderPath = Path.Combine(item.ParentFolder, item.DisplayName);

                Directory.CreateDirectory(folderPath);
            }

            MetaFileModel metaModel = CreateMetaFile(item.DisplayName, item.ParentFolder);

            item.FileHandler.Meta = metaModel;

            FileStructure.Add(item.FileHandler.Meta.GUID, item.FileHandler);
        }

        private static MetaFileModel CreateMetaFile(string name, string path)
        {
            MetaFileModel model = new MetaFileModel();

            string filePath = Path.Combine(path, name + model.FileExtension);

            Toml.WriteFile(model, filePath);

            return model;
        }

        public static void DeteFile(FileHandler fileHandler)
        {
            if (fileHandler.Meta == null)
            {
                return;
            }

            if (FileStructure.TryGetValue(fileHandler.Meta.GUID, out FileHandler outFile))
            {
                string metaPath = Path.Combine(fileHandler.Path, fileHandler.Name + fileHandler.Meta.FileExtension);

                if (File.Exists(metaPath))
                {
                    File.Delete(metaPath);
                }

                if (fileHandler.FileModel != null)
                {
                    string itemPath = Path.Combine(fileHandler.Path, fileHandler.Name + fileHandler.FileModel.FileExtension);

                    if (File.Exists(itemPath))
                    {
                        File.Delete(itemPath);
                    }
                }
                else
                {
                    // Is a folder
                    string itemPath = Path.Combine(fileHandler.Path, fileHandler.Name);

                    if (Directory.Exists(itemPath))
                    {
                        Directory.Delete(itemPath);
                    }
                }

                FileStructure.Remove(fileHandler.Meta.GUID);
            }
        }
    }
}
