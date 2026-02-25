using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace SimpleNetExecutor.Client
{
    internal class DllExecutor
    {
        public void ExecuteLatestDll(Action<string> output)
        {
            string pathToModuleDll = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleNetExecutor", "module.dll");
            if (!File.Exists(pathToModuleDll))
                return;

            var context = new AssemblyLoadContext("module", true);
            var assembly = context.LoadFromAssemblyPath(pathToModuleDll);
            var type = assembly.GetTypes().FirstOrDefault(t => t.GetMethod("Main") != null);

            var method = type.GetMethod("Main");

            method.Invoke(null, new object[]
            {
                new Action<string>(output)
            });
        }
    }
}
