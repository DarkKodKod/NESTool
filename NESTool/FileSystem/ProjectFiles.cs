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

            foreach (KeyValuePair<string, FileHandler> pair in Handlers)
            {
                if (pair.Value.FileModel is T model)
                {
                    models.Add(new FileModelVO()
                    {
                        Index = index++,
                        Name = pair.Value.Name,
                        Model = model
                    });
                }
            }

            return models;
        }

		public static void SaveModel<T>(string guid, T model) where T : AFileModel
		{
			if (string.IsNullOrEmpty(guid))
			{
				return;
			}

			foreach (KeyValuePair<string, FileHandler> pair in Handlers)
			{
				if (pair.Key == guid)
				{
					pair.Value.FileModel = model;
					pair.Value.Save();
				}
			}
		}

		public static FileModelVO GetFileModel(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            foreach (KeyValuePair<string, FileHandler> pair in Handlers)
            {
                if (pair.Key == guid)
                {
                    return new FileModelVO()
                    {
                        Index = 0,
                        Name = pair.Value.Name,
                        Model = pair.Value.FileModel
                    };
                }
            }

            return null;
        }

        public static T GetModel<T>(string guid) where T : AFileModel
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            foreach (KeyValuePair<string, FileHandler> pair in Handlers)
            {
                if (pair.Key == guid &&
					pair.Value.FileModel is T model)
                {
                    return model;
                }
            }

            return null;
        }
    }
}
