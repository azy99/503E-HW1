using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using ProfileService.Web.Dtos;
using ProfileService.Web.Storage;

namespace ProfileService.Web.Tests.Controllers;

public class ProfileControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProfileControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task GetProfile()
    {
        var profileStoreMock = new Mock<IProfileStore>();

        var profile = new Profile("foobar", "Foo", "Bar");
        profileStoreMock.Setup(m => m.GetProfile(profile.Username))
            .ReturnsAsync(profile);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(profileStoreMock.Object);
            });
        }).CreateClient();

        var response = await client.GetAsync($"/Profile/{profile.Username}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Equal(profile, JsonConvert.DeserializeObject<Profile>(responseBody));
    }
    
    // TODO: Add a test for NotFound response
    
    
    //TODO: Add tests for remaining APIs 
}