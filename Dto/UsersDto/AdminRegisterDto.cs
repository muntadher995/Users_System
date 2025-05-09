// DTOs/UserRegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.API.DTOs;


public class AdminRegisterDto
{

    public string? AdminName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? phone { get; set; }

    public string? approve_status { get; set; }

    public bool status { get; set; } 


}