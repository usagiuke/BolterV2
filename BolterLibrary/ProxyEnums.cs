using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BolterLibrary
{
    class ProxyEnums
    {
        
    }

    internal class PCMobStruct
    {
        internal const int Name = 0x30,
            ID = 0x74,
            NPCID = 0x78,
            MobType = 0x8A,
            CurrentTarget = 0x8C,
            Distance = 0x8D,
            GatheringStatus = 0x8E,
            POS = 0xA0,
            Heading = 0xB0,
            ServerHight = 0xB4,
            FateId = 0xE8,
            subStruct = 0xEC,
            GatheringInvisible = 0x11C,
            CamGlide = 0x134,
            StaticCamGlide = 0x174,
            StatusAdjust = 0x188,
            IsGM = 0x189,
            Icon = 0x194,
            IsEngaged = 0x195,
            EntityID = 0x1D0,
            IsMoving = 0x1F0,
            TimeTraveled = 0x23C,
            ChatPoint = 0x594,
            TargetID = 0xAA8,
            CurrentJob = 0x1830,
            CurrentLevel = 0x1831,
            CurrentGC = 0x1832,
            GCRank = 0x1833,
            CurrentHP = 0x1838,
            MaxHP = 0x183C,
            CurrentMP = 0x1840,
            MaxMP = 0x1844,
            TP = 0x1848,
            GP = 0x184A,
            UnknownPoints = 0x184C,
            CurrentCP = 0x184E,
            MaxCP = 0x1850,
            BuffID = 0x31B8,
            BuffParam = 0x31BA,
            BuffTime = 0x31BC,
            BuffOther = 0x31C0;
    }
}
