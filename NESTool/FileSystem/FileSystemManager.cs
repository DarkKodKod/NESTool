using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;
using NESTool.VOs;
using Nett;
using System.IO;

namespace NESTool.FileSystem
{
    public sealed class FileSystemManager
    {
        public static FileSystemManager Instance { get; } = new FileSystemManager();

        private FileSystemManager()
        {
        }

        public void Initialize()
        {
            SignalManager.Get<CreateFileSignal>().AddListener(OnCreateMetaFile);
            SignalManager.Get<DeleteFileSignal>().AddListener(OnDeleteMetaFile);
            SignalManager.Get<MoveFileSignal>().AddListener(OnMoveMetaFile);
            SignalManager.Get<RenameFileSignal>().AddListener(OnRenameMetaFile);
        }

        private void OnCreateMetaFile(FileHandleVO vo)
        {
            string metaFilePath = Path.Combine(vo.Path, vo.Name + vo.Model.FileExtension);

            Toml.WriteFile(vo.Model, metaFilePath);
        }

        public void OnDeleteMetaFile(FileHandleVO vo)
        {
            // todo
        }

        public void OnMoveMetaFile(FileHandleVO vo)
        {
            // todo
        }

        public void OnRenameMetaFile(FileHandleVO vo)
        {
            // todo
        }
    }
}
