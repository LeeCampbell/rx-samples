using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Disposables;
using System.Linq;
using System.Text;

namespace RxSamples.ConsoleApp
{
    class JoinGroupWindowExamples : ExamplesBase
    {
        public void Buffer_With_Count_returns_IObservable_of_IList_of_T()
        {
            var bufferIdx = 0;
            Observable
                .Interval(TimeSpan.FromMilliseconds(200)).Take(10)
                .BufferWithCount(3)
                .Subscribe(buffer =>
                {
                    var thisBufferIdx = bufferIdx++;
                    Console.WriteLine("--Buffer published");
                    WriteListToConsole(buffer, "Buffer" + thisBufferIdx);

                },
                () => Console.WriteLine("Completed"));
        }
        public void Buffer_With_Time_returns_IObservable_of_IList_of_T()
        {
            var bufferIdx = 0;
            Observable
                .Interval(TimeSpan.FromMilliseconds(200)).Take(10)
                .BufferWithTime(TimeSpan.FromMilliseconds(500))
                .Subscribe(window =>
                {
                    var thisBufferIdx = bufferIdx++;
                    Console.WriteLine("--Buffer published");
                    WriteListToConsole(window, "Buffer" + thisBufferIdx);

                },
                () => Console.WriteLine("Completed"));
        }

        public void Window_With_Count_returns_IObservable_of_IObservable_of_T()
        {
            var windowIdx = 0;
            Observable
                .Interval(TimeSpan.FromMilliseconds(200)).Take(10)
                .WindowWithCount(3)
                .Subscribe(window =>
                               {
                                   var thisWindowIdx = windowIdx++;
                                   Console.WriteLine("--Starting new window");
                                   WriteStreamToConsole(window, "Window" + thisWindowIdx);

                               },
                           () => Console.WriteLine("Completed"));
        }
        public void Window_With_Time_returns_IObservable_of_IObservable_of_T()
        {
            var windowIdx = 0;
            Observable
                .Interval(TimeSpan.FromMilliseconds(200)).Take(10)
                .WindowWithTime(TimeSpan.FromMilliseconds(500))
                .Subscribe(window =>
                               {
                                   var thisWindowIdx = windowIdx++;
                                   Console.WriteLine("--Starting new window");
                                   WriteStreamToConsole(window, "Window" + thisWindowIdx);

                               },
                           () => Console.WriteLine("Completed"));
        }

        public void Switch_rolls_Windows_back_into_their_source()
        {
            //is the same as Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(10)
            var switchedWindow = Observable
                .Interval(TimeSpan.FromMilliseconds(200)).Take(10)
                .WindowWithTime(TimeSpan.FromMilliseconds(500))
                .Switch();

            WriteStreamToConsole(switchedWindow, "Switched window");
        }

