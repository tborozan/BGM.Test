namespace BGM.Test.Web.Models.ViewModels;

public record InvoiceVm()
{
    public Guid Id { get; set; }

    public required string Title { get; set; }
    
    public required bool IsPaid { get; set; }
}