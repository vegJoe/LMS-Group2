using System.ComponentModel.DataAnnotations;

namespace LMS.API.Models.Dtos;

public record UserForRegistrationDto
{
    [Required(ErrorMessage = "Username is required")]
    public string? UserName { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public int? CourseId { get; init; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; init; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string? Email { get; init; }
    public string Role { get; set; } = "Student"; // Default role
}
