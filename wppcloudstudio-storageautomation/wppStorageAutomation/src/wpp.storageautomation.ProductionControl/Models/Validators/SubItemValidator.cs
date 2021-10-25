// <copyright file="SubItemValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentValidation;

namespace Wpp.StorageAutomation
{
    /// <summary>
    /// Class SubItemValidator.
    /// Implements the <see cref="FluentValidation.AbstractValidator{Wpp.StorageAutomation.SubItem}" />.
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator{Wpp.StorageAutomation.SubItem}" />
    public class SubItemValidator : AbstractValidator<SubItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubItemValidator"/> class.
        /// </summary>
        public SubItemValidator()
        {
            this.RuleFor(x => x.Name).NotEmpty().MaximumLength(255).Matches(@"^[^\\/:|<>*?]*$").WithMessage(@"Special characters like '\ / : \| < > * ?' are not allowed in Production/ subDirectory names");
            this.RuleFor(x => x.Path).NotEmpty().MaximumLength(255).Matches(@"^[^\\:|<>*?]*$").WithMessage(@"Special characters like '\ : \| < > * ?' are not allowed in Production/ subDirectory names");
            this.RuleForEach(x => x.SubItems).ChildRules(item =>
            {
                item.RuleFor(x => x.Name).NotEmpty().MaximumLength(255).Matches(@"^[^\\/:|<>*?]*$").WithMessage(@"Special characters like '\ / : \| < > * ?' are not allowed in Production/ subDirectory names");
                item.RuleFor(x => x.Type).NotNull();
                item.RuleFor(x => x.Path).NotEmpty().MaximumLength(255).Matches(@"^[^\\:|<>*?]*$").WithMessage(@"Special characters like '\ : \| < > * ?' are not allowed in Production/ subDirectory names");
            });
        }
    }
}
