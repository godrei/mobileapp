using System;
using Toggl.Foundation.Autocomplete.Suggestions;

namespace Toggl.Foundation.Autocomplete
{
    public struct QueryInfo
    {
        public string Text { get; }

        public AutocompleteSuggestionType SuggestionType { get; }

        private const int minimumQueryLength = 2;

        private static QueryInfo emptyQueryInfo
            => new QueryInfo(String.Empty, AutocompleteSuggestionType.None);

        public QueryInfo(string text, AutocompleteSuggestionType suggestionType)
        {
            Text = text;
            SuggestionType = suggestionType;
        }

        public static QueryInfo ParseFieldInfo(TextFieldInfo info)
        {
            if (string.IsNullOrEmpty(info.Text))
                return emptyQueryInfo;

            return searchByQuerySymbols(info)
                ?? getDefaultQueryInfo(info.Text);
        }

        private static QueryInfo? searchByQuerySymbols(TextFieldInfo info)
        {
            int indexOfQuerySymbol = info.DescriptionCursorPosition;
            string stringToSearch;
            do
            {
                stringToSearch = info.Text.Substring(0, indexOfQuerySymbol);
                indexOfQuerySymbol = stringToSearch.LastIndexOfAny(getQuerySymbols(info));
            } while (indexOfQuerySymbol > 0 && Char.IsWhiteSpace(stringToSearch[indexOfQuerySymbol - 1]) == false);

            if (indexOfQuerySymbol >= 0)
            {
                var startingIndex = indexOfQuerySymbol + 1;
                var stringLength = info.Text.Length - indexOfQuerySymbol - 1;
                var type = getSuggestionType(stringToSearch[indexOfQuerySymbol]);
                var text = info.Text.Substring(startingIndex, stringLength);

                return new QueryInfo(text, type);
            }

            return null;
        }

        private static QueryInfo getDefaultQueryInfo(string text)
            => text.Length < minimumQueryLength
                ? emptyQueryInfo
                : new QueryInfo(text, AutocompleteSuggestionType.TimeEntries);

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
