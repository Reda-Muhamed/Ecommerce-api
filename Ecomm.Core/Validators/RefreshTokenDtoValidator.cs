using Ecomm.Core.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Validators
{
    public class RefreshTokenDtoValidator:AbstractValidator<RefreshTokensDto>
    {
        public RefreshTokenDtoValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}
