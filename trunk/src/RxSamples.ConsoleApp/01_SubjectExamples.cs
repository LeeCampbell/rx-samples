using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;

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

        public void SubjectInvalidUsageExample()
        {
            var subject = new Subject<string>();

            subject.Subscribe(Console.WriteLine);

            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnCompleted();
            subject.OnNext("c");
        }

        #region ReplaySubject Examples

        public void ReplaySubjectExample()
        {
            var subject = new ReplaySubject<string>();

            subject.OnNext("a");
            WriteStreamToConsole(subject);

            subject.OnNext("b");
            subject.OnNext("c");
        }

        public void ReplaySubjectBufferExample()
        {
            var bufferSize = 2;
            var subject = new ReplaySubject<string>(bufferSize);

            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnNext("c");
            subject.Subscribe(Console.WriteLine);
            subject.OnNext("d");
        }

        public void ReplaySubjectWindowExample()
        {
            var window = TimeSpan.FromMilliseconds(150);
            var subject = new ReplaySubject<string>(window);

            subject.OnNext("a");
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            subject.OnNext("b");
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            subject.OnNext("c");
            subject.Subscribe(Console.WriteLine);
            subject.OnNext("d");
        }

        public void ReplaySubjectInvalidUsageExample()
        {
            var subject = new ReplaySubject<string>(1);

            //subject.Subscribe(Console.WriteLine);

            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnCompleted();
            subject.OnNext("c");

            subject.Subscribe(Console.WriteLine, ()=>Console.WriteLine("Completed"));
        }

        #endregion

        #region BehaviorSubject examples

        public void BehaviorSubjectExample()
        {
            //Need to provide a default value.
            var subject = new BehaviorSubject<string>("a");
            subject.Subscribe(Console.WriteLine);
            subject.OnNext("b");
        }

        public void BehaviorSubjectExample2()
        {
            //Need to provide a default value.
            var subject = new BehaviorSubject<string>("a");

            subject.OnNext("b");
            subject.Subscribe(Console.WriteLine);
            subject.OnNext("c");
            subject.OnNext("d");
            subject.OnCompleted();

        }

        public void BehaviorSubjectCompletedExample()
        {
            //Need to provide a default value.
            var subject = new BehaviorSubject<string>("a");
            subject.OnCompleted();
            subject.Subscribe(Console.WriteLine);
        }

        #endregion

        #region AsyncSubject Examples

        public void AsyncSubjectExample()
        {
            var subject = new AsyncSubject<string>();

            subject.OnNext("a");
            subject.Subscribe(Console.WriteLine);
            subject.OnNext("b");
            subject.OnNext("c");
            subject.OnCompleted();
        }

        #endregion
    }
}