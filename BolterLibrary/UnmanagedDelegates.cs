using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BolterLibrary
{
    public class UnmanagedDelegates
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fUnloadIt([MarshalAs(UnmanagedType.LPStr)]string domainName);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate float fGetPOS(EntityType eType, Axis axis, Int32 index);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public delegate string fGetName(EntityType eType, Int32 index);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fSetPOS(EntityType type, Axis axis, Int32 index, [MarshalAs(UnmanagedType.R4)]float value);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fSetHeading(EntityType type, Int32 index, float value);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate float fGetHeading(EntityType type, Int32 index);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fSet3DVector(EntityType type, Axis axis, Int32 index, float value);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate float fGet3DVector(EntityType type, Axis axis, Int32 index);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fGetBuff(Int32 buffIndex, Int32 eIndex, [In,Out]byte[] pData, [MarshalAs(UnmanagedType.Bool)]bool set, BuffInfoType pInfo);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate float fGetMovement(MovementEnum mEnum);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fSetMovement(MovementEnum mEnum, float value);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate WalkingStatus fGetMoveStatus();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fSetMoveStatus(WalkingStatus status);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate Int32 fGetEntityID(EntityType eType, Int32 index);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate Int32 fGetTargetEntityID();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fHookMenuIDFunc();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fSetMenuItemVar(ref int menuItemIdVar);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return :MarshalAs(UnmanagedType.BStr)]
        public delegate string fGetZoneName();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public delegate string fGetBuffName(UInt16 id);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fCollisionToggle([MarshalAs(UnmanagedType.Bool)]bool on);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fSendCommand([MarshalAs(UnmanagedType.LPStr)]string pCommand);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool fScanNames([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]string[] pNames);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate Int32 fGetZoneID();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool fIsNewChatLine();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public delegate string fGetNameOfSender();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public delegate string fGetChatLine();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fToggleLockAxis(Axis axis, [MarshalAs(UnmanagedType.Bool)]bool on);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr fGetEntity(EntityType eType, Int32 index);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Funcs
    {
        public static UnmanagedDelegates.fUnloadIt UnloadAppDomain;

        public static UnmanagedDelegates.fGetPOS GetPOS;

        public static UnmanagedDelegates.fGetName GetName;

        public static UnmanagedDelegates.fSetPOS SetPOS;

        public static UnmanagedDelegates.fSetHeading SetHeading;

        public static UnmanagedDelegates.fGetHeading GetHeading;

        public static UnmanagedDelegates.fSet3DVector Set3DVector;

        public static UnmanagedDelegates.fGet3DVector Get3DVector;

        public static UnmanagedDelegates.fGetBuff GetBuff;

        public static UnmanagedDelegates.fGetMovement GetMovement;

        public static UnmanagedDelegates.fSetMovement SetMovement;

        public static UnmanagedDelegates.fGetMoveStatus GetMoveStatus;

        public static UnmanagedDelegates.fSetMoveStatus SetMoveStatus;

        public static UnmanagedDelegates.fGetEntityID GetEntityID;

        public static UnmanagedDelegates.fGetTargetEntityID GetTargetEntityID;

        public static UnmanagedDelegates.fHookMenuIDFunc HookMenuIdFunc;

        public static UnmanagedDelegates.fSetMenuItemVar SetMenuItemVar;

        public static UnmanagedDelegates.fGetZoneName GetZoneName;

        public static UnmanagedDelegates.fGetBuffName GetBuffName;

        public static UnmanagedDelegates.fCollisionToggle CollisionToggle;

        public static UnmanagedDelegates.fSendCommand SendCommand;

        public static UnmanagedDelegates.fScanNames ScanNames;

        public static UnmanagedDelegates.fGetZoneID GetZoneId;

        public static UnmanagedDelegates.fIsNewChatLine IsNewChatLine;

        public static UnmanagedDelegates.fGetNameOfSender GetNameOfSender;

        public static UnmanagedDelegates.fGetChatLine GetChatLine;

        public static UnmanagedDelegates.fToggleLockAxis ToggleLockAxis;

        public static UnmanagedDelegates.fGetEntity GetEntity;
    }
    
}
