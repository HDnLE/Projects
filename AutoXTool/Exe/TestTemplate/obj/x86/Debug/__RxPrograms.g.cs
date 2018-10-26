using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Ranorex;
using Ranorex.Core.Resolver;

namespace __RxMain
{
    public class __RxProgram
    {
        [STAThread]
        public static int Main(string[] args)
        {
            PrerequisiteChecker.WriteErrorToConsoleIfNotAvailable();
            AssemblyLoader.Initialize();

            RanorexInit();

            return MainInvoker.Invoke(Assembly.GetExecutingAssembly(), "", args);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void RanorexInit()
        {
            TestingBootstrapper.SetupCore();
        }
    }
}
