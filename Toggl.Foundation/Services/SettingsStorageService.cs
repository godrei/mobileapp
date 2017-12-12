using System;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Settings;

namespace Toggl.Foundation.Services
{
    public sealed class SettingsStorageService : IAccessRestrictionStorage, IOnboardingStorage
    {
        private const int newUserThreshold = 60;
        private const string outdatedApiKey = "OutdatedApi";
        private const string outdatedClientKey = "OutdatedClient";
        private const string unauthorizedAccessKey = "UnauthorizedAccessForApiToken";

        private const string isNewUserKey = "IsNewUser";
        private const string lastAccessDateKey = "LastAccessDate";
        private const string completedOnboardingKey = "CompletedOnboarding";

        private readonly ISettingsStorage settingsStorage;
        private readonly Version version;

        public SettingsStorageService(ITimeService timeService, ISettingsStorage settingsStorage, Version version)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(settingsStorage, nameof(settingsStorage));

            this.version = version;
            this.settingsStorage = settingsStorage;

            var now = timeService.CurrentDateTime;
            var lastUsed = settingsStorage.GetString(lastAccessDateKey);
            settingsStorage.SetString(lastAccessDateKey, now.ToString());

            if (lastUsed == null) return;

            var lastUsedDate = DateTimeOffset.Parse(lastUsed);
            var offset = now - lastUsedDate;
            if (offset < TimeSpan.FromDays(newUserThreshold)) return;

            settingsStorage.SetBool(isNewUserKey, false);
        }

        #region IAccessRestrictionStorage

        public void SetClientOutdated()
        {
            settingsStorage.SetString(outdatedClientKey, version.ToString());
        }

        public void SetApiOutdated()
        {
            settingsStorage.SetString(outdatedApiKey, version.ToString());
        }

        public void SetUnauthorizedAccess(string apiToken)
        {
            settingsStorage.SetString(unauthorizedAccessKey, apiToken);
        }

        public bool IsClientOutdated()
            => isOutdated(outdatedClientKey);

        public bool IsApiOutdated()
            => isOutdated(outdatedApiKey);

        public bool IsUnauthorized(string apiToken)
            => apiToken == settingsStorage.GetString(unauthorizedAccessKey);

        private bool isOutdated(string key)
        {
            var storedVersion = getStoredVersion(key);
            return storedVersion != null && version <= storedVersion;
        }

        private Version getStoredVersion(string key)
        {
            var stored = settingsStorage.GetString(key);
            return stored == null ? null : Version.Parse(stored);
        }

        #endregion

        #region IOnboardingStorage

        public void SetIsNewUser(bool isNewUser)
        {
            settingsStorage.SetBool(isNewUserKey, isNewUser);
        }

        public void SetCompletedOnboarding()
        {
            settingsStorage.SetBool(completedOnboardingKey, true);
        }

        public bool IsNewUser() => settingsStorage.GetBool(isNewUserKey);

        public bool CompletedOnboarding() => settingsStorage.GetBool(completedOnboardingKey);

        #endregion
    }
}
