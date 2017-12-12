namespace Toggl.PrimeRadiant.Settings
{
    public interface ISettingsStorage
    {
        bool GetBool(string key);

        string GetString(string key);

        void SetBool(string key, bool value);

        void SetString(string key, string value);
    }
}
