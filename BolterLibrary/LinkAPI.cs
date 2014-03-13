using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace BolterLibrary
{
    public class AllEntities
    {
        
        public IList<IPCMobEntity> PcMobEntities = new IPCMobEntity[95];

        public IList<IObjectEntity> ObjectEntities = new IObjectEntity[22];

        public IList<INPCEntity> NpcEntities = new INPCEntity[40];

        public AllEntities()
        {
            for (var i = 0; i < 95; i++)
            {
                PcMobEntities[i] = new PCMobEntity(i, EntityType.PCMob);
            }
            for (var i = 0; i < 22; i++)
            {
                ObjectEntities[i] = new ObjectEntity(i, EntityType.Object);
            }

            for (var i = 0; i < 40; i++)
            {
                NpcEntities[i] = new NPCEntity(i, EntityType.NPC);
            }
            
        }

        public interface IEntity
        {
            int eIndex { get; set; }
            EntityType EType { get; set; }
            IntPtr StructBase { get; }
            float X { get; set; }
            float Y { get; set; }
            float Z { get; set; }
            float VectorX { get; set; }
            float VectorY { get; set; }
            float VectorZ { get; set; }
            string Name { get; }
            Int32 EntityID { get; }
            void Move(float distance, Direction direction);
            void Warp(float x, float z, float y);
            Int32 ID { get; }
            byte MobType { get; }
            byte CurrentTarget { get; set; }
            byte Distance { get; }
            byte GatheringStatus { get; set; }
            UInt16 FateID { get; }
            byte GatheringInvs { get; set; }
            float CamGlide { get; set; }
            float StaticCamGlide { get; set; }
            byte Status { get; set; }
            byte IsGM { get; set; }
            byte Icon { get; set; }
            byte IsEngadged { get; set; }
            Int32 IsMoving { get; }
            float TimeTraveled { get; set; }
            UInt32 TargetID { get; set; }
            byte CurrentJob { get; set; }
            byte CurrentLevel { get; set; }
            byte CurrentGC { get; set; }
            byte GCRank { get; set; }
            Int32 HP { get; set; }
            Int32 MaxHP { get; set; }
            Int32 MP { get; set; }
            Int32 MaxMP { get; set; }
            Int16 TP { get; set; }
            Int16 GP { get; set; }
            Int16 UnknownPoints { get; set; }
            Int16 CP { get; set; }
            Int16 MaxCP { get; set; }

        }

        public interface IPCMobEntity : IEntity
        {
            BuffStruct GetBuffDebuff(int index);
        }

        public interface IObjectEntity : IEntity
        {
            int IsActive { get; set; }
        }

        public interface INPCEntity : IEntity
        {
            int IsActive { get; set; }
        }

        public class PCMobEntity : Entity, IPCMobEntity
        {
            public PCMobEntity(int index, EntityType eType)
            {
                eIndex = index;
                EType = eType;
                StructBase = Funcs.GetEntity(EType, eIndex);
                
            }
            public new BuffStruct GetBuffDebuff(int index)
            {
                return new BuffStruct(eIndex, index);
            }

        }

        public class ObjectEntity : Entity, IObjectEntity
        {
            public ObjectEntity(int index, EntityType eType)
            {
                eIndex = index;
                EType = eType;
                StructBase = Funcs.GetEntity(EType, eIndex);
            }
            public new int IsActive { get; set; }
        }

        public class NPCEntity : Entity, INPCEntity
        {
            public NPCEntity(int index, EntityType eType)
            {
                eIndex = index;
                EType = eType;
                StructBase = Funcs.GetEntity(EType, eIndex);
            }
            public new int IsActive { get; set; }
        }
        public class Entity
        {
            public int eIndex { get; set; }
            public EntityType EType { get; set; }
            public IntPtr StructBase { get; protected set; }
            public void Move(float distance, Direction direction)
            {
                switch (direction)
                {
                    case Direction.N:
                        Y -= distance;
                        break;
                    case Direction.S:
                        Y += distance;
                        break;
                    case Direction.E:
                        X += distance;
                        break;
                    case Direction.W:
                        X -= distance;
                        break;
                    case Direction.NE:
                        X += distance;
                        Y -= distance;
                        break;
                    case Direction.NW:
                        X -= distance;
                        Y -= distance;
                        break;
                    case Direction.SE:
                        X += distance;
                        X += distance;
                        break;
                    case Direction.SW:
                        X -= distance;
                        Y += distance;
                        break;
                    case Direction.Down:
                        Z -= distance;
                        break;
                    case Direction.Up:
                        Z += distance;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("direction");
                }
            }

            public void Warp(float x, float z, float y)
            {
                X = x;
                Z = z;
                Y = y;
            }

            public float X 
            { 
                get { return Funcs.GetPOS(EType, Axis.X, eIndex); }
                set { Funcs.SetPOS(EType, Axis.X, eIndex, value); }
            }

            public float Y
            {
                get { return Funcs.GetPOS(EType, Axis.Y, eIndex); }
                set { Funcs.SetPOS(EType, Axis.Y, eIndex, value); }
            }

            public float Z
            {
                get { return Funcs.GetPOS(EType, Axis.Z, eIndex); }
                set { Funcs.SetPOS(EType, Axis.Z, eIndex, value); }
            }

            public float VectorX
            {
                get { return Funcs.Get3DVector(EType, Axis.X, eIndex); }
                set { Funcs.Set3DVector(EType, Axis.X, eIndex, value); }
            }

            public float VectorY
            {
                get { return Funcs.Get3DVector(EType, Axis.Y, eIndex); }
                set { Funcs.Set3DVector(EType, Axis.Y, eIndex, value); }
            }

            public float VectorZ
            {
                get { return Funcs.Get3DVector(EType, Axis.Z, eIndex); }
                set { Funcs.Set3DVector(EType, Axis.Z, eIndex, value); }
            }

            public string Name
            {
                get { return Funcs.GetName(EType, eIndex); }
            }

            public Int32 EntityID
            {
                get { return Funcs.GetEntityID(EType, eIndex); }
            }

            public virtual BuffStruct GetBuffDebuff(int index)
            {
                return null;
            }

            public Int32 ID
            {
                get { return Marshal.ReadInt32(StructBase, PCMobStruct.ID); }
            }

            public byte MobType
            {
                get { return Marshal.ReadByte(StructBase, PCMobStruct.MobType); }
            }

            public byte CurrentTarget
            {
                get { return Marshal.ReadByte(StructBase, PCMobStruct.CurrentTarget); }
                set { Marshal.WriteByte(StructBase, PCMobStruct.CurrentTarget, value); }
            }

            public byte Distance
            {
                get { return Marshal.ReadByte(StructBase, PCMobStruct.Distance); }
            }

            public byte GatheringStatus
            {
                get { return Marshal.ReadByte(StructBase, PCMobStruct.GatheringStatus); }
                set { Marshal.WriteByte(StructBase, PCMobStruct.GatheringStatus, value); }
            }

            public UInt16 FateID
            {
                get { return (UInt16)Marshal.PtrToStructure(StructBase + PCMobStruct.FateId, typeof(UInt16)); }
            }

            public byte GatheringInvs
            {
                get { return Marshal.ReadByte(StructBase, PCMobStruct.GatheringInvisible); }
                set { Marshal.WriteByte(StructBase, PCMobStruct.GatheringInvisible, value); }
            }

            public float CamGlide
            {
                get { return (float)Marshal.PtrToStructure(StructBase + PCMobStruct.CamGlide, typeof(float)); }
                set { Marshal.StructureToPtr(value, StructBase + PCMobStruct.CamGlide, false); }
            }

            public float StaticCamGlide
            {
                get { return (float)Marshal.PtrToStructure(StructBase + PCMobStruct.StaticCamGlide, typeof(float)); }
                set { Marshal.StructureToPtr(value, StructBase + PCMobStruct.StaticCamGlide, false); }
            }

            public byte Status
            {
                get { return Marshal.ReadByte(StructBase, PCMobStruct.StatusAdjust); }
                set { Marshal.WriteByte(StructBase, PCMobStruct.StatusAdjust, value); }
            }

            public byte IsGM
            {
                get { return Marshal.ReadByte(StructBase, PCMobStruct.IsGM); }
                set { Marshal.WriteByte(StructBase, PCMobStruct.IsGM, value); }
            }

            public byte Icon
            {
                get { return Marshal.ReadByte(StructBase, PCMobStruct.Icon); }
                set { Marshal.WriteByte(StructBase, PCMobStruct.Icon, value); }      
            }

            public byte IsEngadged
            {
                get { return Marshal.ReadByte(StructBase, PCMobStruct.IsEngaged); }
                set { Marshal.WriteByte(StructBase, PCMobStruct.IsEngaged, value); }      
            }

            public Int32 IsMoving
            {
                get { return Marshal.ReadInt32(StructBase, PCMobStruct.IsMoving); }
            }

            public float TimeTraveled
            {
                get { return (float)Marshal.PtrToStructure(StructBase + PCMobStruct.TimeTraveled, typeof(float)); }
                set { Marshal.StructureToPtr(value, StructBase + PCMobStruct.TimeTraveled, false); }
            }

            public UInt32 TargetID
            {
                get { return (UInt32)Marshal.PtrToStructure(StructBase + PCMobStruct.TargetID, typeof(UInt32)); }
                set { Marshal.StructureToPtr(value, StructBase + PCMobStruct.TargetID, false); }
            }

            public byte CurrentJob
            {
                get { return Marshal.ReadByte(StructBase, PCMobStruct.CurrentJob); }
                set { Marshal.WriteByte(StructBase, PCMobStruct.CurrentJob, value); }
            }

            public byte CurrentLevel
            {
                get { return Marshal.ReadByte(StructBase, PCMobStruct.CurrentLevel); }
                set { Marshal.WriteByte(StructBase, PCMobStruct.CurrentLevel, value); }
            }

            public byte CurrentGC
            {
                get { return Marshal.ReadByte(StructBase, PCMobStruct.CurrentGC); }
                set { Marshal.WriteByte(StructBase, PCMobStruct.CurrentGC, value); }
            }

            public byte GCRank
            {
                get { return Marshal.ReadByte(StructBase, PCMobStruct.GCRank); }
                set { Marshal.WriteByte(StructBase, PCMobStruct.GCRank, value); }
            }

            public Int32 HP
            {
                get { return Marshal.ReadInt32(StructBase, PCMobStruct.CurrentHP); }
                set { Marshal.WriteInt32(StructBase, PCMobStruct.CurrentHP, value); }
            }

            public Int32 MaxHP
            {
                get { return Marshal.ReadInt32(StructBase, PCMobStruct.MaxHP); }
                set { Marshal.WriteInt32(StructBase, PCMobStruct.MaxHP, value); }
            }

            public Int32 MP
            {
                get { return Marshal.ReadInt32(StructBase, PCMobStruct.CurrentMP); }
                set { Marshal.WriteInt32(StructBase, PCMobStruct.CurrentMP, value); }
            }

            public Int32 MaxMP
            {
                get { return Marshal.ReadInt32(StructBase, PCMobStruct.MaxMP); }
                set { Marshal.WriteInt32(StructBase, PCMobStruct.MaxMP, value); }
            }

            public Int16 TP
            {
                get { return (Int16)Marshal.PtrToStructure(StructBase + PCMobStruct.TP, typeof(Int16)); }
                set { Marshal.StructureToPtr(value, StructBase + PCMobStruct.TP, false); }
            }

            public Int16 GP
            {
                get { return (Int16)Marshal.PtrToStructure(StructBase + PCMobStruct.TP, typeof(Int16)); }
                set { Marshal.StructureToPtr(value, StructBase + PCMobStruct.TP, false); }
            }

            public Int16 UnknownPoints
            {
                get { return (Int16)Marshal.PtrToStructure(StructBase + PCMobStruct.UnknownPoints, typeof(Int16)); }
                set { Marshal.StructureToPtr(value, StructBase + PCMobStruct.UnknownPoints, false); }
            }

            public Int16 CP
            {
                get { return (Int16)Marshal.PtrToStructure(StructBase + PCMobStruct.CurrentCP, typeof(Int16)); }
                set { Marshal.StructureToPtr(value, StructBase + PCMobStruct.CurrentCP, false); }
            }

            public Int16 MaxCP
            {
                get { return (Int16)Marshal.PtrToStructure(StructBase + PCMobStruct.MaxCP, typeof(Int16)); }
                set { Marshal.StructureToPtr(value, StructBase + PCMobStruct.MaxCP, false); }
            }

            public virtual int IsActive { get; set; }
        }

        public Entity TargetEntity
        {
            get
            {
                foreach (var ent in PcMobEntities.Where(ent => ent.EntityID == Funcs.GetTargetEntityID()))
                {
                    return (Entity)ent;
                }
                foreach (var ent in ObjectEntities.Where(ent => ent.EntityID == Funcs.GetTargetEntityID()))
                {
                    return (Entity)ent;
                }
                return NpcEntities.Where(ent => ent.EntityID == Funcs.GetTargetEntityID()).Cast<Entity>().FirstOrDefault();
            }
        }

        public static D3DXVECTOR2 Get2DPos()
        {
            return new D3DXVECTOR2(Funcs.GetPOS(EntityType.PCMob, Axis.X, 0), Funcs.GetPOS(EntityType.PCMob, Axis.Y, 0));
        }
    }

    public class Movement
    {
        public float CurrentSpeed
        {
            get { return Funcs.GetMovement(MovementEnum.CurrentSpeed); }
        }

        public float ForwardSpeed
        {
            get { return Funcs.GetMovement(MovementEnum.ForwardSpeed); }
            set { Funcs.SetMovement(MovementEnum.ForwardSpeed, value); }
        }

        public float LeftRightSpeed
        {
            get { return Funcs.GetMovement(MovementEnum.LeftRightSpeed); }
            set { Funcs.SetMovement(MovementEnum.LeftRightSpeed, value); }
        }

        public float BackwardSpeed
        {
            get { return Funcs.GetMovement(MovementEnum.BackwardSpeed); }
            set { Funcs.SetMovement(MovementEnum.BackwardSpeed, value); }
        }

        public WalkingStatus Status
        {
            get { return Funcs.GetMoveStatus(); }
            set { Funcs.SetMoveStatus(value); }
        }
    }
    public class BuffStruct
    {
        private readonly IntPtr StructBase;
        public BuffStruct(int eIndex, int bIndex)
        {
            StructBase = Funcs.GetEntity(EntityType.PCMob, eIndex) + (0xC*bIndex);
        }

        public UInt16 ID
        {
            get { return (UInt16)Marshal.PtrToStructure(StructBase + PCMobStruct.BuffID, typeof(UInt16)); }
            set { Marshal.StructureToPtr(value, StructBase + PCMobStruct.BuffID, false); }
        }
        public UInt16 Paras
        {
            get { return (UInt16)Marshal.PtrToStructure(StructBase + PCMobStruct.BuffParam, typeof(UInt16)); }
            set { Marshal.StructureToPtr(value, StructBase + PCMobStruct.BuffParam, false); }
        }
        public float Timer
        {
            get { return (float)Marshal.PtrToStructure(StructBase + PCMobStruct.BuffTime, typeof(float)); }
            set { Marshal.StructureToPtr(value, StructBase + PCMobStruct.BuffTime, false); }
        }
        public uint ExtraInfo
        {
            get { return (uint)Marshal.PtrToStructure(StructBase + PCMobStruct.BuffOther, typeof(uint)); }
            set { Marshal.StructureToPtr(value, StructBase + PCMobStruct.BuffOther, false); }
        }
    }
    
    public enum MovementEnum
    {
        EWalkingStatus,
        CurrentSpeed,
        ForwardSpeed,
        LeftRightSpeed,
        BackwardSpeed
    };

    public enum BuffInfoType
    {
        ID,
        Params,
        Timer,
        ExtraInfo
    };
    [Flags]
    public enum TargetStatus : uint
    {
        NoTarget = 0x00010001,
        HasTarget = 0x00010000,
        Locked = 0x01010000
    }

    [Flags]
    public enum WalkingStatus
    {
        Standing = 0x00000000,
        Running = 0x00000001,
        Heading = 0x00000100,
        Walking = 0x00010000,
        Autorun = 0x01000000
    }
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public enum PosType
    {
        Server,
        Client
    }
    public enum EntityType
    {
        PCMob,
        Object,
        NPC
    };
    public enum Direction
    {
        N,
        S,
        E,
        W,
        NE,
        NW,
        SE,
        SW,
        Down,
        Up
    };
}
