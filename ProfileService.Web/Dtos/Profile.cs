namespace ProfileService.Web.Dtos;

public record Profile
{
    public Profile(string username, string firstName, string lastName)
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
    }

    public string Username { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
}