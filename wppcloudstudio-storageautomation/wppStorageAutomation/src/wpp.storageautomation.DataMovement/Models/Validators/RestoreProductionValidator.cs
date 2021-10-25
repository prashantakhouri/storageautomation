// <copyright file="RestoreProductionValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentValidation;

namespace Wpp.StorageAutomation.DataMovement.Models.Validators
{
    /// <summary>
    /// Restore production validator.
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator{Wpp.StorageAutomation.DataMovement.Models.ProductionRequest}" />
    public class RestoreProductionValidator : AbstractValidator<ProductionRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreProductionValidator"/> class.
        /// </summary>
        public RestoreProductionValidator()
        {
            this.RuleFor(x => x.ProductionStoreId).NotEmpty().MaximumLength(255).Matches(@"^[a-zA-Z0-9-]*$").WithMessage(@"Production store Id can only contain alphabets, numbers and hyphens.");
            this.RuleFor(x => x.ProductionId).NotEmpty().MaximumLength(255).Matches(@"^[a-zA-Z0-9-]*$").WithMessage(@"Production Id can only contain alphabets, numbers and hyphens.");
        }
    }
}