        public void Join_with_no_closing_left()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(6);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65)); ;
            var joinedStream = left
                .Join(
                    right,
                    _ => Observable.Never<Unit>(),
                    _ => Observable.Empty<Unit>(),
                    (leftValue, rightValue) => new { Left = leftValue, Right = rightValue, Time = DateTime.Now.ToString("o") });
            WriteStreamToConsole(joinedStream, "Join");

        }
        public void Join_with_immediate_closing_left()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(6);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65)); ;
            var joinedStream = left
                .Join(
                    right,
                    _ => Observable.Empty<Unit>(),
                    _ => Observable.Empty<Unit>(),
                    (leftValue, rightValue) => new { Left = leftValue, Right = rightValue, Time = DateTime.Now.ToString("o") });
            WriteStreamToConsole(joinedStream, "Join");
        }

        public void Join_with_non_overlapping_left_windows()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(6).Publish().RefCount();
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65)); ;
            var joinedStream = left
                .Join(
                    right,
                    _ => left,
                    _ => Observable.Empty<Unit>(),
                    (leftValue, rightValue) => new { Left = leftValue, Right = rightValue, Time = DateTime.Now.ToString("o") });
            WriteStreamToConsole(joinedStream, "Join");
        }

        private static IObservable<TResult> MyCombineLatest<TLeft, TRight, TResult>(IObservable<TLeft> left, IObservable<TRight> right, Func<TLeft, TRight, TResult> resultSelector)
        {
            var refcountedLeft = left.Publish().RefCount();
            var refcountedRight = right.Publish().RefCount();
            return Observable.Join(
                    refcountedLeft,
                    refcountedRight,
                    _ => refcountedLeft,
                    _ => refcountedRight,
                    resultSelector);
        }
        public void Join_as_CombineLatest()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(7);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65));
            var joinedStream = MyCombineLatest(left, right, (leftValue, rightValue) => new { Left = leftValue, Right = rightValue });
            WriteStreamToConsole(joinedStream, "Join as CombineLatest");
        }
        public void Actual_CombineLatest()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(7);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65));
            var joinedStream = Observable.CombineLatest(left, right, (leftValue, rightValue) => new { Left = leftValue, Right = rightValue });
            WriteStreamToConsole(joinedStream, "Join as CombineLatest");
        }

        public void GroupJoin_with_no_closing_left()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(6);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65)); ;
            var joinedStream = left
                .GroupJoin(
                    right,
                    _ => Observable.Never<Unit>(),
                    _ => Observable.Empty<Unit>(),
                    (leftValue, rightValues) =>
                    {
                        WriteStreamToConsole(rightValues, string.Format("Window{0}'s values", leftValue));
                        return rightValues.Aggregate((s1, s2) => s1 + s2);  //Just for fun return a concatenating string.
                    });
            WriteStreamToConsole(joinedStream.Merge(), "GroupJoin");
        }
        public void GroupJoin_with_no_closing_right()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(6);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65)); ;
            var joinedStream = left
                .GroupJoin(
                    right,
                    _ => Observable.Empty<Unit>(),
                    _ => Observable.Never<Unit>(),
                    (leftValue, rightValues) =>
                    {
                        WriteStreamToConsole(rightValues, string.Format("Window{0}'s values", leftValue));
                        return rightValues.Aggregate((s1, s2) => s1 + s2)
                            .Catch((InvalidOperationException ex) => Observable.Empty<string>());
                    });
            WriteStreamToConsole(joinedStream.Merge(), "GroupJoin");
        }

        public IObservable<TResult> MyJoin<TLeft, TRight, TLeftDuration, TRightDuration, TResult>(
            IObservable<TLeft> left,
            IObservable<TRight> right,
            Func<TLeft, IObservable<TLeftDuration>> leftDurationSelector,
            Func<TRight, IObservable<TRightDuration>> rightDurationSelector,
            Func<TLeft, TRight, TResult> resultSelector)
        {
            return Observable.GroupJoin
                (
                    left,
                    right,
                    leftDurationSelector,
                    rightDurationSelector,
                    (leftValue, rightValues) => rightValues.Select(rightValue => resultSelector(leftValue, rightValue))
                )
                .Merge();
        }
        public void MyJoin_with_no_closing_left()
        {
            var left = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(6);
            var right = Observable.Interval(TimeSpan.FromMilliseconds(400)).Take(3).Select(i => Char.ConvertFromUtf32((int)i + 65)); ;
            var joinedStream = MyJoin(
                    left,
                    right,
                    _ => Observable.Never<Unit>(),
                    _ => Observable.Empty<Unit>(),
                    (leftValue, rightValue) => new { Left = leftValue, Right = rightValue, Time = DateTime.Now.ToString("o") });
            WriteStreamToConsole(joinedStream, "MyJoin");

        }


        private IObservable<IObservable<T>> MyWindowWithTime<T>(IObservable<T> source, TimeSpan windowPeriod)
        {
            return Observable.CreateWithDisposable<IObservable<T>>(o =>
                {
                    var windower = new Subject<long>();
                    var intervals = Observable.Concat(
                            Observable.Return(0L),
                            Observable.Interval(windowPeriod)
                        )
                        .Publish()
                        .RefCount();

                    var subscription = Observable.GroupJoin
                        (
                            windower,
                            source.Do(_ => { }, windower.OnCompleted),
                            _ => windower,
                            _ => Observable.Empty<Unit>(),
                            (left, sourceValues) => sourceValues
                        )
                        .Subscribe(o);
                    var intervalSubscription = intervals.Subscribe(windower);
                    return new CompositeDisposable(subscription, intervalSubscription);
                });
        }
        public void MyWindow_With_Time_returns_IObservable_of_IObservable_of_T()
        {
            var windowIdx = 0;
            var source = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(10);
            MyWindowWithTime(source, TimeSpan.FromMilliseconds(500))
                .Subscribe(window =>
                {
                    var thisWindowIdx = windowIdx++;
                    Console.WriteLine("--Starting new window");
                    WriteStreamToConsole(window, "Window" + thisWindowIdx);

                },
                () => Console.WriteLine("Completed"));
        }

    }
}
