using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace RxSamples.WpfApplication.Examples.MemberSearch
{
    public interface ITypeService
    {
        IObservable<string> ListMethods(string search);
    }

    public sealed class TypeService : ITypeService
    {
        public IObservable<string> ListMethods(string search)
        {
            return Observable.Create<string>(
                observer =>
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));

                        var methods = from method in typeof(Observable).GetMethods()
                                      select method.Name;
                        return methods
                            .Distinct()
                            .OrderBy(method=>method)
                            .Where(method=>method.StartsWith(search, StringComparison.InvariantCultureIgnoreCase))
                            .ToObservable()
                            .Subscribe(observer);
                    });
        }
    }
}