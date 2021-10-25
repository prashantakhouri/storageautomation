// <copyright file="ArchiveProductionValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentValidation;

namespace Wpp.StorageAutomation.DataMovement.Models.Validators
{
    /// <summary>
    /// Archive production validator.
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator{Wpp.StorageAutomation.DataMovement.Models.ArchiveProductionRequest}" />
    public class ArchiveProductionValidator : AbstractValidator<ProductionRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveProductionValidator"/> class.
        /// </summary>
        public ArchiveProductionValidator()
        {
            this.RuleFor(x => x.ProductionStoreId).NotEmpty().MaximumLength(255).Matches(@"^[\sa-zA-Z0-9-]*$");
            this.RuleFor(x => x.ProductionId).NotEmpty().MaximumLength(255).Matches(@"^[\sa-zA-Z0-9-]*$");
        }
    }
}
