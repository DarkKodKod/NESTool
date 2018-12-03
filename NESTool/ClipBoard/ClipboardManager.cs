using System;
using System.ComponentModel;

namespace NESTool.Clipboard
{
    public static class ClipboardManager
    {
        private static string _data = null;

        public static object GetFromClipboard()
        {
            if (IsEmpty())
            {
                return null;
            }

            //https://stackoverflow.com/questions/5743243/convert-datetime-with-typedescriptor-getconverter-convertfromstring-using-custo

            Type type = Type.GetType("Namespace.MyClass, MyAssembly");

            TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
            return typeConverter.ConvertFromString(_data);
        }

        public static void CopyToClipboard(object obj)
        {
            Type type = obj.GetType();

            TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
            _data = typeConverter.ConvertToString(obj);
        }

        public static void Clear()
        {
            _data = "";
        }

        public static bool IsEmpty()
        {
            return string.IsNullOrEmpty(_data);
        }
    }
}
