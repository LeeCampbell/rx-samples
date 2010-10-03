using System;
using System.Collections.Generic;
using System.Linq;

namespace RxSamples.ConsoleApp
{
  class EnumerableExamples
  {
    public void IEnumerableExample()
    {
      Console.WriteLine("--Starting Lazy Evaluation--");
      Do(LazyEvaluation());
      Console.WriteLine("--Starting Eager Evaluation--");
      Do(EagerEvaluation());

      Console.WriteLine("--Starting Lazy Evaluation with Take(1)--");
      LazyEvaluation().Take(1).Run(Console.WriteLine);

      Console.ReadKey();
    }

    private void Do(IEnumerable<int> list)
    {
      foreach (var i in list)
      {
        Console.WriteLine("Read out first value of {0}", i);
        break;
      }
    }

    public IEnumerable<int> LazyEvaluation()
    {
      Console.WriteLine("About to return 1");
      yield return 1;
      Console.WriteLine("About to return 2");
      yield return 2;
    }

    public IEnumerable<int> EagerEvaluation()
    {
      var result = new List<int>();
      Console.WriteLine("About to return 1");
      result.Add(1);
      Console.WriteLine("About to return 2");
      result.Add(2);
      return result;
    }
  }
}