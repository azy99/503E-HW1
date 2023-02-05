using ProfileService.Web.Controllers;
using ProfileService.Web.Dtos;

namespace ProfileService.Web.Storage;

public class InMemoryProfileStore : IProfileStore
{
    private readonly Dictionary<string, Profile> _profiles = new();
        
    public Task UpsertProfile(Profile profile)
    {
        _profiles[profile.Username] = profile;
        return Task.CompletedTask;
    }

    public Task<Profile?> GetProfile(string username)
    {
        if (!_profiles.ContainsKey(username)) return Task.FromResult<Profile?>(null);
        return Task.FromResult<Profile?>(_profiles[username]);
    }
}