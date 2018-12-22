﻿using NESTool.Enums;

namespace NESTool.Models
{
    public class MetaItemModel
    {
        public string GUID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public ProjectItemType Type { get; set; }
    }
}
