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
    private readonly Mock<IProfileStore> _profileStoreMock = new();
    private readonly HttpClient _httpClient;
    

    public ProfileControllerTests(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(_profileStoreMock.Object);
            });
        }).CreateClient();
    }
    
    [Fact]
    public async Task GetProfile()
    {
        
        var profile = new Profile("foobar", "Foo", "Bar");
        _profileStoreMock.Setup(m => m.GetProfile(profile.Username))
            .ReturnsAsync(profile);
        
        var response = await _httpClient.GetAsync($"/Profile/{profile.Username}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Equal(profile, JsonConvert.DeserializeObject<Profile>(responseBody));
    }
    
    // TODO: Add a test for NotFound response
    [Fact]
    public async Task GetProfile_NotFound()
    {
        
        _profileStoreMock.Setup(m => m.GetProfile("foobar"))
            .ReturnsAsync((Profile?)null);

        var response = await _httpClient.GetAsync($"/Profile/foobar");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    // TODO: Add tests for remaining APIs 
     [Fact]
     public async Task AddProfile()
     {
         var profile = new Profile("foobar", "b","c");
         var json = JsonConvert.SerializeObject(profile);
         var content =
             new StringContent(json, Encoding.UTF8, "application/json");
         var response = await _httpClient.PostAsync($"/Profile", content);
         Assert.Equal(HttpStatusCode.Created, response.StatusCode);
         Assert.Equal("http://localhost/Profile/foobar", response.Headers.GetValues("Location").First());
         var responseBody = await response.Content.ReadAsStringAsync();
         Assert.Equal(profile, JsonConvert.DeserializeObject<Profile>(responseBody)); //Not necessary?
         _profileStoreMock.Verify(mock => mock.UpsertProfile(profile), Times.Once);//Makes sure mock function was colled
     }
    
    [Fact]
    public async Task AddProfile_Conflict()
    {
        
        var profile = new Profile("foobar", "Foo", "Bar");
        _profileStoreMock.Setup(m => m.GetProfile(profile.Username))
            .ReturnsAsync(profile);
        
        var json = JsonConvert.SerializeObject(profile);
        var content =
            new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"/Profile", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        
        _profileStoreMock.Verify(m=> m.UpsertProfile(profile), Times.Never);
    }


    [Theory]
    [InlineData(null, "Foo", "Bar")]
    [InlineData("", "Foo", "Bar")]
    [InlineData(" ", "Foo", "Bar")]
    [InlineData("foobar", null, "Bar")]
    [InlineData("foobar", "", "Bar")]
    [InlineData("foobar", "  ", "Bar")]
    [InlineData("foobar", "Foo", null)]
    [InlineData("foobar", "Foo", "")]
    [InlineData("foobar", "Foo", "  ")]
    public async Task AddProfile_InvalidArgs(string username, string firstName, string lastName)
    {
        var profile = new Profile(username, firstName, lastName);
        var response = await _httpClient.PostAsync("/Profile",
            new StringContent(JsonConvert.SerializeObject(profile), Encoding.Default, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _profileStoreMock.Verify(mock=> mock.UpsertProfile(profile), Times.Never);
    }
    
    [Fact]
    public async Task UpdateProfile()
    {


        var profile = new Profile("foobar", "Foo", "Bar");
        _profileStoreMock.Setup(m => m.GetProfile(profile.Username))
            .ReturnsAsync(profile);

        var data = new
        {
            firstName = "b",
            lastName = "c"
        };
        var updatedProfile = new Profile("foobar","b","c");
        var json = JsonConvert.SerializeObject(data);
        var content =
            new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/Profile/{profile.Username}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Equal(updatedProfile, JsonConvert.DeserializeObject<Profile>(responseBody));
        _profileStoreMock.Verify(mock=> mock.UpsertProfile(updatedProfile), Times.Once);
    }
    
    [Fact]
    public async Task UpdateProfile_NotFound()
    {
        
        var profile = new Profile("foobar","Foo","Bar");
        var data = new
        {
            firstName = "b",
            lastName = "c"
        };
        var updatedProfile = new Profile("foobar","b","c");
        var json = JsonConvert.SerializeObject(data);
        var content =
            new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/Profile/{profile.Username}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        _profileStoreMock.Verify(mock=> mock.UpsertProfile(updatedProfile), Times.Never);
    
    }
    
    [Theory]
    [InlineData(null, "Bar")]
    [InlineData( " ", "Bar")]
    [InlineData("Foo", null)]
    [InlineData("Foo", " ")]
    public async Task UpdateProfile_InvalidArgs(string firstName, string lastName)
    {
        var profile = new Profile("foobar", "Foo", "Bar");
        _profileStoreMock.Setup(m => m.GetProfile(profile.Username))
            .ReturnsAsync(profile);

        var data = new
        {
            firstName = firstName,
            lastName = lastName
        };
        var updatedProfile = new Profile("foobar","b","c");
        var json = JsonConvert.SerializeObject(data);
        var content =
            new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/Profile/{profile.Username}", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _profileStoreMock.Verify(mock=> mock.UpsertProfile(updatedProfile), Times.Never);
    }

    
}