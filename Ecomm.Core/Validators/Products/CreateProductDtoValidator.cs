using System;
using System.Collections.Generic;
using System.Text;
using Ecomm.Core.DTOs.Products;
using FluentValidation;

namespace Ecomm.Core.Validators.Products
{
    
    public class CreateProductDtoValidator
        : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("Category is required");

            RuleFor(x => x.BrandId)
                .Must(id => id == null || id != Guid.Empty)
                .WithMessage("Invalid brand id");

           
        }
    }

}
