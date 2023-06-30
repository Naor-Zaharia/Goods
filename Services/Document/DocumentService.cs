using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Goods.Data;
using Goods.Dtos.Document;
using Goods.Services.UserService;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Goods.Validators;


namespace Goods.Services.Document
{
    public class DocumentService : IDocumentService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public DocumentService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<AddDocumentDto>> AddDocument(AddDocumentDto newDocument, CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<AddDocumentDto>();
            var bpActiveCheck = _context.BusinessPartners.Where(c => c.BPCode == newDocument.BPCode).FirstOrDefaultAsync().GetAwaiter().GetResult()?.Active;

            if (bpActiveCheck.Equals(null))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "This business partner doesen't exist ";
                return serviceResponse;
            }

            if (bpActiveCheck == false)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "It’s not possible to add a document for a business partner that is not active";
                return serviceResponse;
            }

            foreach (var orderLine in newDocument.orderLines)
            {
                var bpActiveItemCheck = _context.Items.Where(c => c.ItemCode == orderLine.ItemCode).FirstOrDefaultAsync().GetAwaiter().GetResult()?.Active;
                if (bpActiveItemCheck.Equals(null))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Item number " + orderLine.ItemCode + " doesn't exist";
                    return serviceResponse;
                }

                if (bpActiveItemCheck == false)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Item number " + orderLine.ItemCode + " is not active";
                    return serviceResponse;
                }
            }

            if (newDocument.DocType == 'S')
            {
                return await AddSaleDocument(newDocument, token);
            }
            else
            {
                return await AddPurchaseDocument(newDocument, token);
            }
        }
        public async Task<ServiceResponse<AddDocumentDto>> AddSaleDocument(AddDocumentDto newDocument, CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<AddDocumentDto>();
            var bpTypeCheck = _context.BusinessPartners.Where(c => c.BPCode == newDocument.BPCode).FirstOrDefaultAsync().GetAwaiter().GetResult()?.BPType;

            if (bpTypeCheck == 'V')
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "It’s not possible to add a sale document for a business partner of type V";
                return serviceResponse;
            }

            await _context.SaleOrders.AddAsync(new SaleOrders
            {
                BPCode = newDocument.BPCode,
                CreateDate = DateTime.Now.Date,
                LastUpdateDate = null,
                CreatedBy = newDocument.CreatedBy,
                LastUpdatedBy = null
            }, token);

            var docId = _context.SaleOrders.DefaultIfEmpty().Max(c => c == null ? 0 : c.Id) + 1;

            foreach (var orderLine in newDocument.orderLines)
            {
                await _context.SaleOrdersLines.AddAsync(new SaleOrdersLines
                {
                    DocId = docId,
                    ItemCode = orderLine.ItemCode,
                    Quantity = orderLine.Quantity,
                    CreateDate = DateTime.Now.Date,
                    LastUpdateDate = null,
                    CreatedBy = newDocument.CreatedBy,
                    LastUpdatedBy = null
                }, token);
            }

            await _context.SaleOrdersLinesComments.AddAsync(new SaleOrdersLinesComments
            {
                DocId = docId,
                LineId = _context.SaleOrdersLines.DefaultIfEmpty().Max(c => c == null ? 0 : c.LineId) + 1,
                Comment = newDocument.Comment
            }, token);

            await _context.SaveChangesAsync();

            serviceResponse.Data = newDocument;

            return serviceResponse;

        }

        public async Task<ServiceResponse<AddDocumentDto>> AddPurchaseDocument(AddDocumentDto newDocument, CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<AddDocumentDto>();
            var bpTypeCheck = _context.BusinessPartners.Where(c => c.BPCode == newDocument.BPCode).FirstOrDefaultAsync().GetAwaiter().GetResult()?.BPType;

            if (bpTypeCheck == 'C')
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "It’s not possible to add a purchase document for a business partner of type C";
                return serviceResponse;
            }

            await _context.PurchaseOrders.AddAsync(new PurchaseOrders
            {
                BPCode = newDocument.BPCode,
                CreateDate = DateTime.Now.Date,
                LastUpdateDate = null,
                CreatedBy = newDocument.CreatedBy,
                LastUpdatedBy = null
            }, token);

            foreach (var orderLine in newDocument.orderLines)
            {
                await _context.PurchaseOrdersLines.AddAsync(new PurchaseOrdersLines
                {
                    DocId = _context.PurchaseOrders.DefaultIfEmpty().Max(c => c == null ? 0 : c.Id) + 1,
                    ItemCode = orderLine.ItemCode,
                    Quantity = orderLine.Quantity,
                    CreateDate = DateTime.Now.Date,
                    LastUpdateDate = null,
                    CreatedBy = newDocument.CreatedBy,
                    LastUpdatedBy = null
                }, token);
            }

            await _context.SaveChangesAsync();
            serviceResponse.Data = newDocument;

            return serviceResponse;
        }

        public async Task<ServiceResponse<string>> DeleteDocument(int id, char documentType, CancellationToken token)
        {
            if (documentType == 'S')
            {
                return await DeleteSaleDocument(id, token);
            }
            else
            {
                return await DeletePurchaseDocument(id, token);
            }
        }

        public async Task<ServiceResponse<string>> DeleteSaleDocument(int id, CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<string>();

            try
            {
                var dbSaleOrder = await _context.SaleOrders.FirstOrDefaultAsync(c => c.Id == id);
                if (dbSaleOrder is null)
                {
                    throw new Exception($"Document with Id '{id}' not found.");
                }
                await _context.SaleOrders.Where(c => c.Id == id).ExecuteDeleteAsync(token);
                await _context.SaleOrdersLines.Where(c => c.DocId == id).ExecuteDeleteAsync(token);
                await _context.SaleOrdersLinesComments.Where(c => c.DocId == id).ExecuteDeleteAsync(token);

                serviceResponse.Data = "The document was deleted";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<string>> DeletePurchaseDocument(int id, CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<string>();

            try
            {
                var dbPurchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(c => c.Id == id);
                if (dbPurchaseOrder is null)
                {
                    throw new Exception($"Document with Id '{id}' not found.");
                }
                await _context.PurchaseOrders.Where(c => c.Id == id).ExecuteDeleteAsync(token);
                await _context.PurchaseOrdersLines.Where(c => c.DocId == id).ExecuteDeleteAsync(token);

                serviceResponse.Data = "The document was deleted";

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<UpdateDocumentDto>> UpdateDocument(UpdateDocumentDto newDocument, CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<UpdateDocumentDto>();
            var bpActiveCheck = _context.BusinessPartners.Where(c => c.BPCode == newDocument.BPCode).FirstOrDefaultAsync().GetAwaiter().GetResult()?.Active;

            if (bpActiveCheck.Equals(null))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "This business partner doesen't exist ";
                return serviceResponse;
            }

            if (bpActiveCheck == false)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "It’s not possible to change the business partner to an inactive one";
                return serviceResponse;
            }

            foreach (var orderLine in newDocument.orderLines)
            {
                var bpActiveItemCheck = _context.Items.Where(c => c.ItemCode == orderLine.ItemCode).FirstOrDefaultAsync().GetAwaiter().GetResult()?.Active;
                if (bpActiveItemCheck.Equals(null))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Item number " + orderLine.ItemCode + " doesn't exist";
                    return serviceResponse;
                }

                if (bpActiveItemCheck == false)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Item number " + orderLine.ItemCode + " is not active";
                    return serviceResponse;
                }
            }

            if (newDocument.DocType == 'S')
            {
                return await UpdateSaleDocument(newDocument, token);
            }
            else
            {
                return await UpdatePurchaseDocument(newDocument, token);
            }
        }
        public async Task<ServiceResponse<UpdateDocumentDto>> UpdateSaleDocument(UpdateDocumentDto newDocument, CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<UpdateDocumentDto>();
            var validBPCheck = await _context.SaleOrders.Where(c => c.Id == newDocument.ID).ToListAsync();
            var bpTypeCheck = _context.BusinessPartners.Where(c => c.BPCode == newDocument.BPCode).FirstOrDefaultAsync().GetAwaiter().GetResult()?.BPType;

            if (validBPCheck.Count < 1)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "This document doesen't exist ";
                return serviceResponse;
            }

            if (bpTypeCheck == 'V')
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "It’s not possible to use a business partner of type V in a sale document";
                return serviceResponse;
            }

            await _context.SaleOrders.Where(c => c.Id == newDocument.ID)
                .ExecuteUpdateAsync(c => c.SetProperty(c => c.BPCode, c => newDocument.BPCode)
                .SetProperty(c => c.LastUpdateDate, c => DateTime.Now.Date)
                .SetProperty(c => c.LastUpdatedBy, c => newDocument.CreatedBy));

            var dbSaleOrdersLines = await _context.SaleOrdersLines.Where(c => c.DocId == newDocument.ID).ToListAsync();
            var existingorderLinesIds = await _context.SaleOrdersLines.Where(c => c.DocId == newDocument.ID).Select(c => c.ItemCode).ToListAsync();
            var updatedLineId = newDocument.orderLines.Select(c => c.ItemCode).ToList();
            var updatedItems = existingorderLinesIds.Intersect(updatedLineId).ToHashSet();
            var newItems = updatedLineId.Except(existingorderLinesIds).ToHashSet();
            var removeItems = existingorderLinesIds.Except(updatedLineId).ToHashSet();

            foreach (var orderLine in newDocument.orderLines)
            {
                if (updatedItems.Contains(orderLine.ItemCode))
                {
                    await _context.SaleOrdersLines.Where(c => c.ItemCode == orderLine.ItemCode && c.DocId == newDocument.ID)
                    .ExecuteUpdateAsync(c => c.SetProperty(c => c.Quantity, c => orderLine.Quantity)
                    .SetProperty(c => c.LastUpdateDate, c => DateTime.Now.Date)
                    .SetProperty(c => c.LastUpdatedBy, c => newDocument.CreatedBy));
                }
            }

            foreach (var orderLine in newDocument.orderLines)
            {
                if (newItems.Contains(orderLine.ItemCode))
                {
                    await _context.SaleOrdersLines.AddAsync(new SaleOrdersLines
                    {
                        DocId = newDocument.ID,
                        ItemCode = orderLine.ItemCode,
                        Quantity = orderLine.Quantity,
                        CreateDate = DateTime.Now.Date,
                        LastUpdateDate = null,
                        CreatedBy = newDocument.CreatedBy,
                        LastUpdatedBy = null
                    }, token);
                }
            }

            await _context.SaleOrdersLines.Where(c => removeItems.Contains(c.ItemCode) && c.DocId == newDocument.ID).ExecuteDeleteAsync(token);

            await _context.SaveChangesAsync();

            await _context.SaleOrdersLinesComments.Where(c => c.DocId == newDocument.ID)
            .ExecuteUpdateAsync(c => c.SetProperty(c => c.Comment, c => newDocument.Comment));

            serviceResponse.Data = newDocument;

            return serviceResponse;
        }

        public async Task<ServiceResponse<UpdateDocumentDto>> UpdatePurchaseDocument(UpdateDocumentDto newDocument, CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<UpdateDocumentDto>();
            var validBPCheck = await _context.PurchaseOrders.Where(c => c.Id == newDocument.ID).ToListAsync();
            var bpTypeCheck = _context.BusinessPartners.Where(c => c.BPCode == newDocument.BPCode).FirstOrDefaultAsync().GetAwaiter().GetResult()?.BPType;

            if (validBPCheck.Count < 1)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "This document doesen't exist ";
                return serviceResponse;
            }

            if (bpTypeCheck == 'C')
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "It’s not possible to use a business partner of type C in a purchase document";
                return serviceResponse;
            }

            await _context.PurchaseOrders.Where(c => c.Id == newDocument.ID)
                .ExecuteUpdateAsync(c => c.SetProperty(c => c.BPCode, c => newDocument.BPCode)
                .SetProperty(c => c.LastUpdateDate, c => DateTime.Now.Date)
                .SetProperty(c => c.LastUpdatedBy, c => newDocument.CreatedBy));

            var dbPurchaseOrdersLines = await _context.PurchaseOrdersLines.Where(c => c.DocId == newDocument.ID).ToListAsync();
            var existingPurchaseLinesIds = await _context.PurchaseOrdersLines.Where(c => c.DocId == newDocument.ID).Select(c => c.ItemCode).ToListAsync();
            var updatedLineId = newDocument.orderLines.Select(c => c.ItemCode).ToList();
            var updatedItems = existingPurchaseLinesIds.Intersect(updatedLineId).ToHashSet();
            var newItems = updatedLineId.Except(existingPurchaseLinesIds).ToHashSet();
            var removeItems = existingPurchaseLinesIds.Except(updatedLineId).ToHashSet();

            foreach (var orderLine in newDocument.orderLines)
            {
                if (updatedItems.Contains(orderLine.ItemCode))
                {
                    await _context.PurchaseOrdersLines.Where(c => c.ItemCode == orderLine.ItemCode && c.DocId == newDocument.ID)
                    .ExecuteUpdateAsync(c => c.SetProperty(c => c.Quantity, c => orderLine.Quantity)
                    .SetProperty(c => c.LastUpdateDate, c => DateTime.Now.Date)
                    .SetProperty(c => c.LastUpdatedBy, c => newDocument.CreatedBy));
                }
            }

            foreach (var orderLine in newDocument.orderLines)
            {
                if (newItems.Contains(orderLine.ItemCode))
                {
                    await _context.PurchaseOrdersLines.AddAsync(new PurchaseOrdersLines
                    {
                        DocId = newDocument.ID,
                        ItemCode = orderLine.ItemCode,
                        Quantity = orderLine.Quantity,
                        CreateDate = DateTime.Now.Date,
                        LastUpdateDate = null,
                        CreatedBy = newDocument.CreatedBy,
                        LastUpdatedBy = null
                    }, token);
                }
            }

            await _context.PurchaseOrdersLines.Where(c => removeItems.Contains(c.ItemCode) && c.DocId == newDocument.ID).ExecuteDeleteAsync(token);

            await _context.SaveChangesAsync();

            serviceResponse.Data = newDocument;

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetDocumentDto>> GetDocument(int docId, char docType, CancellationToken token)
        {
            if (docType == 'S')
            {
                return await GetSaleDocument(docId, docType, token);
            }
            else
            {
                return await GetPurchaseDocument(docId, docType, token);
            }
        }

        public async Task<ServiceResponse<GetDocumentDto>> GetSaleDocument(int docId, char docType, CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<GetDocumentDto>();
            var options = new DbContextOptionsBuilder<DataContext>();

            var itemsLines = await _context.SaleOrdersLines
    .Where(sol => sol.DocId == docId)
    .Join(_context.Items, sol => sol.ItemCode, item => item.ItemCode, (sol, item) => new OrderLineGetDocument
    {
        ItemCode = sol.ItemCode,
        Quantity = sol.Quantity,
        ItemName = item.ItemName,
        Active = item.Active
    })
    .ToListAsync();

            var document = await _context.SaleOrders
        .Where(so => so.Id == docId)
        .Join(_context.BusinessPartners, s => s.BPCode, bp => bp.BPCode, (s, bp) => new { SaleOrder = s, BusinessPartner = bp })
        .Join(_context.Users, s => s.SaleOrder.CreatedBy, u => u.Id, (s, u) => new { s.SaleOrder, s.BusinessPartner, User = u })
        .Join(_context.SaleOrdersLinesComments, s => s.SaleOrder.Id, soc => soc.DocId, (s, soc) => new { s.SaleOrder, s.BusinessPartner, s.User, SaleOrderLinesComment = soc })
        .GroupBy(x => new { x.SaleOrder.BPCode, x.SaleOrder.CreatedBy, x.BusinessPartner.BPName, x.BusinessPartner.Active, x.User.FullName })
        .Select(g => new GetDocumentDto
        {
            DocType = docType,
            BPCode = g.Key.BPCode,
            CreatedBy = g.Key.CreatedBy,
            orderLines = itemsLines,
            Comment = g.Select(x => x.SaleOrderLinesComment.Comment).FirstOrDefault(),
            BPName = g.Key.BPName,
            Active = g.Key.Active,
            FullName = g.Key.FullName
        })
.FirstOrDefaultAsync();

            serviceResponse.Data = document;

            if (serviceResponse.Data == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "No document with id " + docId + " exists";
                serviceResponse.Success = false;
            }

            return serviceResponse;

        }

        public async Task<ServiceResponse<GetDocumentDto>> GetPurchaseDocument(int docId, char docType, CancellationToken token)
        {
            var serviceResponse = new ServiceResponse<GetDocumentDto>();
            var options = new DbContextOptionsBuilder<DataContext>();

            var itemsLines = await _context.PurchaseOrdersLines
    .Where(pol => pol.DocId == docId)
    .Join(_context.Items, pol => pol.ItemCode, item => item.ItemCode, (pol, item) => new OrderLineGetDocument
    {
        ItemCode = pol.ItemCode,
        Quantity = pol.Quantity,
        ItemName = item.ItemName,
        Active = item.Active
    })
    .ToListAsync();

            var document = await _context.PurchaseOrders
    .Where(po => po.Id == docId)
    .Join(_context.BusinessPartners, p => p.BPCode, bp => bp.BPCode, (p, bp) => new { PurchaseOrders = p, BusinessPartner = bp })
    .Join(_context.Users, s => s.PurchaseOrders.CreatedBy, u => u.Id, (s, u) => new { s.PurchaseOrders, s.BusinessPartner, User = u })
    .GroupBy(x => new { x.PurchaseOrders.BPCode, x.PurchaseOrders.CreatedBy, x.BusinessPartner.BPName, x.BusinessPartner.Active, x.User.FullName })
    .Select(g => new GetDocumentDto
    {
        DocType = docType,
        BPCode = g.Key.BPCode,
        CreatedBy = g.Key.CreatedBy,
        orderLines = itemsLines,
        BPName = g.Key.BPName,
        Active = g.Key.Active,
        FullName = g.Key.FullName
    })
    .FirstOrDefaultAsync();

            serviceResponse.Data = document;

            if (serviceResponse.Data == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "No document with id " + docId + " exists";
                serviceResponse.Success = false;
            }

            return serviceResponse;


        }
    }
}


