// <copyright file="ProductionValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentValidation;

namespace Wpp.StorageAutomation.Production
{
    /// <summary>
    ///   The production validators.
    /// </summary>
    public class ProductionValidator : AbstractValidator<ProductionRequest>
    {
        /// <summary>Initializes a new instance of the <see cref="ProductionValidator" /> class.</summary>
        public ProductionValidator()
        {
            this.RuleFor(x => x.ProductionName).NotEmpty().MinimumLength(8).MaximumLength(255).Matches(@"^[^\\/:|<>*?]*$").WithMessage(@"Special characters like '\ / : \| < > * ?' are not allowed in Production/ subDirectory names");
            this.RuleFor(x => x.ProductionStoreUri).NotEmpty().MaximumLength(255);
            this.RuleFor(x => x.DirectoryTree).NotNull();
            this.RuleFor(x => x.Tokens).NotNull();
            this.RuleForEach(x => x.Tokens).ChildRules(item =>
            {
                item.RuleFor(x => x.ProductionToken).NotEmpty().MinimumLength(8).MaximumLength(255).Matches(@"^[^\\/:|<>*?]*$").WithMessage(@"Special characters like '\ / : \| < > * ?' are not allowed in Production/ subDirectory names");
            });
            this.RuleForEach(x => x.DirectoryTree).ChildRules(item =>
            {
                item.RuleFor(x => x.Name).NotEmpty().MaximumLength(255).Matches(@"^[^\\/:|<>*?]*$").WithMessage(@"Special characters like '\ / : \| < > * ?' are not allowed in Production/ subDirectory names");
                item.RuleFor(x => x.Type).NotNull();
                item.RuleFor(x => x.Path).NotEmpty().MaximumLength(255).Matches(@"^[^\\:|<>*?]*$").WithMessage(@"Special characters like '\ : \| < > * ?' are not allowed in Production/ subDirectory names");
            });
        }
    }
}
