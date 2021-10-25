// <copyright file="ProductionStoreIdValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentValidation;

namespace Wpp.StorageAutomation.Common
{
    /// <summary>
    /// Production store validator.
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator{string}" />
    public class ProductionStoreIdValidator : AbstractValidator<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionStoreIdValidator"/> class.
        /// </summary>
        public ProductionStoreIdValidator()
        {
            this.RuleFor(x => x).NotEmpty().NotNull().WithMessage(@"Production store ID cannot be empty.");
            this.RuleFor(x => x).MaximumLength(255).WithMessage(@"Production store ID length cannot be greater than 255 characters.");
            this.RuleFor(x => x).Matches(@"^[A-Za-z0-9-]*$").WithMessage(@"Production store Id can only contain alphabets, numbers and hyphens.");
        }
    }
}
