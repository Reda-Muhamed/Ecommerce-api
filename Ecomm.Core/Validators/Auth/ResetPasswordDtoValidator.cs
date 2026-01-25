using Ecomm.Core.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Validators.Auth
{
    public class ResetPasswordDtoValidator: AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator() {
            RuleFor(x => x.NewPassword)
             .NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.UserId)
             .NotEmpty().WithMessage("UserId is required.");
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.");
        }
    }
}
