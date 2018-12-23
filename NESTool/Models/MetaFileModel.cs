﻿using Nett;
using System;
using System.Windows;

namespace NESTool.Models
{
    public class MetaFileModel : AFileModel
    {
        private const string _extensionKey = "extensionMetaFile";

        [TomlIgnore]
        public override string FileExtension
        {
            get
            {
                if (string.IsNullOrEmpty(_fileExtension))
                {
                    _fileExtension = (string)Application.Current.FindResource(_extensionKey);
                }

                return _fileExtension;
            }
        }

        public MetaFileModel()
        {
            GUID = Guid.NewGuid().ToString();
        }

        public string GUID { get; set; }
    }
}
