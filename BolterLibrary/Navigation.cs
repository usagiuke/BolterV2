using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BolterLibrary
{
    public enum Pathing
    {
        Normal,
        At_Index,
        Closest
    }
    public class Navigation : NativeDx
    {
        public Navigation()
        {
            var filepath = InterProcessCom.ConfigPath + "\\waypoints.xml";
            _Waypoints = !File.Exists(filepath) ? new Waypoints() : XmlSerializationHelper.Deserialize<Waypoints>(filepath);
        }

        /// <summary>
        /// Container for our waypoints.xml
        /// </summary>
        public static Waypoints _Waypoints;

        /// <summary>
        /// Flag that stops and starts recording
        /// </summary>
        public static bool RecordFlag;

        public static bool HaltFlag;

        public static int HeadToll = 50;

        public static bool Moving;

        public static bool CorDelay = false;

        public static bool TurnFilter = true;

        public static bool AICorrection = true;

        public static bool CamReset = false;

        /// <summary>
        /// A recipe for PI
        /// </summary>
        private readonly float _pi = (float)(4 * Math.Atan(1));


        protected float Distance(D3DXVECTOR2 p1, D3DXVECTOR2 p2) { return Distance(p1.y, p1.x, p2.y, p2.x); }
        public static float Distance(float y1, float x1, float y2, float x2) { return (float)Math.Sqrt(((y1 - y2) * (y1 - y2)) + ((x1 - x2) * (x1 - x2))); }
        protected int MathMod(int a, int b) { return ((a % b) + b) % b; }
        protected double PosHToDegrees(float PosH) { return MathMod((int)(((PosH * 180.0) / _pi) + 90.0), 360); }
        protected float DegreesToPosH(double Degrees) { return (MathMod((int)(((Degrees + 90) * _pi) / 180.0), (int)(2 * _pi)) - _pi); }
        protected float HeadingToRad(D3DXVECTOR2 from, D3DXVECTOR2 to) { return (float)Math.Atan2((to.x - from.x), (to.y - from.y)); }

        /// <summary>
        /// Rotates the character's model, to face the given destination point.
        /// </summary>
        /// <param name="pointB">The destination point</param>
        protected void ModelRotation(D3DXVECTOR2 pointB)
        {
            // Get new heading angle, for the given point.
            var newHeading = HeadingToRad(AllEntities.Get2DPos(), pointB);

            // Get new 3D matrix Y axis vector, rotated to the given angle.
            var newVector = GetNewVector(newHeading);

            // Set new heading
            Funcs.SetHeading(EntityType.PCMob, 0, newHeading);

            // Set new rotation vector.
            Funcs.Set3DVector(EntityType.PCMob, Axis.X, 0, newVector.x);
            Funcs.Set3DVector(EntityType.PCMob, Axis.Y, 0, newVector.y);
        }

        private void RecordWaypoint(int interval, string pathName, dynamic logBox = null)
        {
            if (RecordFlag)
                return;
            // Set recording as true.
            RecordFlag = true;

            // Get ID of current zone.
            var zoneId = Funcs.GetZoneId();

            // Check if we need to make a new Zone entry.
            var nozones = _Waypoints.Zone.All(p => p.ID != zoneId);

            // We need to make a new entry.
            if (nozones)
            {
                // Add new Zone entry for our current zone.
                _Waypoints.Zone.Add(new Zones(zoneId));

                // Add an empty Path entry.
                _Waypoints.AddPathToZone(zoneId, pathName);

                // This is a new entry, so grab the last Zone we added, and the first Path in that Zone,
                // Then start adding new Point entries (at the rate of the given interval), and wait for the user to click stop.
                _Waypoints.Zone.Last().Path.First().AddPoints(interval, logBox);
            }
            else
                // We already have the Zone in our xml, so lets go strait to adding points.
                _Waypoints.Zone.First(p => p.ID == zoneId).AddPoints(pathName, interval, logBox);

            // If this is a single add, just reset the record flag and save the xml.
            if (interval == 0)
                StopRecord();
        }

        private void PlayPath(string pathName, Pathing pType, bool forawrd, int index, dynamic log = null)
        {
            if (Moving)
                return;
            Moving = true;

            var zoneId = Funcs.GetZoneId();

            HaltFlag = false;
            float heading;
            float tobeHeading;

            foreach (
                var waypoint in
                    RebuildList(_Waypoints.Zone.First(p => p.ID == zoneId).Path.First(i => i.Name == pathName).Point,
                        forawrd, pType, index))
            {
                if (CorDelay)
                    Funcs.SetMoveStatus(WalkingStatus.Standing);

                ModelRotation(waypoint);

                if (CamReset)
                    SendKeyPress(KeyStates.Toggled, Key.End);

                if (CorDelay)
                    Thread.Sleep(HeadToll);

                var decX = (Funcs.GetPOS(EntityType.PCMob, Axis.X, 0) > waypoint.x);

                Funcs.SetMoveStatus(WalkingStatus.Autorun | WalkingStatus.Running);

                if (decX)
                {
                    while (Funcs.GetPOS(EntityType.PCMob, Axis.X, 0) > waypoint.x)
                    {
                        if (HaltFlag)
                        {
                            Funcs.SetMoveStatus(WalkingStatus.Standing);
                            Moving = false;
                            return;
                        }
                        if (AICorrection)
                        {
                            heading = Funcs.GetHeading(EntityType.PCMob, 0);

                            tobeHeading = HeadingToRad(AllEntities.Get2DPos(), waypoint);

                            // Check if our heading is within our tolerance.
                            if (tobeHeading - heading < 0 ? tobeHeading - heading < -0.1f : tobeHeading - heading > 0.1f)
                                ModelRotation(waypoint);
                        }
                        Thread.Sleep(10);
                    }
                }
                else
                {
                    while (Funcs.GetPOS(EntityType.PCMob, Axis.X, 0) < waypoint.x)
                    {
                        if (HaltFlag)
                        {
                            Funcs.SetMoveStatus(WalkingStatus.Standing);
                            Moving = false;
                            return;
                        }
                        if (AICorrection)
                        {
                            heading = Funcs.GetHeading(EntityType.PCMob, 0);
                            tobeHeading = HeadingToRad(AllEntities.Get2DPos(), waypoint);

                            // Check if our heading is within our tolerance.
                            if (tobeHeading - heading < 0 ? tobeHeading - heading < -0.1f : tobeHeading - heading > 0.1f)
                                ModelRotation(waypoint);
                        }
                        Thread.Sleep(10);
                    }
                }
                if (log != null)
                    log.AppendTextP(string.Format("{0} {1} {2}\u2028", pathName, waypoint.x, waypoint.y));
            }
            Funcs.SetMoveStatus(WalkingStatus.Standing);
            Moving = false;
        }

        private int GetClosestIndex(List<D3DXVECTOR2> PList, D3DXVECTOR2 curPos)
        {
            var n = 0;

            PList.ForEach(p => n = Distance(curPos, p) < Distance(curPos, PList[n]) ? PList.IndexOf(p) : n);

            return n;
        }

        private IEnumerable<D3DXVECTOR2> RebuildList(IEnumerable<D3DXVECTOR2> pList, bool forward, Pathing pType, int index = 0)
        {
            var rPlist = new List<D3DXVECTOR2>(pList);

            if (!forward)
                rPlist.Reverse();

            int localIndex;

            if (pType == Pathing.Normal)
                localIndex = 0;
            else
                localIndex = pType == Pathing.At_Index ? index : GetClosestIndex(rPlist, AllEntities.Get2DPos());

            return rPlist.Skip(localIndex);
        }

        /// <summary>
        /// Starts recording waypoints on a new thread.
        /// </summary>
        /// <param name="interval">Time in ms, between waypoints</param>
        /// <param name="pathName">Name of the path to save the waypoints to. A new on will be created, if it does not exist</param>
        /// <param name="logBox"></param>
        public void Record(int interval, string pathName, dynamic log = null)
        {
            new Thread(() => RecordWaypoint(interval, pathName, log)).Start();
        }

        public void Play(string pathName, Pathing pType, bool forward = true, int index = 0, dynamic log = null)
        {
            new Thread(() => PlayPath(pathName, pType, forward, index, log)).Start();
        }

        public void Walk(float x, float y, bool KeepWalking)
        {
            if (Moving)
                return;
            Moving = true;

            HaltFlag = false;
            float heading;
            float tobeHeading;
            var waypoint = new D3DXVECTOR2(x, y);
            if (CorDelay)
                Funcs.SetMoveStatus(WalkingStatus.Standing);

            ModelRotation(waypoint);

            if (CamReset)
                SendKeyPress(KeyStates.Toggled, Key.End);

            if (CorDelay)
                Thread.Sleep(HeadToll);

            var decX = (Funcs.GetPOS(EntityType.PCMob, Axis.X, 0) > x);

            Funcs.SetMoveStatus(WalkingStatus.Autorun | WalkingStatus.Running);

            if (decX)
            {
                while (Funcs.GetPOS(EntityType.PCMob, Axis.X, 0) > x)
                {
                    if (HaltFlag)
                    {
                        Funcs.SetMoveStatus(WalkingStatus.Standing);
                        Moving = false;
                        return;
                    }
                    if (AICorrection)
                    {
                        heading = Funcs.GetHeading(EntityType.PCMob, 0);

                        tobeHeading = HeadingToRad(AllEntities.Get2DPos(), waypoint);

                        // Check if our heading is within our tolerance.
                        if (tobeHeading - heading < 0 ? tobeHeading - heading < -0.1f : tobeHeading - heading > 0.1f)
                            ModelRotation(waypoint);
                    }
                    Thread.Sleep(10);
                }
            }
            else
            {
                while (Funcs.GetPOS(EntityType.PCMob, Axis.X, 0) < x)
                {
                    if (HaltFlag)
                    {
                        Funcs.SetMoveStatus(WalkingStatus.Standing);
                        Moving = false;
                        return;
                    }
                    if (AICorrection)
                    {
                        heading = Funcs.GetHeading(EntityType.PCMob, 0);
                        tobeHeading = HeadingToRad(AllEntities.Get2DPos(), waypoint);

                        // Check if our heading is within our tolerance.
                        if (tobeHeading - heading < 0 ? tobeHeading - heading < -0.1f : tobeHeading - heading > 0.1f)
                            ModelRotation(waypoint);
                    }
                    Thread.Sleep(10);
                }
            }
            if (!KeepWalking)
                Funcs.SetMoveStatus(WalkingStatus.Standing);
            Moving = false;
        }

        /// <summary>
        /// Stops an active recording session, and saves the results to xml.
        /// </summary>
        public void StopRecord()
        {
            RecordFlag = false;
            XmlSerializationHelper.Serialize(InterProcessCom.ConfigPath + "\\waypoints.xml", _Waypoints);
        }

        public void Reload()
        {
            XmlSerializationHelper.Serialize(InterProcessCom.ConfigPath + "\\waypoints.xml", _Waypoints);
            _Waypoints = XmlSerializationHelper.Deserialize<Waypoints>(InterProcessCom.ConfigPath + "\\waypoints.xml");
        }
    }
}
