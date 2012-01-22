using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxSamples.ConsoleApp
{
    class SideEffectsExamples
    {
        public void Simple_count_side_effect()
        {
            var letters = Observable.Range(0, 3)
                .Select(i => (char)(i + 65));

            var index = -1;
            var result = letters.Select(
                c =>
                {
                    index++;
                    return c;
                });

            result.Subscribe(
                c => Console.WriteLine("Received {0} at index {1}", c, index),
                () => Console.WriteLine("completed"));
        }

        public void Simple_count_side_effect_with_2nd_subscription()
        {
            var letters = Observable.Range(0, 3)
                .Select(i => (char)(i + 65));

            var index = -1;
            var result = letters.Select(
                c =>
                {
                    index++;
                    return c;
                });

            result.Subscribe(
                c => Console.WriteLine("Received {0} at index {1}", c, index),
                () => Console.WriteLine("completed"));

            result.Subscribe(
                c => Console.WriteLine("Also received {0} at index {1}", c, index),
                () => Console.WriteLine("2nd completed"));
        }

        public void Simple_count_side_effect_free()
        {
            var source = Observable.Range(0, 3);
            var result = source.Select(
                (idx, value) => new
                                    {
                                        Index = idx,
                                        Letter = (char)(value + 65)
                                    });

            result.Subscribe(
                x => Console.WriteLine("Received {0} at index {1}", x.Letter, x.Index),
                () => Console.WriteLine("completed"));

            result.Subscribe(
                x => Console.WriteLine("Also received {0} at index {1}", x.Letter, x.Index),
                () => Console.WriteLine("2nd completed"));
        }




        public void Do_For_logging_SideEffects()
        {
            var source = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Take(3);
            var result = source.Do(
                i => Log(i),
                ex => Log(ex),
                () => Log());

            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("completed"));
        }

        public void Do_for_logging_in_a_pipeline()
        {
            var source = GetNumbers();
            var result = source.Where(i => i % 3 == 0)
                .Take(3)
                .Select(i => (char)(i + 65));

            result.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("completed"));
        }

        private static IObservable<long> GetNumbers()
        {
            return Observable.Interval(TimeSpan.FromMilliseconds(250))
                .Do(i => Console.WriteLine("pushing {0} from GetNumbers", i));
        }

        public void AsObservable_missing_allowing_sabotage()
        {
            var repo = new ObscuredLeakinessLetterRepo();
            var good = repo.GetLetters();
            var evil = repo.GetLetters();
            
            good.Subscribe(
                Console.WriteLine);

            //Be naughty
            var asSubject = evil as ISubject<string>;
            if (asSubject != null)
            {
                //So naughty. 1 is not a letter!
                asSubject.OnNext("1");
            }
            else
            {
                Console.WriteLine("could not sabotage");
            }
        }

        public void AsObservable_protects_encapsulation()
        {
            var repo = new EncapsulatedLetterRepo();
            var good = repo.GetLetters();
            var evil = repo.GetLetters();

            good.Subscribe(
                Console.WriteLine);

            //Try to be naughty
            var asSubject = evil as ISubject<string>;
            if (asSubject != null)
            {
                //Won't get to here.
                asSubject.OnNext("1");
            }
            else
            {
                Console.WriteLine("could not sabotage");
            }
        }

        public void Mutable_elements_cant_be_protected()
        {
            var source = new Subject<Account>();

            //Evil code
            source.Subscribe(account => account.Name = "Garbage");
            //unassuming well behaved code
            source.Subscribe(
                account=>Console.WriteLine("{0} {1}", account.Id, account.Name),
                () => Console.WriteLine("completed"));

            source.OnNext(new Account {Id = 1, Name = "Microsoft"});
            source.OnNext(new Account {Id = 2, Name = "Google"});
            source.OnNext(new Account {Id = 3, Name = "IBM"});
            source.OnCompleted();
        }

        private static void Log(object onNextValue)
        {
            Console.WriteLine("Logging OnNext({0}) @ {1}", onNextValue, DateTime.Now);
        }
        private static void Log(Exception onErrorValue)
        {
            Console.WriteLine("Logging OnError({0}) @ {1}", onErrorValue, DateTime.Now);
        }
        private static void Log()
        {
            Console.WriteLine("Logging OnCompleted()@ {0}", DateTime.Now);
        }
    }

    public class UltraLeakyLetterRepo
    {
        public ReplaySubject<string> Letters {get; set; }

        public UltraLeakyLetterRepo()
        {
            Letters = new ReplaySubject<string>();
            Letters.OnNext("A");
            Letters.OnNext("B");
            Letters.OnNext("C");
        }
    }

    public class LeakyLetterRepo
    {
        private readonly ReplaySubject<string> _letters;

        public LeakyLetterRepo()
        {
            _letters = new ReplaySubject<string>();
            _letters.OnNext("A");
            _letters.OnNext("B");
            _letters.OnNext("C");
        }
        public ReplaySubject<string> GetLetters()
        {
            return _letters;
        }
    }

    public class ObscuredLeakinessLetterRepo
    {
        private readonly ReplaySubject<string> _letters;

        public ObscuredLeakinessLetterRepo()
        {
            _letters = new ReplaySubject<string>();
            _letters.OnNext("A");
            _letters.OnNext("B");
            _letters.OnNext("C");
        }
        public IObservable<string> GetLetters()
        {
            return _letters;
        }
    }
    public class EncapsulatedLetterRepo
    {
        private readonly ReplaySubject<string> _letters;

        public EncapsulatedLetterRepo()
        {
            _letters = new ReplaySubject<string>();
            _letters.OnNext("A");
            _letters.OnNext("B");
            _letters.OnNext("C");
        }
        public IObservable<string> GetLetters()
        {
            return _letters.AsObservable();
        }
    }

    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
