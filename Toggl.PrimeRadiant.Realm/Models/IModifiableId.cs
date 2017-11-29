namespace Toggl.PrimeRadiant.Realm.Models
{
    interface IModifiableId
    {
        long Id { get; set; }

        long? OriginalId { get; set; }
    }
}
