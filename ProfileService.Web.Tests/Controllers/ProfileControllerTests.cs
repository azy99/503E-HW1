using System.Net;
using System.Text;
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
    [Fact]
    public async Task GetNonExistentProfile()
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

        var response = await client.GetAsync($"/Profile/{profile.LastName}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    // TODO: Add tests for remaining APIs 
     [Fact]
     public async Task AddProfile()
     {
         var profileStoreMock = new Mock<IProfileStore>();
         var client = _factory.WithWebHostBuilder(builder =>
         {
             builder.ConfigureTestServices(services =>
             {
                 services.AddSingleton(profileStoreMock.Object);
             });
         }).CreateClient();
         
         var data = new
         {
             username = "a",
             firstName = "b",
             lastName = "c"
         };
         var profile = new Profile("a", "b","c");
         var json = JsonConvert.SerializeObject(data);
         var content =
             new StringContent(json, Encoding.UTF8, "application/json");
         var response = await client.PostAsync($"/Profile", content);
         Assert.Equal(HttpStatusCode.Created, response.StatusCode);
         var responseBody = await response.Content.ReadAsStringAsync();
         Assert.Equal(profile, JsonConvert.DeserializeObject<Profile>(responseBody));
     }
    
    [Fact]
    public async Task AddExistingProfile()
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
        
        var data = new
        {
            username = "foobar",
            firstName = "b",
            lastName = "c"
        };
        var json = JsonConvert.SerializeObject(data);
        var content =
            new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"/Profile", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    
    [Fact]
    public async Task PutProfile()
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
        
        var data = new
        {
            firstName = "b",
            lastName = "c"
        };
        var updatedProfile = new Profile("foobar","b","c");
        var json = JsonConvert.SerializeObject(data);
        var content =
            new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"/Profile/{profile.Username}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Equal(updatedProfile, JsonConvert.DeserializeObject<Profile>(responseBody));
    }
    
    [Fact]
    public async Task PutNonExistentProfile()
    {
        var profileStoreMock = new Mock<IProfileStore>();

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(profileStoreMock.Object);
            });
        }).CreateClient();
        
        var profile = new Profile("foobar","b","c");
        var data = new
        {
            firstName = "b",
            lastName = "c"
        };
        var json = JsonConvert.SerializeObject(data);
        var content =
            new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"/Profile/{profile.Username}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    
    }
    
}