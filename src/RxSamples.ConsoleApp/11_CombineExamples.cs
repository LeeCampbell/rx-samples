using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Joins;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxSamples.ConsoleApp
{
    public class CombineExamples
    {
        public void Concat_continues_with_second_sequence()
        {
            //Generate values 0,1,2 
            var s1 = Observable.Range(0, 3);
            //Generate values 5,6,7,8,9 
            var s2 = Observable.Range(5, 5);
            s1.Concat(s2)
                .Subscribe(Console.WriteLine);

            //Similar to OnErrorResumeNext
            //s1.OnErrorResumeNext(s2)
            //  .Subscribe(Console.WriteLine);

            /* Returns:
             * 
             * stream1 --0--1--2-|
             * stream2           -5--6--7--8--|
             * 
             * result  --0--1--2--5--6--7--8--|
             */
        }

        public void Repeat_continues_with_self()
        {
            //Generate values 0,1,2 
            var source = Observable.Range(0, 3);
            var result = source.Repeat(3);

            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));

            /* stream1 -0--1--2-|
             *                  -0--1--2-|
             *                           -0--1--2-|
             * 
             * result  -0--1--2--0--1--2--0--1--2-|
             */
        }

        public void StartWith_allows_values_to_be_prepended_to_a_sequence()
        {
            //Generate values 0,1,2 
            var source = Observable.Range(0, 3);
            var result = source.StartWith(-3, -2, -1);
            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));

        }

        public void Amb_will_return_values_from_the_first_sequence_to_produce_values()
        {
            var s1 = new Subject<int>();
            var s2 = new Subject<int>();
            var s3 = new Subject<int>();

            var result = Observable.Amb(s1, s2, s3);

            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));

            s1.OnNext(1);
            s2.OnNext(2);
            s3.OnNext(3);
            s1.OnNext(1);
            s2.OnNext(2);
            s3.OnNext(3);
            s1.OnCompleted();
            s2.OnCompleted();
            s3.OnCompleted();
        }


        public void Search_With_merge()
        {
            IObservable<string> searchValues = Observable.Interval(TimeSpan.FromSeconds(1)).Take(3).Select(i => i.ToString());
            var search = from searchText in searchValues
                         select SearchResults(searchText);

            var subscription = search.Merge()
                .Subscribe(
                    Console.WriteLine);
            Console.ReadLine();

            subscription.Dispose();
        }

        public void Search_corrected_with_Switch()
        {
            var searchValues = Observable.Interval(TimeSpan.FromSeconds(1)).Take(3).Select(i => i.ToString());
            var search = from searchText in searchValues
                         select SearchResults(searchText);

            var subscription = search.Switch()
                .Subscribe(
                    Console.WriteLine);
            Console.ReadLine();

            subscription.Dispose();
        }

        public IObservable<string> SearchResults(string query)
        {
            return Observable.Interval(TimeSpan.FromMilliseconds(200)).Select(i => string.Format("{0}{1}", query, i)).Take(20);
        }

        public void CombineLatest()
        {
            IObservable<bool> webServerStatus = Observable.Interval(TimeSpan.FromMilliseconds(250)).Select(i => i % 2 == 0);
            IObservable<bool> databaseStatus = Observable.Interval(TimeSpan.FromMilliseconds(350)).Select(i => i % 2 == 0);

            var systemStatus = webServerStatus
                .CombineLatest(
                    databaseStatus,
                    (webStatus, dbStatus) => webStatus && dbStatus)
                .DistinctUntilChanged();

            var subscription = systemStatus.Subscribe(
                Console.WriteLine);

            Console.ReadLine();
            subscription.Dispose();
        }

        public void Zip_example()
        {
            //Generate values 0,1,2 
            var nums = Observable.Interval(TimeSpan.FromMilliseconds(250))
                .Take(3);
            //Generate values a,b,c,d,e,f 
            var chars = Observable.Interval(TimeSpan.FromMilliseconds(150))
                .Take(6)
                .Select(i => Char.ConvertFromUtf32((int)i + 97));
            nums
                .Zip(chars, (lhs, rhs) => new { Left = lhs, Right = rhs })
                .Subscribe(Console.WriteLine);
        }

        public void Zip_with_skip1()
        {
            var mm = new Subject<Coord>();
            var s1 = mm.Skip(1);

            var delta = mm.Zip(s1,
                               (prev, curr) => new Coord
                                    {
                                        X = curr.X - prev.X,
                                        Y = curr.Y - prev.Y
                                    });

            delta.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));

            mm.OnNext(new Coord { X = 0, Y = 0 });
            mm.OnNext(new Coord { X = 1, Y = 0 }); //Move across 1
            mm.OnNext(new Coord { X = 3, Y = 2 }); //Diagonally up 2
            mm.OnNext(new Coord { X = 0, Y = 0 }); //Back to 0,0
            mm.OnCompleted();
        }
        public class Coord
        {
            public int X { get; set; }
            public int Y { get; set; }
            public override string ToString()
            {
                return string.Format("{0},{1}", X, Y);
            }
        }

        public void Basic_AndThenWhen_is_just_zip()
        {
            var left = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(10);


            var pattern = left.And(right);
            var plan = pattern.Then((lhs, rhs) => string.Format("values {0} & {1}", lhs, rhs));
            var result = Observable.When(plan);

            //or
            //var result = Observable.When(left.And(right).Then((lhs, rhs) => string.Format("values {0} & {1}", lhs, rhs)));

            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
        }

        public void ZipZip()
        {
            var one = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
            var two = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(10);
            var three = Observable.Interval(TimeSpan.FromMilliseconds(150)).Take(14);

            var zippedSequence = one
                .Zip(two, (lhs, rhs) => new { One = lhs, Two = rhs })
                .Zip(three, (lhs, rhs) => new { One = lhs.One, Two = lhs.Two, Three = rhs });

            zippedSequence.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
        }

        public void AndAndThenWhen_is_ZipZip()
        {
            var one = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
            var two = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(10);
            var three = Observable.Interval(TimeSpan.FromMilliseconds(150)).Take(14);

            var pattern = one.And(two).And(three);
            var plan = pattern.Then((first, second, third) => new { One = first, Two = second, Three = third });
            var zippedSequence = Observable.When(plan);

            //or
            zippedSequence = Observable.When(
                one.And(two)
                    .And(three)
                    .Then((first, second, third) => new { One = first, Two = second, Three = third })
                );

            zippedSequence.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("Completed"));
        }

        public void Composite_AndThenWhen()
        {
            var left = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(10);


            Plan<string> plan1 = left.And(right).Then((lhs, rhs) => string.Format("1)values  {0} & {1}", lhs, rhs));
            var plan2 = left.Where(i => i % 2 == 0)
                            .And(right.Where(i => i % 2 == 0))
                            .Then((lhs, rhs) => string.Format("2)values  {0} & {1}", lhs, rhs));
            Observable.When(plan1, plan2)
                      .Subscribe(
                        Console.WriteLine,
                        () => Console.WriteLine("Completed"));

        }
    }
}
