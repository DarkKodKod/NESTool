using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using Nett;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace NESTool.FileSystem
{
    public static class ProjectItemFileSystem
    {
        private const string _metaExtensionKey = "extensionMetaFile";

        private static Dictionary<string, FileHandler> FileStructure = new Dictionary<string, FileHandler>();

        public static  void Initialize()
        {
            SignalManager.Get<RegisterFileHandlerSignal>().AddListener(OnRegisterFileHandler);
        }

        private static string GetMetaExtension()
        {
            return (string)Application.Current.FindResource(_metaExtensionKey);
        }
            
        private static void OnRegisterFileHandler(ProjectItem item)
        {
            FileHandler handler = new FileHandler();
            handler.Name = item.DisplayName;
            handler.Path = item.ParentFolder;

            item.FileHandler = handler;

            if (!item.IsFolder)
            {
                RegisterItemFile(item, handler);
            }

            RegisterMetaFile(item, handler);

            FileStructure.Add(item.FileHandler.Meta.GUID, handler);
        }

        private static void RegisterItemFile(ProjectItem item, FileHandler handler)
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

        private static void RegisterMetaFile(ProjectItem item, FileHandler handler)
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

        public static void CreateFolder(string name, string path)
        {
            string folderPath = Path.Combine(path, name);

            Directory.CreateDirectory(folderPath);

            CreateMetaFile(name, path);
        }

        public static void CreateFile(string name, string path, ProjectItemType type)
        {
            AFileModel model = Util.FileModelFactory(type);

            string filePath = Path.Combine(path, name + model.FileExtension);

            Toml.WriteFile(model, filePath);

            CreateMetaFile(name, path);

            //FileStructure.Add(file.Meta.GUID, file);
        }

        private static MetaFileModel CreateMetaFile(string name, string path)
        {
            MetaFileModel model = new MetaFileModel();

            string filePath = Path.Combine(path, name + model.FileExtension);

            Toml.WriteFile(model, filePath);

            return model;
        }

        public static void DeteFile(FileHandler filePtr)
        {
            if (FileStructure.TryGetValue(filePtr.Meta.GUID, out FileHandler outFile))
            {
                string metaPath = Path.Combine(filePtr.Path, filePtr.Name + filePtr.Meta.FileExtension);
                string itemPath = Path.Combine(filePtr.Path, filePtr.Name + filePtr.FileModel.FileExtension);

                if (File.Exists(metaPath))
                {
                    File.Delete(metaPath);
                }
                if (File.Exists(itemPath))
                {
                    File.Delete(itemPath);
                }
            }
        }
    }
}
