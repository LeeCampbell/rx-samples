using System;
using System.Reactive;
using System.Reactive.Linq;

namespace RxSamples.ConsoleApp
{
    //public static class ObservableExtensions
    //{
    //    //Is missing now the System.Interactive has been dropped
    //    public static IObservable<bool> IsEmpty<T>(this IObservable<T> source)
    //    {
    //        return source.Materialize()
    //            .Take(1)
    //            .Select(n => n.Kind == NotificationKind.OnCompleted
    //                          || n.Kind == NotificationKind.OnError);
    //    }
    //}
}