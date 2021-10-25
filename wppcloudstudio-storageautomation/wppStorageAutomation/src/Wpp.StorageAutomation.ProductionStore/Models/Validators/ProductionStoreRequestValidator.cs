// <copyright file="ProductionStoreRequestValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentValidation;

namespace Wpp.StorageAutomation.ProductionStore
{
    /// <summary>
    ///   The production store request validators.
    /// </summary>
    public class ProductionStoreRequestValidator : AbstractValidator<ProductionStoreRequest>
    {
        /// <summary>Initializes a new instance of the <see cref="ProductionStoreRequestValidator" /> class.</summary>
        public ProductionStoreRequestValidator()
        {
            this.RuleFor(x => x.Name).NotEmpty().WithMessage("Production Store name cannot be empty for registration.");
            this.RuleFor(x => x.Name).MaximumLength(255).WithMessage("Production Store name cannot be greater than 255 characters.");
            this.RuleFor(x => x.Name).Matches(@"^[a-z0-9-]*$").WithMessage(@"Production store Name can only contain lower case alphabets, numbers and hyphens.");
            this.RuleFor(x => x.Region).NotEmpty().WithMessage("Production Store region cannot be empty for registration.");
            this.RuleFor(x => x.Region).MaximumLength(255).WithMessage("Production Store region cannot be greater than 255 characters.");
            this.RuleFor(x => x.WIPURL).NotEmpty().WithMessage("Production Store WIPURL cannot be empty for registration.");
            this.RuleFor(x => x.ArchiveURL).NotEmpty().WithMessage("Production Store ArchiveURL cannot be empty for registration.");
            this.RuleFor(x => x.UserRoleGroupNames).NotEmpty().WithMessage("Production Store UserRoleGroupNames cannot be empty for registration.");
            this.RuleFor(x => x.ManagerRoleGroupNames).NotEmpty().WithMessage("Production Store ManagerRoleGroupNames cannot be empty for registration.");
            this.RuleFor(x => x.WipkeyName).NotEmpty().WithMessage("Wip key name cannot be empty for registration.");
            this.RuleFor(x => x.ArchiveKeyName).NotEmpty().WithMessage("Archive key name cannot be empty for registration.");
        }
    }
}
