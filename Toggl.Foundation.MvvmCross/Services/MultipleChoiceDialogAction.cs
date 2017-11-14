namespace Toggl.Foundation.MvvmCross.Services
{
    public struct MultipleChoiceDialogAction
    {
        public string Text { get; }

        public bool Destructive { get; }

        public MultipleChoiceDialogAction(string text, bool destructive)
        {
            Text = text;
            Destructive = destructive;
        }
    }
}
