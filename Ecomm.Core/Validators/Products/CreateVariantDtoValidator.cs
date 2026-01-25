using Ecomm.Core.DTOs.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Validators.Products
{
    public class CreateVariantDtoValidator:AbstractValidator<CreateVariantDto>
    {
        public CreateVariantDtoValidator() { 
         
            RuleFor(x => x.SKU)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0);
        }
    }
}
