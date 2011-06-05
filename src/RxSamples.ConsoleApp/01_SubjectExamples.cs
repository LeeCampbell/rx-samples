using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace RxSamples.ConsoleApp
{
  class SubjectExamples : ExamplesBase
  {
    public void SubjectExample()
    {
      var subject = new Subject<string>();

      WriteStreamToConsole(subject);

      subject.OnNext("a");
      subject.OnNext("b");
      subject.OnNext("c");
    }

    public void ReplaySubjectExample()
    {
      var subject = new ReplaySubject<string>();

      subject.OnNext("a");
      WriteStreamToConsole(subject);

      subject.OnNext("b");
      subject.OnNext("c");
    }

    public void AsyncSubjectExample()
    {
      var subject = new AsyncSubject<string>();

      subject.OnNext("a");
      WriteStreamToConsole(subject);
      subject.OnNext("b");
      subject.OnNext("c");
      subject.OnCompleted();
    }

    public void BehaviorSubjectExample()
    {
      //Need to provide a default value.
      var subject = new BehaviorSubject<string>("a");

      subject.OnNext("b");
      subject.OnNext("c");
      subject.OnNext("d");
      subject.OnCompleted();
      WriteStreamToConsole(subject);
    }
  }
}