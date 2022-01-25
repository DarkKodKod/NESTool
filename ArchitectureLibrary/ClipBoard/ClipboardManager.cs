using System;
using System.ComponentModel;

namespace ArchitectureLibrary.Clipboard
{
    public static class ClipboardManager
    {
        private static readonly ClipboardData _data = new ClipboardData();

        public static object GetData()
        {
            if (IsEmpty())
            {
                return null;
            }

            Type type = Type.GetType(_data.Type + ", " + _data.Assembly);

            TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
            return typeConverter.ConvertFromString(_data.Content);
        }

        public static void SetData(IClipboardable obj)
        {
            Type type = obj.GetType();

            TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
            _data.Assembly = type.Assembly.FullName.ToString();
            _data.Type = typeConverter.ConvertToString(obj);
            _data.Content = obj.GetContent();
        }

        public static void Clear()
        {
            _data.Clear();
        }

        public static bool IsEmpty()
        {
            return _data.IsEmpty();
        }
    }
}