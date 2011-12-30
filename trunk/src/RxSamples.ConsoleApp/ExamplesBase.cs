using System;
using System.Collections.Generic;

namespace RxSamples.ConsoleApp
{
  internal class ExamplesBase
  {
    protected static void WriteStreamToConsole(IObservable<string> stream)
    {
      stream.Subscribe(Console.WriteLine);
    }
    protected static void WriteStreamToConsole<T>(IObservable<T> stream, string name)
    {
      stream.Subscribe(
        value => Console.WriteLine("{0} : {1}", name, value),
        ex => Console.WriteLine("{0} : {1}", name, ex),
        () => Console.WriteLine("{0} Completed", name));
    }
    protected static void WriteListToConsole<T>(IEnumerable<T> list, string name)
    {
        foreach (var item in list)
        {
            Console.WriteLine("{0} : {1}", name, item);
        }
        Console.WriteLine("{0} Completed", name);
    }
  }
}