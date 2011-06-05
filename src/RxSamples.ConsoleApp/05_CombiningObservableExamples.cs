using System.Collections.Generic;
using System.Linq;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxSamples.ConsoleApp
{
  class CombiningObservableExamples : ExamplesBase
  {
    public void ConcatSample()
    {
      //Generate values 0,1,2
      var stream1 = Observable.Generate(0, i => i < 3, i => i, i => i + 1);
      //Generate values 100,101,102,103,104
      var stream2 = Observable.Generate(100, i => i < 105, i => i, i => i + 1);

      stream1
          .Concat(stream2)
          .Subscribe(Console.WriteLine);
      Console.ReadLine();

      stream1
          .OnErrorResumeNext(stream2)
          .Subscribe(Console.WriteLine, ex => Console.WriteLine(ex));
      Console.ReadLine();

      /* Returns:
       * 
       * stream1 --0--0--0--|
       * stream2 -----------0--0--0--0--|
       * 
       * result  --0--0--0--0--0--0--0--|
       */
    }

    public void AmbExample()
    {
      //Generate values 0,1,2
      var stream1 = Observable.Range(0, 3);
      //Generate values 100,101,102,103,104
      var stream2 = Observable.Range(100, 5);

      stream1
          .Amb(stream2)
          .Subscribe(Console.WriteLine);
      Console.ReadLine();
      /* Returns:
       *  if stream 1 produces a value first...
       * stream1 --0--0--0--|
       * stream2 ---0--0--0--0--0--|
       * 
       * result  --0--0--0--|     //All from stream1
       */

      stream1.Delay(TimeSpan.FromMilliseconds(100))
          .Amb(stream2)
          .Subscribe(Console.WriteLine);
      Console.ReadLine();
      /* Returns:
       * stream1 ---0--0--0--|
       * stream2 --0--0--0--0--0--|
       * 
       * result  --0--0--0--0--0--|     //All from stream2
       */
    }

    public void MergeExample()
    {
      //Generate values 0,1,2
      var stream1 = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(3);
      //Generate values 100,101,102,103,104
      var stream2 = Observable.Interval(TimeSpan.FromMilliseconds(150)).Take(5).Select(i => i + 100);
      stream1
          .Merge(stream2)
          .Subscribe(Console.WriteLine);
      Console.ReadLine();
      /*
       * Returns:
       * stream1 ----0----0----0|
       * stream2 --0--0--0--0--0|
       * 
       * result  --0-00--00-0--00-|   the result as pairs. 
       * Output:
       * 100
       * 0
       * 101
       * 102
       * 1
       * 103
       * 104      //Note this is a race condition. 2 could be
       * 2        //  published before 104.
       */

      //Create a third stream
      var stream3 = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(10).Select(i => i + 200);

      //Number of streams known at compile time.
      Observable.Merge(stream1, stream2, stream3, Observable.Throw<long>(new Exception("blah")))
          .Subscribe(Console.WriteLine, ex => Console.WriteLine(ex));
      Console.ReadLine();

      //We can dynamically create a list at run time with this overload.
      var streams = new List<System.IObservable<long>>();
      streams.Add(stream1);
      streams.Add(stream2);
      streams.Add(stream3);
      Observable.Merge(streams).Subscribe(Console.WriteLine);
      Console.ReadLine();

    }

    public void SelectManyExample()
    {
      //Generate values 0,1,2
      var stream1 = Enumerable.Range(0, 3).ToObservable();
      //Generate values 100,101,102,103,104
      var stream2 = Enumerable.Range(100, 5).ToObservable();
      stream1
          .SelectMany(i => stream2, (lhs, rhs) => new { Left = lhs, Right = rhs })
          .Subscribe(Console.WriteLine);
      Console.ReadLine();
      /*
       * Output.
       * { Left = 0, Right = 100 }
       * { Left = 0, Right = 101 }
       * { Left = 1, Right = 100 }
       * { Left = 0, Right = 102 }
       * { Left = 1, Right = 101 }
       * { Left = 2, Right = 100 }
       * { Left = 0, Right = 103 }
       * { Left = 1, Right = 102 }
       * { Left = 2, Right = 101 }
       * { Left = 0, Right = 104 }
       * { Left = 1, Right = 103 }
       * { Left = 2, Right = 102 }
       * { Left = 1, Right = 104 }
       * { Left = 2, Right = 103 }
       * { Left = 2, Right = 104 }
       */
    }

    public void SelectMany_IsNotCartesian_Example()
    {
      //Generate values 0,1,2
      var stream1 = new Subject<int>();
      //Generate values 100,101,102,103,104
      var stream2 = new Subject<int>();
      stream1
          .SelectMany(i => stream2, (lhs, rhs) => new { Left = lhs, Right = rhs })
          .Subscribe(Console.WriteLine);

      stream1.OnNext(0);

      stream2.OnNext(100);
      stream2.OnNext(101);
      stream2.OnNext(102);
      stream2.OnNext(103);
      stream2.OnNext(104);

      stream1.OnNext(1);
      stream1.OnNext(2);

      Console.ReadLine();

      stream2.OnNext(105);
      Console.ReadLine();
      /*
       * Output.
       * { Left = 0, Right = 100 }
       * { Left = 0, Right = 101 }
       * { Left = 1, Right = 100 }
       * { Left = 0, Right = 102 }
       * { Left = 1, Right = 101 }
       * { Left = 2, Right = 100 }
       * { Left = 0, Right = 103 }
       * { Left = 1, Right = 102 }
       * { Left = 2, Right = 101 }
       * { Left = 0, Right = 104 }
       * { Left = 1, Right = 103 }
       * { Left = 2, Right = 102 }
       * { Left = 1, Right = 104 }
       * { Left = 2, Right = 103 }
       * { Left = 2, Right = 104 }
       */
    }

    public void SelectMany_on_3_streams_IsNotCartesian_Example()
    {
      //Generate values 0,1,2
      var stream1 = new Subject<int>();
      //Generate values 100,101,102,103,104
      var stream2 = new Subject<int>();
      //Generate values a,b,c
      var stream3 = new Subject<char>();

      (from s1 in stream1
       from s2 in stream2
       from s3 in stream3
       select new { S1 = s1, S2 = s2, S3 = s3 })
          .Subscribe(Console.WriteLine);

      stream1.OnNext(0);

      stream2.OnNext(100);
      stream2.OnNext(101);
      stream2.OnNext(102);

      stream3.OnNext('a');
      stream3.OnNext('b');

      stream1.OnNext(1);
      stream1.OnNext(2);

      Console.ReadLine();
      /*
      * Output to here.
      * { S1 = 0, S2 = 100, S3 = a }
      * { S1 = 0, S2 = 101, S3 = a }
      * { S1 = 0, S2 = 102, S3 = a }
      * { S1 = 0, S2 = 100, S3 = b }
      * { S1 = 0, S2 = 101, S3 = b }
      * { S1 = 0, S2 = 102, S3 = b }
      */

      stream2.OnNext(103);
      stream2.OnNext(104);
      stream3.OnNext('c');
      Console.ReadLine();
      /*
       * final output.
       * { S1 = 0, S2 = 100, S3 = a }
       * { S1 = 0, S2 = 101, S3 = a }
       * { S1 = 0, S2 = 102, S3 = a }
       * { S1 = 0, S2 = 100, S3 = b }
       * { S1 = 0, S2 = 101, S3 = b }
       * { S1 = 0, S2 = 102, S3 = b }
       *
       * { S1 = 0, S2 = 100, S3 = c }
       * { S1 = 0, S2 = 101, S3 = c }
       * { S1 = 0, S2 = 102, S3 = c }
       * { S1 = 0, S2 = 103, S3 = c }
       * { S1 = 1, S2 = 103, S3 = c }
       * { S1 = 2, S2 = 103, S3 = c }
       * { S1 = 0, S2 = 104, S3 = c }
       * { S1 = 1, S2 = 104, S3 = c }
       * { S1 = 2, S2 = 104, S3 = c }
       * 
       * 
       * Note that we miss some values eg: { S1 = 1, S2 = 100, S3 = c }
       */
    }

    public void ZipExample()
    {
      //Generate values 0,1,2
      var stream1 = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(3);
      //Generate values a,b,c,d,e,f
      var stream2 = Observable.Interval(TimeSpan.FromMilliseconds(150)).Take(6).Select(i => Char.ConvertFromUtf32((int)i + 97));
      stream1
          .Zip(stream2, (lhs, rhs) => new { Left = lhs, Right = rhs })
          .Subscribe(Console.WriteLine, () => Console.WriteLine("Complete."));
      Console.ReadLine();
      /* Returns:
       * stream1 ----0----1----2|        stream 1 values represented as ints
       * stream2 --a--b--c--d--e--f|   s2 values represented as chars
       * 
       * result  ----0----1----2|
       *             a    b    c
       */
    }

    public void CombineLatestExample()
    {
      //Generate values 0,1,2
      var stream1 = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(3);
      //Generate values a,b,c,d,e,f
      var stream2 = Observable.Interval(TimeSpan.FromMilliseconds(150)).Take(6).Select(i => Char.ConvertFromUtf32((int)i + 97));
      stream1
          .CombineLatest(stream2, (s1, s2) => new { Left = s1, Right = s2 })
          .Subscribe(Console.WriteLine, () => Console.WriteLine("Complete"));
      Console.ReadLine();
      /*
       * Returns:
       * stream1 ----0----1----2|        stream 1 values represented as ints
       * stream2 --a--b--c--d--e--f|     stream 2 values represented as chars
       * 
       * result  ----00--01-1--22-2|     the result as pairs.
       *             ab  cc d  de f    
       */
    }

    public void ForkJoinExample()
    {
      //Generate values 0,1,2
      var stream1 = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(3);
      //Generate values a,b,c,d,e
      var stream2 = Observable.Interval(TimeSpan.FromMilliseconds(150)).Take(5).Select(i => Char.ConvertFromUtf32((int)i + 97));
      stream1
          
          //.ForkJoin(stream2, (s1, s2) => new { Left = s1, Right = s2 })

          .CombineLatest(stream2, (s1, s2) => new { Left = s1, Right = s2 })
          .TakeLast(1)

          .Subscribe(Console.WriteLine);
      Console.ReadLine();
      /*
       * Returns:
       * stream1 ----0----1----2|        stream 1 values represented as ints
       * stream2 --a--b--c--d--e|   s2 values represented as chars
       * 
       * result  --------------2|   the result as pairs. 
       *                       e     
       */
    }

  }
}