using System;
using System.Collections.Generic;
using System.Linq;
using ExampleLibrary.NewerVersion;
using ExampleLibrary.OldVersion;

namespace ExampleApp
{
  //This app uses a version of the Lib that is newer that one of the Referenced Libraries, but 
  //  Older than the lib referenced in the other.
  //The interesting part of this solution is the ExampleLibrary.[Old][Newer]Version projects. 
  //  Note the project proerties on the files in the EmbeddedAssemblies folders and the 
  //  DependencyResolver class in each project. By Embedding Rx into each Library, I have
  //  isolated this client from the version that they depend on.
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("This application shows the use of 3 different version of Rx running at once.");
      Console.WriteLine("This application directly references version   1.0.2856.0");
      Console.WriteLine("This application then references two libraries (ExampleLibrary.NewerVersion & ExampleLibrary.OldVersion)");
      Console.WriteLine("ExampleLibrary.OldVersion references version   1.0.2838.0");
      Console.WriteLine("ExampleLibrary.NewerVersion references version 1.0.10425");
      Console.WriteLine("");
      Console.WriteLine("Each library uses a feature that is not available in the other to prove that they are infact running different versions.");
      Console.WriteLine("");

      var dummy = new Subject<int>();
      dummy.Select(i=>string.Format("Current : {0}", i)).Subscribe(Console.WriteLine);

      dummy.OnNext(0);
      dummy.OnNext(-1);
      dummy.OnNext(-2);
      
      var oldVer = SomeOtherProvider.GetNumbers()
        .Merge()
        .Select(i => string.Format("Old : {0}", i));
      //IObservable<IObservable<long>> oldVer = Observable.Interval(1.Seconds()).BufferWithTime(3.Seconds()) //Buffer doesnt returns IO<IO<T>> in this version.
      oldVer.Subscribe(Console.WriteLine);


      var newVer = SomeProvider.GetNumbers().Select(i => string.Format("New : {0}", string.Concat(i.ToArray())));
      //newVer.Buffer(3);   //No Buffer in this version, only BufferWithCount/Time
      newVer.Subscribe(Console.WriteLine);

      Console.ReadLine();
    }
  }
}
