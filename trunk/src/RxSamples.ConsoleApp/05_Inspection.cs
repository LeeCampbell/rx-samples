using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxSamples.ConsoleApp
{
    class Inspection
    {
        public void Any_without_a_value()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var any = subject.Any();
            any.Subscribe(b => Console.WriteLine("The subject has any values? {0}", b));
            subject.OnCompleted();
        }

        public void Any_with_a_value()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var any = subject.Any();

            any.Subscribe(b => Console.WriteLine("The subject has any values? {0}", b));

            subject.OnNext(1);
            subject.OnCompleted();
        }

        public void Any_with_an_error()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine,
                ex => Console.WriteLine("subject OnError : {0}", ex),
                () => Console.WriteLine("Subject completed"));
            var any = subject.Any();

            any.Subscribe(b => Console.WriteLine("The subject has any values? {0}", b),
                ex => Console.WriteLine(".Any() OnError : {0}", ex),
                () => Console.WriteLine(".Any() completed"));

            subject.OnError(new Exception("Fail"));
        }

        public void Any_with_a_value_then_error()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine,
                ex => Console.WriteLine("subject OnError : {0}", ex),
                () => Console.WriteLine("Subject completed"));
            var any = subject.Any();

            any.Subscribe(b => Console.WriteLine("The subject has any values? {0}", b),
                ex => Console.WriteLine(".Any() OnError : {0}", ex),
                () => Console.WriteLine(".Any() completed"));

            subject.OnNext(1);
            subject.OnError(new Exception("Fail"));
        }

        public void Any_predicate_with_a_value()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var anyPredicate = subject.Any(i => i > 2);
            anyPredicate.Subscribe(b => Console.WriteLine("The subject has any values? {0}", b));

            var whereAny = subject.Where(i => i > 2).Any();
            whereAny.Subscribe(b => Console.WriteLine("The subject has any values? {0}", b));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();
        }

        public void Custom_Any_from_Create()
        {
            
        }

        public void All_without_a_value()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var all = subject.All(i => i < 5);
            all.Subscribe(b => Console.WriteLine("All values less than 5? {0}", b));
            subject.OnCompleted();
        }

        public void All_with_values()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var all = subject.All(i => i < 5);
            all.Subscribe(b => Console.WriteLine("All values less than 5? {0}", b));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnCompleted();
        }

        public void All_with_failing_values()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var all = subject.All(i => i < 5);
            all.Subscribe(
                b => Console.WriteLine("All values less than 5? {0}", b),
                () => Console.WriteLine("all completed"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(6);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnCompleted();
        }


        public void All_with_an_error()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine,
                ex => Console.WriteLine("subject OnError : {0}", ex),
                () => Console.WriteLine("Subject completed"));
            var all = subject.All(i => i < 5);

            all.Subscribe(b => Console.WriteLine("All values less than 5? {0}", b),
                ex => Console.WriteLine(".All() OnError : {0}", ex),
                () => Console.WriteLine(".All() completed"));

            subject.OnError(new Exception("Fail"));
        }

        public void IsEmpty_is_just_Any_false()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine,
                ex => Console.WriteLine("subject OnError : {0}", ex),
                () => Console.WriteLine("Subject completed"));
            var isEmpty = subject.All(_ => false);

            isEmpty.Subscribe(b => Console.WriteLine("Is Subject empty? {0}", b),
                ex => Console.WriteLine("isEmpty OnError : {0}", ex),
                () => Console.WriteLine("isEmpty completed"));

            //subject.OnNext(1);
            subject.OnCompleted();
        }

        public void Contains_with_value()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var contains = subject.Contains(2);
            contains.Subscribe(
                b => Console.WriteLine("Contains the value 2? {0}", b),
                () => Console.WriteLine("contains completed"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            subject.OnCompleted();
        }


        public void DefaultIfEmpty_is_a_noop_if_there_are_values()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var defaultIfEmpty = subject.DefaultIfEmpty();
            defaultIfEmpty.Subscribe(
                b => Console.WriteLine("defaultIfEmpty value: {0}", b),
                () => Console.WriteLine("defaultIfEmpty completed"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            subject.OnCompleted();
        }

        public void DefaultIfEmpty_returns_Default_T_if_empty()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var defaultIfEmpty = subject.DefaultIfEmpty();
            defaultIfEmpty.Subscribe(
                b => Console.WriteLine("defaultIfEmpty value: {0}", b),
                () => Console.WriteLine("defaultIfEmpty completed"));

            var default42IfEmpty = subject.DefaultIfEmpty(42);
            default42IfEmpty.Subscribe(
                b => Console.WriteLine("default42IfEmpty value: {0}", b),
                () => Console.WriteLine("default42IfEmpty completed"));

            subject.OnCompleted();
        }

        public void ElementAt1()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var elementAt1 = subject.ElementAt(1);
            elementAt1.Subscribe(
                b => Console.WriteLine("elementAt1 value: {0}", b),
                () => Console.WriteLine("elementAt1 completed"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            subject.OnCompleted();
        }

        public void ElementAt1_is_Skip1Take1()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var elementAt1 = subject.ElementAt(1);
            elementAt1.Subscribe(
                b => Console.WriteLine("elementAt1 value: {0}", b),
                () => Console.WriteLine("elementAt1 completed"));
            var skip1Take1 = subject.Skip(1).Take(1);
            skip1Take1.Subscribe(
                b => Console.WriteLine("skip1Take1 value: {0}", b),
                () => Console.WriteLine("skip1Take1 completed"));


            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            subject.OnCompleted();
        }

        public void ElementAtLengthPlus1()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var elementAt10 = subject.ElementAt(10);
            elementAt10.Subscribe(
                b => Console.WriteLine("elementAt10 value: {0}", b),
                ex => Console.WriteLine("elementAt10 OnError : {0}", ex),
                () => Console.WriteLine("elementAt10 completed"));

            var skip10Take1 = subject.Skip(10).Take(1);
            skip10Take1.Subscribe(
                b => Console.WriteLine("skip10Take1 value: {0}", b),
                ex => Console.WriteLine("skip10Take1 OnError : {0}", ex),
                () => Console.WriteLine("skip10Take1 completed"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            subject.OnCompleted();
        }

        public void ElementAtOrDefault()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Subject completed"));
            var elementAt10OrDefault = subject.ElementAtOrDefault(10);
            elementAt10OrDefault.Subscribe(
                b => Console.WriteLine("elementAt10OrDefault value: {0}", b),
                ex => Console.WriteLine("elementAt10OrDefault OnError : {0}", ex),
                () => Console.WriteLine("elementAt10OrDefault completed"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            subject.OnCompleted();
        }

        public void SequenceEqual()
        {
            var subject1 = new Subject<int>();
            subject1.Subscribe(
                i=>Console.WriteLine("subject1.OnNext({0})", i),
                () => Console.WriteLine("subject1 completed"));
            var subject2 = new Subject<int>();
            subject2.Subscribe(
                i=>Console.WriteLine("subject2.OnNext({0})", i),
                () => Console.WriteLine("subject2 completed"));


            var areEqual = subject1.SequenceEqual(subject2);
            areEqual.Subscribe(
                i => Console.WriteLine("areEqual.OnNext({0})", i),
                () => Console.WriteLine("areEqual completed"));

            subject1.OnNext(1);
            subject1.OnNext(2);

            subject2.OnNext(1);
            subject2.OnNext(2);
            subject2.OnNext(3);

            subject1.OnNext(3);

            subject1.OnCompleted();
            subject2.OnCompleted();
        }
    }
}
