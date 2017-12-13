using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Foundation.Autocomplete.Suggestions;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Autocomplete
{
    public sealed class AutocompleteProvider : IAutocompleteProvider
    {
        private readonly ITogglDatabase database;

        public AutocompleteProvider(ITogglDatabase database)
        {
            Ensure.Argument.IsNotNull(database, nameof(database));

            this.database = database;
        }

        public IObservable<IEnumerable<AutocompleteSuggestion>> Query(TextFieldInfo info)
            => Query(QueryInfo.ParseFieldInfo(info));

        public IObservable<IEnumerable<AutocompleteSuggestion>> Query(QueryInfo queryInfo)
        {
            var wordsToQuery = queryInfo.Text.Split(' ').Where(word => !string.IsNullOrEmpty(word)).Distinct();
            return querySuggestions(wordsToQuery, queryInfo.SuggestionType);
        }

        private IObservable<IEnumerable<AutocompleteSuggestion>> querySuggestions(
            IEnumerable<string> wordsToQuery, AutocompleteSuggestionType suggestionType)
        {
            var queryListIsEmpty = !wordsToQuery.Any();

            switch (suggestionType)
            {
                case AutocompleteSuggestionType.Projects when queryListIsEmpty:
                    return database.Projects.GetAll()
                        .Select(ProjectSuggestion.FromProjects);

                case AutocompleteSuggestionType.Projects:
                    return wordsToQuery
                        .Aggregate(database.Projects.GetAll(), (obs, word) => obs.Select(filterProjectsByWord(word)))
                        .Select(ProjectSuggestion.FromProjects);

                case AutocompleteSuggestionType.Tags:
                    return wordsToQuery
                        .Aggregate(database.Tags.GetAll(), (obs, word) => obs.Select(filterTagsByWord(word)))
                        .Select(TagSuggestion.FromTags);
            }

            if (queryListIsEmpty)
                return Observable.Return(QuerySymbolSuggestion.Suggestions);

            return wordsToQuery
               .Aggregate(database.TimeEntries.GetAll(), (obs, word) => obs.Select(filterTimeEntriesByWord(word)))
               .Select(TimeEntrySuggestion.FromTimeEntries);
        }

        private Func<IEnumerable<IDatabaseTimeEntry>, IEnumerable<IDatabaseTimeEntry>> filterTimeEntriesByWord(string word)
            => timeEntries =>
                timeEntries.Where(
                    te => te.Description.ContainsIgnoringCase(word)
                       || (te.Project != null && te.Project.Name.ContainsIgnoringCase(word))
                       || (te.Project?.Client != null && te.Project.Client.Name.ContainsIgnoringCase(word))
                       || (te.Task != null && te.Task.Name.ContainsIgnoringCase(word)));

        private Func<IEnumerable<IDatabaseProject>, IEnumerable<IDatabaseProject>> filterProjectsByWord(string word)
            => projects =>
                projects.Where(
                    p => p.Name.ContainsIgnoringCase(word)
                      || (p.Client != null && p.Client.Name.ContainsIgnoringCase(word))
                      || (p.Tasks != null && p.Tasks.Any(task => task.Name.ContainsIgnoringCase(word))));

        private Func<IEnumerable<IDatabaseTag>, IEnumerable<IDatabaseTag>> filterTagsByWord(string word)
            => tags => tags.Where(t => t.Name.ContainsIgnoringCase(word));
    }
}
