using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Concurrency;

namespace RxSamples.ConsoleApp
{
  public class HotAndColdExamples
  {
    public void Subject_is_hot_so_starts_regardless_of_subscription()
    {
      var subject = new Subject<int>();
      Console.WriteLine("publishing 0");
      subject.OnNext(0);
      Console.WriteLine("publishing 1");
      subject.OnNext(1);
      Console.WriteLine("Subscribing");
      subject.Subscribe(i => Console.WriteLine(i)); //Can be refactored to subject.Subscribe(Console.WriteLine)
      Console.WriteLine("publishing 2");
      subject.OnNext(2);
      Console.WriteLine("publishing 3");
      subject.OnNext(3);
      Console.ReadKey();
      /*
       * Output 
       * publishing 0
       * publishing 1
       * Subscribing
       * publishing 2
       * 2
       * publishing 3
       * 3
       */
    }

    public void Interval_is_cold_so_starts_on_subscription_and_does_not_share_stream()
    {
      var period = TimeSpan.FromSeconds(1);
      var observable = Observable.Interval(period);
      observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
      Thread.Sleep(period);
      observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));
      /* Ouput:
       first subscription : 0
       first subscription : 1
       second subscription : 0
       first subscription : 2
       second subscription : 1
       first subscription : 3
       second subscription : 2   
       */
    }

    public void Publish_shares_stream_and_Connect_makes_cold_observables_hot()
    {
      var period = TimeSpan.FromSeconds(1);
      var observable = Observable.Interval(period).Publish();
      observable.Connect();
      observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
      Thread.Sleep(period);
      Thread.Sleep(period);
      observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));

      /* Ouput:
       first subscription : 0
       first subscription : 1
       second subscription : 1
       first subscription : 2
       second subscription : 2   
       */
    }

    public void Connections_can_be_disposed_and_reconnected()
    {
      var period = TimeSpan.FromSeconds(1);
      var observable = Observable.Interval(period).Publish();
      observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));
      var exit = false;
      while (!exit)
      {
        Console.WriteLine("Press enter to connect, esc to exit.");
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Enter)
        {
          var connection = observable.Connect();
          Console.WriteLine("Press any key to dispose of connection.");
          Console.ReadKey();
          connection.Dispose();
        }
        if (key.Key == ConsoleKey.Escape)
        {
          exit = true;
        }
      }
      /* Ouput:
       Press enter to connect, esc to exit.
       Press any key to dispose of connection.
       subscription : 0
       subscription : 1
       subscription : 2
       Press enter to connect, esc to exit.
       Press any key to dispose of connection.
       subscription : 0
       subscription : 1
       subscription : 2
       Press enter to connect, esc to exit.   
       */
    }

    public void Connected_publishes_regardless_of_subscribers()
    {
      var period = TimeSpan.FromSeconds(1);
      var observable = Observable.Interval(period)
        .Do(l => Console.WriteLine("Publishing {0}", l)) //produce Side effect to show it is running.
        .Publish();
      observable.Connect();
      Console.WriteLine("Press any key to subscribe");
      Console.ReadKey();
      var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));

      Console.WriteLine("Press any key to unsubscribe.");
      Console.ReadKey();
      subscription.Dispose();

      Console.WriteLine("Press any key to exit.");
      /* Ouput:
       Press any key to subscribe
       Publishing 0
       Publishing 1
       Press any key to unsubscribe.
       Publishing 2
       subscription : 2
       Publishing 3
       subscription : 3
       Press any key to exit.
       Publishing 4
       Publishing 5
       */
    }

    public void RefCount_only_publishes_once_subscribed_to()
    {
      var period = TimeSpan.FromSeconds(1);
      var observable = Observable.Interval(period)
        .Do(l => Console.WriteLine("Publishing {0}", l)) //produce Side effect to show it is running.
        .Publish()
        .RefCount();
      //observable.Connect(); Use RefCount instead now
      Console.WriteLine("Press any key to subscribe");
      Console.ReadKey();
      var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));

      Console.WriteLine("Press any key to unsubscribe.");
      Console.ReadKey();
      subscription.Dispose();

      Console.WriteLine("Press any key to exit.");
      /* Ouput:
       Press any key to subscribe
       Press any key to unsubscribe.
       Publishing 0
       subscription : 0
       Publishing 1
       subscription : 1
       Publishing 2
       subscription : 2
       Press any key to exit.
       */
    }

    public void RefCount_is_a_shared_stream_and_unsubscribes_to_underlying_when_no_more_subscribers()
    {
      var period = TimeSpan.FromSeconds(1);
      var observable = Observable.Interval(period)
        .Do(l => Console.WriteLine("Publishing {0}", l)) //produce Side effect to show it is running.
        .Publish()
        .RefCount();
      //observable.Connect(); Use RefCount instead now
      Console.WriteLine("Press any key to subscribe");
      Console.ReadKey();
      var subscription = observable.Subscribe(i => Console.WriteLine("subscription1 : {0}", i));

      Console.WriteLine("Press any key to subscribe");
      Console.ReadKey();
      var subscription2 = observable.Subscribe(i => Console.WriteLine("subscription2 : {0}", i));

      Console.WriteLine("Press any key to unsubscribe.");
      Console.ReadKey();
      subscription2.Dispose();

      Console.WriteLine("Press any key to unsubscribe.");
      Console.ReadKey();
      subscription.Dispose();
      /* Ouput:
       Press any key to subscribe
       Press any key to unsubscribe.
       Publishing 0
       subscription : 0
       Publishing 1
       subscription : 1
       Publishing 2
       subscription : 2
       Press any key to exit.
       */
    }

    public void Prune_will_subscribe_and_return_the_last_value_like_AsyncSubject()
    {
      var period = TimeSpan.FromSeconds(1);
      var observable = Observable.Interval(period)
        .Take(5)
        .Do(l => Console.WriteLine("Publishing {0}", l)) //produce Side effect to show it is running.
        .Prune();
      observable.Connect();
      Console.WriteLine("Press any key to subscribe");
      Console.ReadKey();
      var subscription = observable.Subscribe(i => Console.WriteLine("subscription : {0}", i));

      Console.WriteLine("Press any key to unsubscribe.");
      Console.ReadKey();
      subscription.Dispose();
      /* Ouput:
       Press any key to subscribe
       Publishing 0
       Publishing 1
       Press any key to unsubscribe.
       Publishing 2
       Publishing 3
       Publishing 4
       subscription : 4
       Press any key to exit.
       */
    }
    public void Replay_wraps_underlying_in_ReplaySubject()
    {
      var period = TimeSpan.FromSeconds(1);
      var observable = Observable.Interval(period)
        .Take(3)
        .Replay();
      observable.Connect();
      observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
      Thread.Sleep(period);
      Thread.Sleep(period);
      observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));

      Console.ReadKey();
      observable.Subscribe(i => Console.WriteLine("third subscription : {0}", i));
      /* Ouput:
       first subscription : 0
       second subscription : 0
       first subscription : 1
       second subscription : 1
       first subscription : 2
       second subscription : 2   
       third subscription : 0
       third subscription : 1
       third subscription : 2
       */
    }

    public void ReplayOnHotExample()
    {
      var period = TimeSpan.FromSeconds(1);
      var hot = Observable.Interval(period)
        .Take(3)
        .Publish();
      hot.Connect();
      Thread.Sleep(period); //Run hot and ensure a value is lost.
      var observable = hot.Replay();
      observable.Connect();
      observable.Subscribe(i => Console.WriteLine("first subscription : {0}", i));
      Thread.Sleep(period);
      observable.Subscribe(i => Console.WriteLine("second subscription : {0}", i));

      Console.ReadKey();
      observable.Subscribe(i => Console.WriteLine("third subscription : {0}", i));
      Console.ReadKey();

      /* Ouput:
       first subscription : 1
       second subscription : 1
       first subscription : 2
       second subscription : 2   
       third subscription : 1
       third subscription : 2
       */
    }

    //TODO: What is this proving?
    public void DeferExample()
    {
      var expensiveSubcription = Observable.CreateWithDisposable<long>
        (o =>
           {
             Console.WriteLine("Subscribing now....");
             var stream = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
             var subscrition = stream.Subscribe(o);
             return subscrition;
           });

      var query = from item in expensiveSubcription
                  from newItem in Observable.Interval(TimeSpan.FromMilliseconds(500))
                  select new { item, newItem };

      var deffered = Observable.Defer(() => query);
      //Console.WriteLine("waiting...");
      //Thread.Sleep(1000);
      // query.Subscribe(Console.WriteLine);
      deffered.Subscribe(Console.WriteLine);
    }

    public void DeferExample2()
    {
      Console.WriteLine("Requesting the stream...");
      var deffered = Observable.Defer<int>(BlockingMethodCall);
      Console.WriteLine("stream retrieved.");
      deffered
        .SubscribeOn(Scheduler.ThreadPool)
        .ObserveOn(Scheduler.CurrentThread)
        .Subscribe(Console.WriteLine);
    }

    public void WithoutDeferExample()
    {
      Console.WriteLine("Requesting the stream...");
      var deffered = BlockingMethodCall();
      Console.WriteLine("stream retrieved.");
      deffered
        .SubscribeOn(Scheduler.ThreadPool)
        .ObserveOn(Scheduler.CurrentThread)
        .Subscribe(Console.WriteLine);

    }

    public IObservable<int> BlockingMethodCall()
    {
      Console.WriteLine("Subscribing now....");
      Thread.Sleep(1000);//Simulate connecting to slow service, IO etc...
      return Observable.Range(0, 10);
    }

    /* These examples are probably overly complex from an introduction to Rx */

    //public void SimpleHotExample()
    //{
    //  var stream = Observable.Interval(TimeSpan.FromSeconds(1))
    //    .Publish()
    //    .RefCount();

    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to subscribe to temperature stream");
    //  }
    //  //The TemperatureService temperature stream is now running hot.
    //  Console.ReadKey(true);

    //  //Will start the hot observable.
    //  var token = stream.Subscribe(t => Console.WriteLine("New temp published : {0}", t));
    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to create a second subscription...");
    //  }
    //  Console.ReadKey(true);

    //  var token2 = stream.Subscribe(t => Console.WriteLine("New temp published on 2 : {0}", t));
    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to unsubscribe from all streams.");
    //  }
    //  Console.ReadKey(true);

    //  token.Dispose();
    //  token2.Dispose();

    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to subscribe to a new stream.");
    //  }
    //  Console.ReadKey(true);

    //  var token3 = stream.Subscribe(t => Console.WriteLine("New temp published on 3 : {0}", t));
    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to create another subscription...");
    //  }
    //  Console.ReadKey(true);

    //  var token4 = stream.Subscribe(t => Console.WriteLine("New temp published on 4 : {0}", t));
    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to unsubscribe from all streams.");
    //  }
    //  Console.ReadKey(true);
    //  token3.Dispose();
    //  token4.Dispose();

    //  //The TemperatureService temperature stream is still running hot.
    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to exit...");
    //    Console.ReadKey(true);
    //  }
    //}

    //public void HotSample()
    //{
    //  var svc = new TemperatureService();
    //  svc.Start();//Will start the hot observable.

    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to subscribe to temperature stream");
    //  }
    //  //The TemperatureService temperature stream is now running hot.
    //  Console.ReadKey(true);

    //  var temperatures = svc.GetTemperatureStream();
    //  var token = temperatures.Subscribe(t => Console.WriteLine("New temp published : {0}", t));
    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to unsubscribe from temperature stream");
    //  }
    //  Console.ReadKey(true);

    //  token.Dispose();
    //  //The TemperatureService temperature stream is still running hot.
    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to exit...");
    //    Console.ReadKey(true);
    //  }
    //}

    //public void RefCountExample()
    //{
    //  var svc = new RefCountTemperatureService();

    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to subscribe to temperature stream");
    //  }
    //  //The TemperatureService temperature stream is now running hot.
    //  Console.ReadKey(true);

    //  //Will start the hot observable.
    //  var temperatures = svc.GetTemperatureStream();
    //  var token = temperatures.Subscribe(t => Console.WriteLine("New temp published : {0}", t));
    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to create a second subscription...");
    //  }
    //  Console.ReadKey(true);

    //  var temperatures2 = svc.GetTemperatureStream();
    //  var token2 = temperatures2.Subscribe(t => Console.WriteLine("New temp published on 2 : {0}", t));
    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to unsubscribe from all streams.");
    //  }
    //  Console.ReadKey(true);

    //  token.Dispose();
    //  token2.Dispose();

    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to subscribe to a new stream.");
    //  }
    //  Console.ReadKey(true);

    //  var temperatures3 = svc.GetTemperatureStream();
    //  var token3 = temperatures3.Subscribe(t => Console.WriteLine("New temp published on 3 : {0}", t));
    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to create a second subscription...");
    //  }
    //  Console.ReadKey(true);
    //  var temperatures4 = svc.GetTemperatureStream();
    //  var token4 = temperatures4.Subscribe(t => Console.WriteLine("New temp published on 4 : {0}", t));
    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to unsubscribe from all streams.");
    //  }
    //  Console.ReadKey(true);
    //  token3.Dispose();
    //  token4.Dispose();

    //  //The TemperatureService temperature stream is still running hot.
    //  using (new ConsoleColorScope(ConsoleColor.White))
    //  {
    //    Console.WriteLine("Press any key to exit...");
    //    Console.ReadKey(true);
    //  }
    //}

    //private static IObservable<string> GetProducts()
    //{
    //  return Observable.CreateWithDisposable<string>(
    //    o =>
    //    {
    //      using (var conn = new SqlConnection(@"Data Source=.\SQLSERVER;Initial Catalog=AdventureWorksLT2008;Integrated Security=SSPI;"))
    //      using (var cmd = new SqlCommand("Select Name FROM SalesLT.ProductModel", conn))
    //      {
    //        conn.Open();
    //        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
    //        while (reader.Read())
    //        {
    //          o.OnNext(reader.GetString(0));
    //        }
    //        o.OnCompleted();
    //        return Disposable.Create(() => Console.WriteLine("--Disposed--"));
    //      }
    //    });
    //}

    //private class TemperatureService
    //{
    //  public TemperatureService()
    //  {
    //    TempStream = Observable.Interval(TimeSpan.FromSeconds(2))
    //      //.Select(i => GetTemperature())
    //      .Select(i => Convert.ToDecimal(i))
    //      .Publish();
    //  }

    //  protected IConnectableObservable<decimal> TempStream { get; set; }

    //  public virtual IObservable<decimal> GetTemperatureStream()
    //  {
    //    return TempStream;
    //  }

    //  public virtual void Start()
    //  {
    //    TempStream.Connect();
    //  }

    //  private static decimal GetTemperature()
    //  {
    //    var uriString = @"http://weather.yahooapis.com/forecastrss?w=2442047&u=c";
    //    string xmlResponse = GetXmlResponse(uriString);
    //    var xml = XDocument.Parse(xmlResponse);
    //    decimal temp = GetTemperatureFromXml(xml);
    //    using (new ConsoleColorScope(ConsoleColor.DarkGray))
    //    {
    //      Console.WriteLine("GetTemperature returning {0}", temp);
    //    }
    //    return temp;
    //  }

    //  private static decimal GetTemperatureFromXml(XDocument xml)
    //  {
    //    XNamespace xmlns = "http://xml.weather.yahoo.com/ns/rss/1.0";
    //    var query = from weather in xml.Descendants(xmlns + "condition")
    //                select weather.Attribute("temp");
    //    var tempString = query.FirstOrDefault();
    //    decimal temp;
    //    decimal.TryParse(tempString.Value, out temp);
    //    return temp;
    //  }

    //  private static string GetXmlResponse(string uriString)
    //  {
    //    string xmlResponse;
    //    try
    //    {
    //      var webRequest = (HttpWebRequest)WebRequest.Create(uriString);
    //      webRequest.Timeout = 10000;     // 10 secs
    //      var webResponse = (HttpWebResponse)webRequest.GetResponse();
    //      Encoding encoding = Encoding.GetEncoding(1252);  // Windows default Code Page
    //      using (var responseStream = new StreamReader(webResponse.GetResponseStream(), encoding))
    //      {
    //        xmlResponse = responseStream.ReadToEnd();
    //      }
    //      webResponse.Close();
    //    }
    //    catch (Exception)
    //    {
    //      xmlResponse = LoadHardCodedResponse();
    //    }
    //    return xmlResponse;
    //  }

    //  private static string LoadHardCodedResponse()
    //  {
    //    return File.ReadAllText("WeatherResponse.xml");
    //  }
    //}

    //private class RefCountTemperatureService : TemperatureService
    //{
    //  private readonly IObservable<decimal> _refCountedStream;
    //  private bool _isConnected;
    //  public RefCountTemperatureService()
    //    : base()
    //  {
    //    _refCountedStream = TempStream.RefCount();
    //  }

    //  public override IObservable<decimal> GetTemperatureStream()
    //  {
    //    if (_isConnected) Start();
    //    return _refCountedStream;
    //  }

    //  public override void Start()
    //  {
    //    base.Start();
    //    _isConnected = true;
    //  }
    //}
  }
}