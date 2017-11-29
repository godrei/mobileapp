using System;
using NSubstitute;
using Toggl.Foundation.Models;
using Toggl.Foundation.Sync.States;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave;

namespace Toggl.Foundation.Tests.Sync.States
{
    public sealed class UpdateUserStateTests : BaseUpdateEntityStateTests
    {
        public UpdateUserStateTests()
            : base(new TheStartMethod())
        {
        }

        private sealed class TheStartMethod : TheStartMethod<IDatabaseUser, IUser>
        {
            protected override BasePushEntityState<IDatabaseUser> CreateState(ITogglApi api, IRepository<IDatabaseUser> repository)
                => new UpdateUserState(api, repository);

            protected override Func<IDatabaseUser, IObservable<IUser>> GetUpdateFunction(ITogglApi api)
                => api.User.Update;

            protected override IDatabaseUser CreateDirtyEntity(long id, DateTimeOffset lastUpdate = default(DateTimeOffset))
                => User.Dirty(new Ultrawave.Models.User { Id = id, Fullname = Guid.NewGuid().ToString(), At = lastUpdate });

            protected override void AssertUpdateReceived(ITogglApi api, IDatabaseUser entity)
                => api.User.Received().Update(entity);

            protected override IDatabaseUser CreateDirtyEntityWithNegativeId()
                => User.Dirty(new Ultrawave.Models.User { Id = -1, Fullname = Guid.NewGuid().ToString() });

            protected override IDatabaseUser CreateCleanWithPositiveIdFrom(IDatabaseUser entity)
            {
                var te = new Ultrawave.Models.User(entity);
                te.Id = 1;
                return User.Clean(te);
            }

            protected override IDatabaseUser CreateCleanEntityFrom(IDatabaseUser entity)
                => User.Clean(entity);
        }
    }
}
