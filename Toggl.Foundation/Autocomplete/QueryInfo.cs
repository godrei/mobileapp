using Toggl.Foundation.Autocomplete.Suggestions;

namespace Toggl.Foundation.Autocomplete
{
    public struct QueryInfo
    {
        public string Text { get; }

        public AutocompleteSuggestionType SuggestionType { get; }

        public QueryInfo(string text, AutocompleteSuggestionType suggestionType)
        {
            Text = text;
            SuggestionType = suggestionType;
        }

        public static QueryInfo ParseFieldInfo(TextFieldInfo info)
        {
            if (string.IsNullOrEmpty(info.Text))
                return new QueryInfo("", AutocompleteSuggestionType.TimeEntries);

            return trySearchByQuerySymbols(info, out var query)
                ? query
                : new QueryInfo(info.Text, AutocompleteSuggestionType.TimeEntries);
        }

        private static bool trySearchByQuerySymbols(TextFieldInfo info, out QueryInfo query)
        {
            var stringToSearch = info.Text.Substring(0, info.DescriptionCursorPosition);

            int indexOfQuerySymbol = stringToSearch.LastIndexOfAny(getQuerySymbols(info));
            if (indexOfQuerySymbol >= 0)
            {
                var startingIndex = indexOfQuerySymbol + 1;
                var stringLength = info.Text.Length - indexOfQuerySymbol - 1;
                var type = getSuggestionType(stringToSearch[indexOfQuerySymbol]);
                var text = info.Text.Substring(startingIndex, stringLength);

                query = new QueryInfo(text, type);
                return true;
            }

            query = default(QueryInfo);
            return false;
        }

        private static char[] getQuerySymbols(TextFieldInfo info)
            => info.ProjectId.HasValue
                ? QuerySymbols.ProjectSelected
                : QuerySymbols.All;

        private static AutocompleteSuggestionType getSuggestionType(char symbol)
            => symbol == QuerySymbols.Projects
                ? AutocompleteSuggestionType.Projects
                : AutocompleteSuggestionType.Tags;
    }
}
