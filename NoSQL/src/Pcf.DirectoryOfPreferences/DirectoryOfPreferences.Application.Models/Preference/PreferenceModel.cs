namespace DirectoryOfPreferences.Application.Models.Preference
{
    public class PreferenceModel
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
    }
}
