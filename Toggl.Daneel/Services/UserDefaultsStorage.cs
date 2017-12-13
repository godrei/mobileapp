using System;
using Foundation;
using Toggl.Foundation;
using Toggl.PrimeRadiant.Settings;

namespace Toggl.Daneel.Services
{
    internal sealed class UserDefaultsStorage : IAccessRestrictionStorage, IOnboardingStorage
    {
        private const int newUserThreshold = 60;
        private const string outdatedApiKey = "OutdatedApi";
        private const string outdatedClientKey = "OutdatedClient";
        private const string unauthorizedAccessKey = "UnauthorizedAccessForApiToken";

        private const string isNewUserKey = "IsNewUser";
        private const string lastAccessDateKey = "LastAccessDate";
        private const string completedOnboardingKey = "CompletedOnboarding";
        
        private readonly Version version;

        public UserDefaultsStorage(ITimeService timeService, Version version)
        {
            this.version = version;

            var now = timeService.CurrentDateTime;
            var lastUsed = getString(lastAccessDateKey);
            setString(lastAccessDateKey, now.ToString());

            if (lastUsed == null) return;

            var lastUsedDate = DateTimeOffset.Parse(lastUsed);
            var offset = now - lastUsedDate;
            if (offset < TimeSpan.FromDays(newUserThreshold)) return;

            setBool(isNewUserKey, false);
        }

        #region IAccessRestrictionStorage

        public void SetClientOutdated()
        {
            setString(outdatedClientKey, version.ToString());
        }

        public void SetApiOutdated()
        {
            setString(outdatedApiKey, version.ToString());
        }

        public void SetUnauthorizedAccess(string apiToken)
        {
            setString(unauthorizedAccessKey, apiToken);
        }

        public bool IsClientOutdated()
            => isOutdated(outdatedClientKey);

        public bool IsApiOutdated()
            => isOutdated(outdatedApiKey);

        public bool IsUnauthorized(string apiToken)
            => apiToken == getString(unauthorizedAccessKey);

        private bool isOutdated(string key)
        {
            var storedVersion = getStoredVersion(key);
            return storedVersion != null && version <= storedVersion;
        }

        private Version getStoredVersion(string key)
        {
            var stored = getString(key);
            return stored == null ? null : Version.Parse(stored);
        }

        #endregion

        #region IOnboardingStorage

        public void SetIsNewUser(bool isNewUser)
        {
            setBool(isNewUserKey, isNewUser);
        }

        public void SetCompletedOnboarding()
        {
            setBool(completedOnboardingKey, true);
        }

        public bool IsNewUser() => getBool(isNewUserKey);

        public bool CompletedOnboarding() => getBool(completedOnboardingKey);

        #endregion

        private bool getBool(string key)
            => NSUserDefaults.StandardUserDefaults.BoolForKey(key);

        private string getString(string key)
            => NSUserDefaults.StandardUserDefaults.StringForKey(key);

        private void setBool(string key, bool value)
        {
            NSUserDefaults.StandardUserDefaults.SetBool(value, key);
        }

        private void setString(string key, string value)
        {
            NSUserDefaults.StandardUserDefaults.SetString(value, key);
        }
    }
}
