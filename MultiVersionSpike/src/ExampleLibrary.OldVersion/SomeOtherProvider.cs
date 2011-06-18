using System;
using System.Linq;

namespace ExampleLibrary.OldVersion
{
  public class SomeOtherProvider
  {
    static SomeOtherProvider()
    {
      DependencyResolver.Ensure();
    }
    
    public static IObservable<IObservable<long>> GetNumbers()
    {
      return Observable.Interval(TimeSpan.FromSeconds(1))
        .BufferWithTime(TimeSpan.FromSeconds(3)); //This old overload returned IO<IO<T>>, newer ones return IO<IList<T>>, newer still renamed the overload to Buffer
    }
  }
}
