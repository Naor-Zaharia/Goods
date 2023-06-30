using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Goods.Dtos.Document;
using Goods.Services.Document;
using FluentValidation;
using Goods.Validators;


namespace Goods.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentControler : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentControler(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetDocumentDto>>> GetDocument(int id, char docType, CancellationToken token)
        {
            var response = await _documentService.GetDocument(id, docType, token);
            if (response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<AddDocumentDto>>> AddDocument(AddDocumentDto newDocument, CancellationToken token)
        {
            AddDocumentValidator validDoc = new AddDocumentValidator();
            var errorsList = validDoc.Validate(newDocument).Errors;
            if (errorsList.Count > 0)
            {
                return BadRequest(errorsList);
            }

            var response = await _documentService.AddDocument(newDocument, token);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<UpdateDocumentDto>>> UpdateDocument(UpdateDocumentDto newDocument, CancellationToken token)
        {
            UpdateDocumentValidator validDoc = new UpdateDocumentValidator();
            var errorsList = validDoc.Validate(newDocument).Errors;

            if (errorsList.Count > 0)
            {
                return BadRequest(errorsList);
            }

            var response = await _documentService.UpdateDocument(newDocument, token);

            if (response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteDocument(int id, char documentType, CancellationToken token)
        {
            var response = await _documentService.DeleteDocument(id, documentType, token);
            if (response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}