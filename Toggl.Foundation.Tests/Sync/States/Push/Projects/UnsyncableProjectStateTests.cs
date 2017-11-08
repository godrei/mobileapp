using System;
using Toggl.Foundation.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Tests.Sync.States.Push
{
    public sealed class UnsyncableProjectStateTests : BaseUnsyncableEntityStateTests
    {
        public UnsyncableProjectStateTests()
            : base(new TheStartMethod())
        {
        }
            
        private sealed class TheStartMethod : TheStartMethod<IDatabaseProject>
        {
            protected override BaseUnsyncableEntityState<IDatabaseProject> CreateState(IRepository<IDatabaseProject> repository)
                => new UnsyncableProjectState(repository);

            protected override IDatabaseProject CreateDirtyEntity()
                => Project.Dirty(new Ultrawave.Models.Project { Name = Guid.NewGuid().ToString() });
        }
    }
}
