using ProfileService.Web.Dtos;
using ProfileService.Web.Storage;

namespace ProfileService.Web.Tests.Storage;

public class InMemoryProfileStoreTests
{
    private readonly InMemoryProfileStore _store = new();
    
    [Fact]
    public async Task AddNewProfile()
    {
        var profile = new Profile(username: "foobar", firstName: "Foo", lastName: "Bar");
        await _store.UpsertProfile(profile);
        Assert.Equal(profile, await _store.GetProfile(profile.Username));
    }
    
    [Fact]
    public async Task UpdateExistingProfile()
    {
        var profile = new Profile(username: "foobar", firstName: "Foo", lastName: "Bar");
        await _store.UpsertProfile(profile);

        var updatedProfile = profile with { FirstName = "Foo1", LastName = "Foo2" };
        await _store.UpsertProfile(updatedProfile);
        
        Assert.Equal(updatedProfile, await _store.GetProfile(profile.Username));
    }
    
    // TODO:
    // Add additional tests for invalid arguments (username, firstname and lastname are all mandatory fields)
    // Fix the implementation of the InMemoryProfileStore if necessary to validate the arguments being passed
}