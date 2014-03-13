using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace BolterLibrary
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
    public class Waypoints
    {
        [XmlElement("Zone")]
        public List<Zones> Zone = new List<Zones>();

        public void AddPathToZone(int zoneId, string pathName)
        {
            Zone.First(p => p.ID == zoneId).Path.Add(new Paths(pathName));
        }

    }

    [Serializable]
    public class Zones
    {
        public Zones()
        {

        }
        public Zones(int id)
        {
            ID = id;
            Name = Funcs.GetZoneName();
        }

        public void AddPoints(string pathName, int interval, dynamic log = null)
        {
            if (Path.All(p => p.Name != pathName))
            {
                Path.Add(new Paths(pathName));
                Path.Last().AddPoints(interval, log);
            }
            else
                Path.First(p => p.Name == pathName).AddPoints(interval, log);
        }

        [XmlAttribute("ID")]
        public int ID;
        [XmlAttribute("Name")]
        public string Name;
        [XmlElement("Path")]
        public List<Paths> Path = new List<Paths>();
    }

    [Serializable]
    public class Paths
    {
        public Paths()
        {

        }

        public Paths(string pathName)
        {
            Name = pathName;
        }

        public void AddPoints(int interval, dynamic log = null)
        {
            var lastHeading = 0f;
            // Save our current heading.
            if (Navigation.TurnFilter)
                lastHeading = Funcs.GetHeading(EntityType.PCMob, 0);

            // Loop while record flag is true.
            while (Navigation.RecordFlag)
            {
                // Get current position.
                var currentPos = AllEntities.Get2DPos();

                // Get last saved position, or null if there are none.
                var lastPos = Point.LastOrDefault();

                // Check if this is a new entry, or if we have moved from our last position.
                if (lastPos == null || lastPos.x != currentPos.x || lastPos.y != currentPos.y)
                {
                    if (Navigation.TurnFilter && (lastHeading == Funcs.GetHeading(EntityType.PCMob, 0)))
                    {
                        Thread.Sleep(interval);
                        continue;
                    }
                    // Add the saved waypoint information to the log.
                    if (log != null)
                        log.AppendTextR(string.Format("{0} {1} {2} {3} {4}\u2028", Point.Count, Funcs.GetZoneName(), Name, currentPos.x, currentPos.y));

                    // Add the new waypoint.
                    Point.Add(new D3DXVECTOR2(currentPos.x, currentPos.y));

                    // Save out last heading
                    if (Navigation.TurnFilter)
                        lastHeading = Funcs.GetHeading(EntityType.PCMob, 0);
                }
                // End if this is a single add.
                if (interval == 0)
                    return;
                // Hold for the given interval.
                Thread.Sleep(interval);
            }


        }

        [XmlAttribute("Name")]
        public string Name;
        [XmlElement("Point")]
        public List<D3DXVECTOR2> Point = new List<D3DXVECTOR2>();
    }
}
