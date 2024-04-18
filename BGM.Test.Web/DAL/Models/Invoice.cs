using System.ComponentModel.DataAnnotations;

namespace BGM.Test.Web.DAL.Models;

public class Invoice
{
    public Guid Id { get; set; }

    [MaxLength(128)]
    public required string FileName { get; set; }

    [MaxLength(128)]
    public required string Title { get; set; }

    public bool IsPaid { get; set; }
}