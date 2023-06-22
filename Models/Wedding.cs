#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models;

public class Wedding
{
    [Key]
    public int WeddingId { get; set; }
    [Required]
    public string? WedderOne { get; set; }
    [Required]
    public string? WedderTwo { get; set; }
    [Required]
    [ValidateDate]
    public DateTime? DateOfWed { get; set; }
    [Required]
    public string? Address { get; set; }

    [Required]
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public List<RSVP>? RSVPs { get; set; }
    public User? User { get; set; }
}

public class ValidateDate : ValidationAttribute
{
    protected override ValidationResult IsValid
    (object date, ValidationContext validationContext)
    {
        return ((DateTime)date >= DateTime.Now)
            ? ValidationResult.Success
            : new ValidationResult("Date must be in the future");
    }
}