﻿using System.ComponentModel.DataAnnotations;

namespace API.Dtos;
public class AddRoleDTO
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }
}
