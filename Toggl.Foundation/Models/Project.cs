using System;
using Toggl.PrimeRadiant;

namespace Toggl.Foundation.Models
{
    internal partial class Project
    {
        internal sealed class Builder
        {
            private const string errorMessage = "You need to set the {0} before building a project";

            public static Builder Create(long id) => new Builder(id);

            public long Id { get; }

            public string Name { get; private set; }

            public string Color { get; private set; }

            public bool? Billable { get; private set; }

            public SyncStatus SyncStatus { get; private set; }

            public long? WorkspaceId { get; private set; }

            public DateTimeOffset? At { get; private set; }

            public DateTimeOffset? ServerDeletedAt { get; private set; }

            public bool IsDeleted { get; private set; }

            public long? ClientId { get; private set; }

            private Builder(long id)
            {
                Id = id;
            }

            public Project Build()
            {
                ensureValidity();
                return new Project(this);
            }

            public Builder SetSyncStatus(SyncStatus syncStatus)
            {
                SyncStatus = syncStatus;
                return this;
            }

            public Builder SetWorkspaceId(long workspaceId)
            {
                WorkspaceId = workspaceId;
                return this;
            }

            internal Builder SetClientId(long? clientId)
            {
                ClientId = clientId;
                return this;
            }

            internal Builder SetBillable(bool? billable)
            {
                Billable = billable;
                return this;
            }

            internal Builder SetColor(string color)
            {
                Color = color;
                return this;
            }

            public Builder SetName(string name)
            {
                Name = name;
                return this;
            }

            public Builder SetAt(DateTimeOffset at)
            {
                At = at;
                return this;
            }

            public Builder SetServerDeletedAt(DateTimeOffset? serverDeleteAt)
            {
                ServerDeletedAt = serverDeleteAt;
                return this;
            }

            public Builder SetIsDeleted(bool isDeleted)
            {
                IsDeleted = isDeleted;
                return this;
            }

            private void ensureValidity()
            {
                if (string.IsNullOrEmpty(Name))
                    throw new InvalidOperationException(string.Format(errorMessage, "name"));

                if (string.IsNullOrEmpty(Color))
                    throw new InvalidOperationException(string.Format(errorMessage, "color"));

                if (WorkspaceId == null || WorkspaceId == 0)
                    throw new InvalidOperationException(string.Format(errorMessage, "workspace id"));

                if (At == null)
                    throw new InvalidOperationException(string.Format(errorMessage, "at"));
            }
        }

        private Project(Builder builder)
        {
            Id = builder.Id;
            Name = builder.Name;
            At = builder.At.Value;
            Color = builder.Color;
            Billable = builder.Billable;
            ClientId = builder.ClientId;
            IsDeleted = builder.IsDeleted;
            SyncStatus = builder.SyncStatus;
            WorkspaceId = builder.WorkspaceId.Value;
            ServerDeletedAt = builder.ServerDeletedAt;
        }
    }
}
