using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Services
{
    public sealed class PasswordManagerResult
    {
        public Email Email { get; }
        public Password Password { get; }

        public static PasswordManagerResult None { get; } = new PasswordManagerResult("", "");

        public PasswordManagerResult(string email, string password)
            : this(Email.FromString(email), Password.FromString(password))
        {
        }

        public PasswordManagerResult(Email email, Password password)
        {
            Email = email;
            Password = password;
        }
    }
}
