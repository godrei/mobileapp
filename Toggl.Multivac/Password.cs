namespace Toggl.Multivac
{
    public struct Password
    {
        private const int minimumLength = 6;

        private readonly string password;

        public bool IsValid => password?.Length >= minimumLength;

        private Password(string password)
        {
            this.password = password;
        }

        public override string ToString() => password;

        public static Password FromString(string password)
            => new Password(password);

        public static Password Empty { get; } = new Password("");
    }
}
