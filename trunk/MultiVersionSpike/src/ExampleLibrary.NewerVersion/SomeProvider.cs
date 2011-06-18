using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace ExampleLibrary.NewerVersion
{
  public class SomeProvider
  {
    static SomeProvider()
    {
      DependencyResolver.Ensure();
    }

    public static IObservable<IList<long>> GetNumbers()
    {
      return Observable.Interval(TimeSpan.FromSeconds(1))
        .Buffer(3); //Brand new to Rx_v1.0.10425
    }
  }
}
