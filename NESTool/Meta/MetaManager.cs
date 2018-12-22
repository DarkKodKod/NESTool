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

        public void Initialize()
        {
            SignalManager.Get<CreateMetaFileSignal>().AddListener(OnCreateMetaFile);
        }

        private void OnCreateMetaFile(MetaFileVO vo)
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

        public void DeleteMetaFile(string filePath)
        {
            // todo
        }

        public void MoveMetaFile(string filePath)
        {
            // todo
        }

        public void RenameMetaFile(string filePath)
        {
            // todo
        }
    }
}
