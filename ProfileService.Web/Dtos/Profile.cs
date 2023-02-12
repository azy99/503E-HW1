using System.ComponentModel.DataAnnotations;

namespace ProfileService.Web.Dtos;

public record Profile(
    [Required] string Username,
    [Required] string FirstName,
    [Required] string LastName
);

