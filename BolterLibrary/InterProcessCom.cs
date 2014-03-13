using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;

#pragma warning disable 0618

namespace BolterLibrary
{
    public static class InterProcessCom //: MarshalByRefObject
    {
        public static string ConfigPath;

        private static object pHandle;

        public static EventWaitHandle eHandle;
        /// <summary>
        /// Function that starts Bolter. Takes various information 
        /// that the unmanaged side needs to pass to the managed side.
        /// </summary>
        public static int PassInfo(string[] configPath, int[] fPtrs)
        {
            Console.Write(configPath[0]);
            EventWaitHandle result;
            EventWaitHandle.TryOpenExisting(@"Global\FinishedLoading",
                EventWaitHandleRights.Synchronize | EventWaitHandleRights.Modify, out result);
            if (result != null)
            {
                eHandle = result;
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;

            ConfigPath = Path.GetDirectoryName(Path.GetDirectoryName(configPath[0])) + "\\Resources";

            var count = 0;
            foreach (var f in typeof(Funcs).GetFields())
            {
                f.SetValue(null, Marshal.GetDelegateForFunctionPointer((IntPtr)fPtrs[count], f.FieldType));
                count++;
            }

            new Thread(new ThreadStart(delegate
            {
                pHandle =
                    AppDomain.CurrentDomain.GetAssemblies()
                        .First(ass => ass.GetTypes().Any((t => t.Name == "PluginMain")))
                        .GetTypes().First(t => t.Name == "PluginMain")
                        .GetConstructors()
                        .First()
                        .Invoke(null);
                pHandle.GetType().GetMethod("Start").Invoke(pHandle, null);
            })) { ApartmentState = ApartmentState.STA }.Start();
            return 1;
        }

        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Console.WriteLine("{0} Unloaded", ((AppDomain)sender).FriendlyName);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void SignalFinished()
        {
            eHandle.Set();
            eHandle.Close();
        }

        public static int PluginSelfUnload()
        {
            Funcs.UnloadAppDomain(AppDomain.CurrentDomain.FriendlyName);
            return 1;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
        }
    }
}