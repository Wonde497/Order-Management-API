using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.DTOs;
public record UpdateCustomerDto(
[Required, StringLength(100)] string Fullname,
        [Required, EmailAddress, StringLength(150)] string Email,
        [Phone, StringLength(20)] string? Phone
);