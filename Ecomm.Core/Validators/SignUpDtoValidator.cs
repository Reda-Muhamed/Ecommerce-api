using Ecomm.Core.DTOs;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Ecomm.Core.Validators
{
    public class SignUpDtoValidator : AbstractValidator<SignUpDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public SignUpDtoValidator(IUserRepository userRepository, IPasswordService securityService)
        {
            _userRepository = userRepository;
            _passwordService = securityService;

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(256).WithMessage("Email must be at most 256 characters.");



            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .MaximumLength(128).WithMessage("Password must be at most 128 characters.");
                // CustomAsync used to surface detailed password policy errors from ISecurityService

            RuleFor(x => x.FirstName)
                .MaximumLength(100).WithMessage("First name must be at most 100 characters.")
                .Unless(x => string.IsNullOrWhiteSpace(x.FirstName));
        }

      

       
    }
}
