using Foundation;
using Toggl.PrimeRadiant.Settings;

namespace Toggl.Daneel.Services
{
    internal sealed class UserDefaultsStorage : ISettingsStorage
    {
        public bool GetBool(string key)
            => NSUserDefaults.StandardUserDefaults.BoolForKey(key);

        public string GetString(string key)
            => NSUserDefaults.StandardUserDefaults.StringForKey(key);

        public void SetBool(string key, bool value)
        {
            NSUserDefaults.StandardUserDefaults.SetBool(value, key);
        }

        public void SetString(string key, string value)
        {
            NSUserDefaults.StandardUserDefaults.SetString(value, key);
        }
    }
}
