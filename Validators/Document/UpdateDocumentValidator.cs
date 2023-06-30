using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Goods.Dtos.Document;

namespace Goods.Validators
{
    public class UpdateDocumentValidator : AbstractValidator<UpdateDocumentDto>
    {
        public UpdateDocumentValidator()
        {
            RuleFor(document => document.orderLines).Must(orderLines => orderLines != null && orderLines.Any()).WithMessage("It’s not possible to delete all the document lines");
        }
    }
}