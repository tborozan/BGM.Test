using BGM.Test.Web.DAL.Models;
using BGM.Test.Web.Models.ViewModels;

namespace BGM.Test.Web.Mappers;

public static class InvoiceMapper
{
    public static InvoiceVm MapToViewModel(Invoice source) =>
        new()
        {
            Id = source.Id,
            Title = source.Title,
            IsPaid = source.IsPaid,
        };

    public static Invoice MapToEntity(Models.Documents.Invoice document, string fileName) =>
        new()
        {
            FileName = fileName,
            Title = document.Title,
        };


    public static Models.Documents.Invoice MapToDocument(Invoice source) =>
        new()
        {
            Title = source.Title,
        };
}