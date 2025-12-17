using Ecomm.Core.DTOs;
using Ecomm.Core.Services;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Ecomm.Core.Validators
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        private readonly IPasswordService passwordService;

        public ChangePasswordDtoValidator(IPasswordService passwordService)
        {

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("New password must be at least 8 characters.")
                .MaximumLength(128).WithMessage("New password must be at most 128 characters.")
                .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from current password.")
                .CustomAsync(ValidatePasswordStrengthAsync);
            this.passwordService = passwordService;
        }

        private async Task ValidatePasswordStrengthAsync(string newPassword, ValidationContext<ChangePasswordDto> context, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(newPassword)) return;

            var result = await passwordService.ValidatePasswordStrengthAsync(newPassword, ct);
            if (!result.IsValid)
            {
                foreach (var err in result.Errors)
                    context.AddFailure("NewPassword", err);
            }
        }
    }
}
