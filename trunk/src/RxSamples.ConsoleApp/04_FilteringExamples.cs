using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxSamples.ConsoleApp
{
    class FilteringExamples
    {
        public void Where()
        {
            var oddNumbers = Observable.Range(0, 10).Where(i => i % 2 == 0);
            oddNumbers.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }

        public void Distinct_will_only_serve_a_value_once()
        {
            var subject = new Subject<int>();
            var distinct = subject.Distinct();
            
            subject.Subscribe(
                i => Console.WriteLine("{0}", i),
                () => Console.WriteLine("subject.OnCompleted()"));
            distinct.Subscribe(
                i => Console.WriteLine("distinct.OnNext({0})", i),
                () => Console.WriteLine("distinct.OnCompleted()"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(4);
            subject.OnCompleted();
        }

        public class Account
        {
            public int AccountId { get; set; }

        }
        public void Distinct_with_KeySelector()
        {
            var subject = new Subject<Account>();
            var distinct = subject.Distinct(acc => acc.AccountId);
        }

        public void DistinctUntilChanged_can_serve_the_same_value_more_than_once()
        {
            var subject = new Subject<int>();
            var distinct = subject.DistinctUntilChanged();

            subject.Subscribe(
                i => Console.WriteLine("{0}", i),
                () => Console.WriteLine("subject.OnCompleted()"));
            distinct.Subscribe(
                i => Console.WriteLine("distinct.OnNext({0})", i),
                () => Console.WriteLine("distinct.OnCompleted()"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(4);
            subject.OnCompleted();
        }

        public void IgnoreElements()
        {
            var subject = new Subject<int>();

            var noElements = subject.Where(_ => false);
            //var noElements = subject.IgnoreElements();
            subject.Subscribe(
                i => Console.WriteLine("subject.OnNext({0})", i),
                () => Console.WriteLine("subject.OnCompleted()"));
            noElements.Subscribe(
                i => Console.WriteLine("noElements.OnNext({0})", i),
                () => Console.WriteLine("noElements.OnCompleted()"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }

        public void Skip()
        {
            Observable.Range(0, 10)
                .Skip(3)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }

        public void Take()
        {
            Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Take(3)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }

        public void SkipWhile()
        {
            var subject = new Subject<int>();
            subject
                .SkipWhile(i => i < 4)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnNext(3);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnNext(0);

            subject.OnCompleted();
        }
        public void TakeWhile()
        {
            var subject = new Subject<int>();
            subject
                .TakeWhile(i => i < 4)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnNext(3);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnNext(0);

            subject.OnCompleted();
        }

        public void SkipLast()
        {
            var subject = new Subject<int>();
            subject
                .SkipLast(2)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            Console.WriteLine("Pushing 1");
            subject.OnNext(1);
            Console.WriteLine("Pushing 2");
            subject.OnNext(2);
            Console.WriteLine("Pushing 3");
            subject.OnNext(3);
            Console.WriteLine("Pushing 4");
            subject.OnNext(4);
            subject.OnCompleted();
        }

        public void TakeLast()
        {
            var subject = new Subject<int>();
            subject
                .TakeLast(2)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            Console.WriteLine("Pushing 1");
            subject.OnNext(1);
            Console.WriteLine("Pushing 2");
            subject.OnNext(2);
            Console.WriteLine("Pushing 3");
            subject.OnNext(3);
            Console.WriteLine("Pushing 4");
            subject.OnNext(4);
            Console.WriteLine("Completing");
            subject.OnCompleted();
        }

        public void SkipUntil()
        {
            var subject = new Subject<int>();
            var otherSubject = new Subject<Unit>();
            subject
                .SkipUntil(otherSubject)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            otherSubject.OnNext(Unit.Default);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnNext(6);
            subject.OnNext(7);
            subject.OnNext(8);

            subject.OnCompleted();
        }

        public void TakeUntil()
        {
            var subject = new Subject<int>();
            var otherSubject = new Subject<Unit>();
            subject
                .TakeUntil(otherSubject)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            otherSubject.OnNext(Unit.Default);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnNext(6);
            subject.OnNext(7);
            subject.OnNext(8);

            subject.OnCompleted();
        }
    }
}
