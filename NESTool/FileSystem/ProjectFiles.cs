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
                if (handler.Value.FileModel is T model)
                {
                    models.Add(new FileModelVO()
                    {
                        Id = index++,
                        Name = handler.Value.Name,
                        Model = model,
                        Meta = handler.Value.Meta
                    });
                }
            }

            return models;
        }

        public static T GetModel<T>(string guid) where T : AFileModel
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            foreach (var handler in Handlers)
            {
                if (handler.Value.Meta.GUID == guid && 
                    handler.Value.FileModel is T model)
                {
                    return model;
                }
            }

            return null;
        }
    }
}
