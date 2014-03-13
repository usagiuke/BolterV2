using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace ConfigHelper
{
    public class XmlSerializationHelper
    {
        public static void Serialize<T>(string filename, T obj)
        {
            var xs = new XmlSerializer(typeof(T));

            var dir = Path.GetDirectoryName(filename);

            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var sw = new StreamWriter(filename))
            {
                xs.Serialize(sw, obj);
            }
        }

        public static T Deserialize<T>(string filename)
        {
            try
            {
                var xs = new XmlSerializer(typeof(T));

                using (var rd = new StreamReader(filename))
                {
                    return (T)xs.Deserialize(rd);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return default(T);
            }
        }
    }
    [Serializable]
    public class config
    {
        public List<PastProcess> MemInfo = new List<PastProcess>();
    }

    [Serializable]
    public struct PastProcess
    {
        [XmlIgnore]
        public IntPtr hModule
        {
            get { return (IntPtr)_hModule; }
            set { _hModule = (int)value; }
        }
        [XmlIgnore]
        public IntPtr IsLoadedPtr
        {
            get { return (IntPtr)_IsLoadedPtr; }
            set { _IsLoadedPtr = (int)value; }
        }
        [XmlAttribute("ID")]
        public int ID;
        [XmlAttribute("hModule"), EditorBrowsable(EditorBrowsableState.Never)]
        public int _hModule;
        [XmlAttribute("ComPtr"), EditorBrowsable(EditorBrowsableState.Never)]
        public int _IsLoadedPtr;
    }
}