using System;

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
  }
}