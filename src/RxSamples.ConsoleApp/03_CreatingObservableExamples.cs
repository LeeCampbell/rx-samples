using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

// ReSharper disable InconsistentNaming
namespace RxSamples.ConsoleApp
{
    class CreatingObservableExamples : ExamplesBase
    {
        public void Return_is_like_a_completed_replay_one_subject()
        {
            var subject = new ReplaySubject<string>(1);
            subject.OnNext("Value");
            subject.OnCompleted();
            WriteStreamToConsole(subject, "subject");
            //Or
            var obReturn = Observable.Return("Value");
            WriteStreamToConsole(obReturn, "Observable.Return");
        }

        public void Empty_is_like_an_empty_completed_ReplaySubject()
        {
            var subject = new ReplaySubject<string>();
            subject.OnCompleted();
            WriteStreamToConsole(subject, "ReplaySubject");

            //Or
            var empty = Observable.Empty<string>();
            WriteStreamToConsole(empty, "empty");
        }

        public void Never_is_like_a_Subject()
        {
            var subject = new Subject<string>();
            WriteStreamToConsole(subject, "subject");
            //Or
            var never = Observable.Never<string>();
            WriteStreamToConsole(never, "never");
        }

        public void Throw_is_like_ReplaySubject_that_throws_when_subsribed_to()
        {
            var subject = new ReplaySubject<string>();
            subject.OnError(new Exception());
            WriteStreamToConsole(subject, "subject");
            //Or
            var throws = Observable.Throw<string>(new Exception());
            WriteStreamToConsole(throws, "throws");
            //See the next few methods regarding Observable.Create for a more realistic way of reproducing.
        }


        #region Observable Create & Defer. Compare blocking vs Non-blocking
        private IObservable<string> BlockingMethod()
        {
            var subject = new ReplaySubject<string>();
            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnCompleted();
            Thread.Sleep(1000);
            return subject;
        }
        private IObservable<string> NonBlocking()
        {
            return Observable.Create<string>(
                (IObserver<string> observer) =>
                {
                    observer.OnNext("a");
                    observer.OnNext("b");
                    observer.OnCompleted();
                    Thread.Sleep(1000);
                    return () => Console.WriteLine("Observer has unsubscribed");
                });
        }

        public static IObservable<T> Empty<T>()
        {
            return Observable.Create<T>(o =>
            {
                o.OnCompleted();
                return Disposable.Empty;
            });
        }
        public static IObservable<T> Return<T>(T value)
        {
            return Observable.Create<T>(o =>
            {
                o.OnNext(value);
                o.OnCompleted();
                return Disposable.Empty;
            });
        }
        public static IObservable<T> Never<T>()
        {
            return Observable.Create<T>(o =>
            {
                return Disposable.Empty;
            });
        }
        public static IObservable<T> Throws<T>(Exception exception)
        {
            return Observable.Create<T>(o =>
            {
                o.OnError(exception);
                return Disposable.Empty;
            });
        }

        //public void NonBlocking_event_driven()
        //{
        //    Timer timer;
        //    var ob = Observable.Create<string>(
        //        observer =>
        //        {
        //            var initialState = "Timer State";
        //            timer = new Timer(
        //                tickState => observer.OnNext(tickState.ToString()),
        //                initialState,
        //                TimeSpan.FromSeconds(1),
        //                TimeSpan.FromSeconds(1));
        //            //Timer implements IDisposable.
        //            //return timer;
        //            return Disposable.Empty;
        //        });
        //    var subsciption = ob.Subscribe(Console.WriteLine);
        //    Console.ReadLine();
        //    subsciption.Dispose();
        //    Console.ReadLine();
        //}

        public void NonBlocking_event_driven()
        {
            //var ob = Observable.Create<string>(
            //    observer =>
            //    {
            //        var timer = new System.Timers.Timer();
            //        timer.Enabled = true;
            //        timer.Interval = 100;
            //        timer.Elapsed += (s, e) => observer.OnNext("tick");
            //        timer.Elapsed += OnTimerElapsed;
            //        timer.Start();

            //        //return timer;
            //        return Disposable.Empty;
            //    });
            var ob = Observable.Create<string>(
            observer =>
            {
                var timer = new System.Timers.Timer();
                timer.Enabled = true;
                timer.Interval = 100;
                timer.Elapsed += OnTimerElapsed;
                timer.Start();

                return () =>
                {
                    timer.Elapsed -= OnTimerElapsed;
                    timer.Dispose();
                };
            });


            var subsciption = ob.Subscribe(Console.WriteLine);
            Console.ReadLine();
            subsciption.Dispose();
            Console.ReadLine();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine(e.SignalTime);
        }

        public void Blocking_vs_NonBlocking_via_Observable_Create()
        {
            Console.WriteLine("Requesting observable via blocking call @ {0:o}", DateTime.Now);
            var blocked = BlockingMethod();
            Console.WriteLine("Recieved observable via blocking call @ {0:o}", DateTime.Now);
            WriteStreamToConsole(blocked, "blocked");

            Console.WriteLine("Requesting observable via nonblocking call @ {0:o}", DateTime.Now);
            var nonblocked = NonBlocking();
            Console.WriteLine("Recieved observable via nonblocking call @ {0:o}", DateTime.Now);
            WriteStreamToConsole(nonblocked, "nonblocked");

        }

