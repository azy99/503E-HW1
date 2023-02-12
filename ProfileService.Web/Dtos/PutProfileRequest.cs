using System.ComponentModel.DataAnnotations;

namespace ProfileService.Web.Dtos;

public record PutProfileRequest([Required] string FirstName, [Required] string LastName);