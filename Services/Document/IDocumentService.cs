using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goods.Dtos.Document;
namespace Goods.Services.Document
{
    public interface IDocumentService
    {

        Task<ServiceResponse<GetDocumentDto>> GetDocument(int docId, char docType, CancellationToken token);
        Task<ServiceResponse<AddDocumentDto>> AddDocument(AddDocumentDto newDocument, CancellationToken token);
        Task<ServiceResponse<UpdateDocumentDto>> UpdateDocument(UpdateDocumentDto updatedUser, CancellationToken token);
        Task<ServiceResponse<string>> DeleteDocument(int id, char documentType, CancellationToken token);
    }
}