        public void Defer_to_make_a_blocking_non_blocking_like_Observable_Create()
        {
            Console.WriteLine("Requesting observable via blocking call @ {0:o}", DateTime.Now);
            var blocked = BlockingMethod();
            Console.WriteLine("Recieved observable via blocking call @ {0:o}", DateTime.Now);
            WriteStreamToConsole(blocked, "blocked");

            Console.WriteLine("Requesting observable via defer call @ {0:o}", DateTime.Now);
            var defer = Observable.Defer<string>(BlockingMethod);
            Console.WriteLine("Recieved observable via defer call @ {0:o}", DateTime.Now);
            WriteStreamToConsole(defer, "defer");

        }

        public void Use_Generate_to_make_Range()
        {
            var range = Range(5, 10);
            range.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));

            range = Observable.Range(5, 10).Where(i => i % 2 == 0);
            range.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }

        public static IObservable<int> Range(int start, int count)
        {
            var max = start + count;
            return Observable.Generate(
                start,
                value => value < max,
                value => value + 1,
                value => value);
        }

        public static IObservable<long> Interval(TimeSpan period)
        {
            //return Observable.Timer(TimeSpan.Zero, period);
            return Observable.Generate(
                0l,
                i => true,
                i => i + 1,
                i => i,
                i => period);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, TimeSpan period)
        {
            return Observable.Generate(
                0l,
                i => true,
                i => i + 1,
                i => i,
                i => i == 0 ? dueTime : period);
        }

        public static IObservable<long> Timer(TimeSpan dueTime)
        {
            return Observable.Generate(
                0l,
                i => i < 1,
                i => i + 1,
                i => i,
                i => dueTime);
        }

        #endregion

        public void TaskT()
        {
            var t = new Task<int>(() =>
                                      {
                                          Thread.SpinWait(100000000);
                                          return 42;
                                      });

            Console.WriteLine("{0:o}", DateTimeOffset.Now);
            t.Start();
            Console.WriteLine(t.Result);
            Console.WriteLine("{0:o}", DateTimeOffset.Now);
            Console.WriteLine(t.Result);
            Console.WriteLine("{0:o}", DateTimeOffset.Now);
        }

        public void ToObservable_from_TaskT()
        {
            Func<int> function = () =>
                                {
                                    Thread.SpinWait(100000000);
                                    return 42;
                                };
            //var t = new Task<int>(function);
            //t.Start();

            //or
            var t = Task.Factory.StartNew(() => "Test");
            var source = t.ToObservable();
            source.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("completed"));
        }

        public void ToObservable_from_TaskT_late_start()
        {
            Func<int> function = () =>
            {
                Thread.SpinWait(100000000);
                return 42;
            };
            var t = new Task<int>(function);

            var source = t.ToObservable();
            source.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("completed"));

            t.Start();
        }

        public void ToObservable_from_Task()
        {
            var t = Task.Factory.StartNew(() => { });
            var source = t.ToObservable();
            source.Subscribe(
                _ => Console.WriteLine("."),
                () => Console.WriteLine("completed"));
        }

        public void ToObservable_can_convert_IEnumables()
        {
            var enumT = new List<string>();
            enumT.Add("a");
            enumT.Add("b");
            enumT.Add("c");
            var fromEnum = enumT.ToObservable();

            WriteStreamToConsole(fromEnum, "fromEnum");
        }


        #region FromAsync samples
        public void Sync_WebRequest()
        {
            var webRequest = WebRequest.Create("http://LeeCampbell.blogspot.com");
            var response = webRequest.GetResponse();
            var contentStream = response.GetResponseStream();
            var reader = new StreamReader(contentStream);
            var content = reader.ReadToEnd();

            Console.WriteLine(content);
        }
        public void APM_WebRequest()
        {
            Console.WriteLine("OMG. Look at all the code you need to write!!");
            //http://msdn.microsoft.com/en-us/library/86wf6409.aspx
        }

        private const string TestFilePath =
            @"C:\Users\Lee\Documents\svn\rx-samples\packages\Rx-Main.1.0.11226\lib\SL5\System.Reactive.XML";
        public void Sync_ReadFile()
        {
            var fs = new FileStream(TestFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            Console.WriteLine("{0:o} Running file read sync.", DateTimeOffset.Now);
            var reader = new StreamReader(fs);
            var content = reader.ReadToEnd();
            Console.WriteLine("{1:o} Number of bytes read={0}", content.Length, DateTimeOffset.Now);
            Console.WriteLine("Last byte is {0}", content.Last());

            //Console.WriteLine(content);
        }

        public class State
        {
            public byte[] Data { get; set; }
            public int Index { get; set; }
            public int BufferSize { get; set; }
            public byte[] Buffer { get; set; }
            public FileStream FileStream { get; set; }
        }

        public void APM_ReadFile()
        {
            var fs = new FileStream(TestFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,
                FileOptions.Asynchronous);

            var state = new State
            {
                Index = 0,
                BufferSize = (int)fs.Length,
                Buffer = new byte[fs.Length],
                FileStream = fs
            };
            var token = fs.BeginRead(
                state.Buffer,
                0,
                state.BufferSize,
                (IAsyncResult asyncResult) =>
                {
                    var asyncState = (State)asyncResult.AsyncState;
                    var bytesRead = state.FileStream.EndRead(asyncResult);
                    asyncState.FileStream.Close();
                    Console.WriteLine("{1:o} Number of bytes read={0}", bytesRead, DateTimeOffset.Now);
                    Console.WriteLine("Last byte is {0}", state.Buffer.Last());
                    //Console.WriteLine(BitConverter.ToString(asyncState.Buffer, 0, bytesRead));
                },
                state);

            Console.WriteLine("{1:o} Running file read async. token.IsCompleted : {0}", token.IsCompleted, DateTimeOffset.Now);
        }

        //TODO: Surely there is a way to do buffered APM reads of an entire file? Or should you just set the buffer to the file length if you are not doing some sort of file copy?
        public void APM_ReadFile_Todo()
        {
            var bufferSize = 4096;
            var fs = new FileStream(TestFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize,
                FileOptions.Asynchronous);

            var state = new State
            {
                Index = 0,
                BufferSize = bufferSize,
                Buffer = new byte[bufferSize],
                FileStream = fs
            };
            fs.BeginRead(
                state.Buffer,
                state.Index,
                state.BufferSize,
                BufferReadComplete,
                state);

            //Console.WriteLine(content);
        }

        private void BufferReadComplete(IAsyncResult asyncResult)
        {
            var state = (State)asyncResult.AsyncState;

            Int32 bytesRead = state.FileStream.EndRead(asyncResult);
            if (bytesRead > 0)
            {
                state.Index += bytesRead;
                //Copy buffer to Data.
                //state.Data = Array.

                state.FileStream.BeginRead(
                    state.Buffer,
                    state.Index,
                    state.BufferSize,
                    BufferReadComplete,
                    state);
            }
            else
            {
                state.FileStream.Close();
            }

            // Now, it is OK to access the byte array and show the result.
            Console.WriteLine("Number of bytes read={0}", bytesRead);
            //Console.WriteLine(BitConverter.ToString(s_data, 0, bytesRead));
        }

        public void APM_WithRx_FileRead()
        {
            var bufferSize = 4096;
            var stream = new FileStream(TestFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize,
                FileOptions.Asynchronous);
            var fileLength = (int)stream.Length;
            //Func<byte[], int, int, IObservable<int>>
            var read = Observable.FromAsyncPattern<byte[], int, int, int>(stream.BeginRead, stream.EndRead);
            var buffer = new byte[fileLength];
            var bytesReadStream = read(buffer, 0, fileLength);
            bytesReadStream.Subscribe(
                byteCount =>
                {
                    Console.WriteLine("{1:o} Number of bytes read={0}", byteCount, DateTimeOffset.Now);
                    Console.WriteLine("Last byte is {0}", buffer.Last());
                });
            //byteCount => Console.WriteLine("Read {0} bytes. buffer will have all data now. ", byteCount));
        }

        //static IObservable<byte[]> AsyncRead(Stream stream, int bufferSize)
        //{
        //    return Observable.Iterate<byte[]>(
        //        result => AsyncReadHelper(result, stream, bufferSize));
        //}

        //private static IEnumerable<IObservable<object>> AsyncReadHelper(
        //    IObserver<byte[]> result,
        //    Stream stream,
        //    int bufferSize)
        //{
        //    var asyncRead = Observable.FromAsyncPattern<byte[], int, int, int>(
        //        stream.BeginRead, stream.EndRead);
        //    var buffer = new byte[bufferSize];

        //    while (true)
        //    {
        //        //The underlying filestream will move fwd, so the 0 indicates continue from where it already is.
        //        //var read = Observable.Start(() => asyncRead(buffer, 0, bufferSize));
        //        var read = asyncRead(buffer, 0, bufferSize)
        //        yield return read;
        //        // As asyncRead returns an AsyncSubject, 
        //        // and the Iterator operator takes care of exceptions, 
        //        // we know at this point there will always be exactly one value in the list.

        //        var bytesRead = read[0];
        //        if (bytesRead == 0)
        //        {
        //            // End of file       
        //            yield break;
        //        }
        //        var outBuffer = new byte[bytesRead];
        //        Array.Copy(buffer, outBuffer, bytesRead);

        //        // Fire out to the outer Observable 
        //        result.OnNext(outBuffer);
        //    }
        //}

        #endregion
    }

    public static class EnumerableExtensions
    {
        //Naive implementation
        public static IObservable<T> ToObservable<T>(this IEnumerable<T> source)
        {
            return Observable.Create<T>(o =>
            {
                foreach (var item in source)
                {
                    o.OnNext(item);
                }
                //Dont allow for correct disposal :(
                return Disposable.Empty;
            });
        }
    }
}
// ReSharper restore InconsistentNaming