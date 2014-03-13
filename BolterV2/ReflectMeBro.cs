using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace BolterV2
{
    public class ReflectMeBro
    {
        private Assembly MapMe { get; set; }
        private Type WinAPI { get; set; }
        private MethodInfo ReadRemoteMemory { get; set; }
        private MethodInfo WriteProcessMemory { get; set; }
        private MethodInfo GetProcAddressEx { get; set; }
        private MethodInfo NtCreateThreadEx { get; set; }
        private MethodInfo WaitForSingleObject { get; set; }
        private MethodInfo VirtualAllocEx { get; set; }
        private MethodInfo VirtualFreeEx { get; set; }
        private MethodInfo OpenHandle { get; set; }
        private MethodInfo CloseHandle { get; set; }
        private MethodInfo Injector { get; set; }
        private ConstructorInfo PortableExecutable { get; set; }
        private Type InjectionEnum { get; set; }

        public ReflectMeBro()
        {

            try
            {
                MapMe = AppDomain.CurrentDomain.Load(Properties.Resources.ManualMapper);
                WinAPI = MapMe.GetType("ManualMapper.WinAPI");
                ReadRemoteMemory = WinAPI.GetMethod("ReadRemoteMemory");
                WriteProcessMemory = WinAPI.GetMethod("WriteProcessMemory");
                GetProcAddressEx = WinAPI.GetMethod("GetProcAddressEx");
                NtCreateThreadEx = WinAPI.GetMethod("NtCreateThreadEx");
                WaitForSingleObject = WinAPI.GetMethod("WaitForSingleObject");
                VirtualAllocEx = WinAPI.GetMethod("VirtualAllocEx");
                VirtualFreeEx = WinAPI.GetMethod("VirtualFreeEx");
                CloseHandle = WinAPI.GetMethod("CloseHandle");
                OpenHandle = WinAPI.GetMethod("OpenProcess");
                Injector = MapMe.GetType("ManualMapper.InjectionMethod").GetMethod("Create");
                InjectionEnum = MapMe.GetType("ManualMapper.InjectionMethodType");
                PortableExecutable =
                    MapMe.GetType("ManualMapper.PortableExecutable").GetConstructor(new[] {typeof (byte[])});
                MapMe.GetType("ManualMapper.ReflectionHelper").GetMethod("AddHandler").Invoke(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Encountered an unrecoverable error, Now closing. Error: " + ex.Message);
                Process.GetCurrentProcess().Close();
            }
        }

        public IntPtr Inject(byte[] dllBytes, IntPtr hProc)
        {

            var injectorClass = Injector.Invoke(null, new[] {InjectionEnum.GetEnumValues().GetValue(1)});

            var img = PortableExecutable.Invoke(new object[] {Properties.Resources.Link});

            return Inject(injectorClass, img, hProc);
        }

        private IntPtr Inject(object instance, object img, IntPtr hProc)
        {
            var methodSig = new[] {img.GetType(), typeof (IntPtr)};
            return
                (IntPtr)
                    MapMe.GetType("ManualMapper.ManualMap")
                        .GetMethod("Inject", methodSig)
                        .Invoke(instance, new[] {img, hProc});
        }

        public IntPtr CreateThread(IntPtr hProc, IntPtr routinePtr, IntPtr paraPtr, bool suspend = false)
        {
            var args = new object[11];
            args[1] = 0x1FFFFFu;
            args[3] = hProc;
            args[4] = routinePtr;
            args[5] = paraPtr;
            args[6] = suspend;
            NtCreateThreadEx.Invoke(null, args);
            return (IntPtr) args[0];
        }

        public byte[] ReadMemory(IntPtr hProc, IntPtr address, uint size)
        {
            var args = new object[3];
            args[0] = hProc;
            args[1] = address;
            args[2] = size;
            return (byte[]) ReadRemoteMemory.Invoke(null, args);
        }

        public uint WriteMemory(IntPtr hProc, IntPtr address, byte[] data, int size)
        {
            var args = new object[5];
            args[0] = hProc;
            args[1] = address;
            args[2] = data;
            args[3] = size;
            WriteProcessMemory.Invoke(null, args);
            return (uint) args[4];
        }

        public IntPtr GetFuncPointer(IntPtr hProc, IntPtr hModule, string FuncName)
        {
            var args = new object[3];
            args[0] = hProc;
            args[1] = hModule;
            args[2] = FuncName;
            return (IntPtr) GetProcAddressEx.Invoke(null, args);
        }

        public IntPtr AllocMem(IntPtr hProc, uint size, int allocType, int protection)
        {
            var args = new object[5];
            args[0] = hProc;
            args[2] = size;
            args[3] = allocType;
            args[4] = protection;
            return (IntPtr) VirtualAllocEx.Invoke(null, args);
        }

        public void FreeMem(IntPtr hProc, IntPtr address, int freeType)
        {
            var args = new object[4];
            args[0] = hProc;
            args[1] = address;
            args[3] = freeType;
            VirtualFreeEx.Invoke(null, args);
        }

        public void CloseHan(IntPtr handle)
        {
            CloseHandle.Invoke(null, new object[] {handle});
        }

        public IntPtr OpenHan(uint dwDesiredAccess, int pid)
        {
            var args = new object[3];
            args[0] = dwDesiredAccess;
            args[1] = false;
            args[2] = pid;
            return (IntPtr) OpenHandle.Invoke(null, args);
        }

        public void WaitForEvent(IntPtr handle, int timeout)
        {
            WaitForSingleObject.Invoke(null, new object[] {handle, timeout});
        }

    }
}
