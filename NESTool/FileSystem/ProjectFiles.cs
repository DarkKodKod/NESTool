using NESTool.Models;
using NESTool.VOs;
using System.Collections.Generic;

namespace NESTool.FileSystem
{
    public static class ProjectFiles
    {
        public static Dictionary<string, FileHandler> Handlers = new Dictionary<string, FileHandler>();

        public static List<FileModelVO> GetModels<T>() where T : AFileModel
        {
            List<FileModelVO> models = new List<FileModelVO>();

            int index = 0;

            foreach (var handler in Handlers)
            {
                if (handler.Value.FileModel is T)
                {
                    models.Add(new FileModelVO()
                    {
                        Id = index++,
                        Name = handler.Value.Name,
                        Model = handler.Value.FileModel as T,
                        Meta = handler.Value.Meta
                    });
                }
            }

            return models;
        }
    }
}
