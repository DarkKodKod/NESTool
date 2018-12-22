using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;
using NESTool.VOs;
using Nett;
using System.IO;

namespace NESTool.Meta
{
    public sealed class MetaManager
    {
        public static MetaManager Instance { get; } = new MetaManager();

        private const string FileExtension = ".meta";

        private MetaManager()
        {
        }

        public void Initialize()
        {
            SignalManager.Get<CreateMetaFileSignal>().AddListener(OnCreateMetaFile);
            SignalManager.Get<DeleteMetaFileSignal>().AddListener(OnDeleteMetaFile);
            SignalManager.Get<MoveMetaFileSignal>().AddListener(OnDeleteMetaFile);
            SignalManager.Get<RenameMetaFileSignal>().AddListener(OnDeleteMetaFile);
        }

        private void OnCreateMetaFile(FileHandleVO vo)
        {
            var model = new MetaItemModel()
            {
                GUID = System.Guid.NewGuid().ToString(),
                Path = vo.Path,
                Name = vo.Name
            };

            CreateMetaFile(model);
        }

        public void CreateMetaFile(MetaItemModel model)
        {
            string metaFilePath = Path.Combine(model.Path, model.Name + FileExtension);

            Toml.WriteFile(model, metaFilePath);
        }

        public void OnDeleteMetaFile()
        {
            // todo
        }

        public void OnMoveMetaFile()
        {
            // todo
        }

        public void OnRenameMetaFile()
        {
            // todo
        }
    }
}
