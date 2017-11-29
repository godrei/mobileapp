using System;
using System.Reactive;
using Toggl.Multivac.Models;

namespace Toggl.PrimeRadiant
{
    public interface ISingleObjectStorage<TModel> : IRepository<TModel>
        where TModel : IBaseModel, IDatabaseSyncable
    {
        IObservable<TModel> Single();
        IObservable<Unit> Delete();
        IObservable<TModel> Update(TModel entity);
    }
}
