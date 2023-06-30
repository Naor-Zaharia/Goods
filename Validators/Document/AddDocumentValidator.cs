using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Goods.Data;
using Goods.Dtos.Document;
using Microsoft.EntityFrameworkCore;

namespace Goods.Validators
{
    public class AddDocumentValidator : AbstractValidator<AddDocumentDto>
    {
        public AddDocumentValidator()
        {
            RuleFor(document => document.DocType).Must(t => t == 'S' || t == 'P').WithMessage("The document type is unkown");
            RuleFor(document => document.orderLines).Must(orderLines => orderLines != null && orderLines.Any()).WithMessage("The document has no lines");
        }
    }
}