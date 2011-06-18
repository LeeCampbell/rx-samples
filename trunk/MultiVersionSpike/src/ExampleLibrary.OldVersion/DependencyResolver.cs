using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace ExampleLibrary.OldVersion
{
  internal static class DependencyResolver
  {
    private static int _isSet = 0;

    internal static void Ensure()
    {
      if (Interlocked.CompareExchange(ref _isSet, 1, 0) == 0)
      {
        var thisAssembly = Assembly.GetExecutingAssembly();
        var assemblyName = new AssemblyName(thisAssembly.FullName).Name;
        var embededAssemblyPrefix = assemblyName + ".EmbeddedAssemblies.";

        var myEmbeddedAssemblies =
          Assembly.GetExecutingAssembly().GetManifestResourceNames()
            .Where(name => name.StartsWith(embededAssemblyPrefix))
            .Select(resourceName =>
                      {
                        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                        {
                          var assemblyData = new Byte[stream.Length];
                          stream.Read(assemblyData, 0, assemblyData.Length);
                          return Assembly.Load(assemblyData);
                        }
                      })
            .ToDictionary(ass => ass.FullName);

        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                                                     {
                                                       Assembly assemblyToLoad = null;
                                                       myEmbeddedAssemblies.TryGetValue(args.Name, out assemblyToLoad);
                                                       return assemblyToLoad;
                                                     };
      }
    }
  }
}