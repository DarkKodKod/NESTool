using NESTool.Models;
using Nett;

namespace NESTool.Meta
{
    public static class MetaManager
    {
        public static void DeleteMetaFile(string filePath)
        {
            //
        }

        public static void CreateMetaFile(string filePath)
        {
            MetaItemModel model = new MetaItemModel();

            model.GUID = System.Guid.NewGuid().ToString();

            Toml.WriteFile(model, filePath);
        }

        public static void MoveMetaFile(string filePath)
        {
            //
        }

        public static void RenameMetaFile(string filePath)
        {
            //
        }
    }
}
