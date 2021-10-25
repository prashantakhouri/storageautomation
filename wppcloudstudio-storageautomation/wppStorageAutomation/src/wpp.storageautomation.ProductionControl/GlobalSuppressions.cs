// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Minor Code Smell", "S1450:Private fields only used as local variables in methods should become local variables", Justification = "As per requirement.")]
[assembly: SuppressMessage("Usage", "CA2200:Rethrow to preserve stack details", Justification = "As per need.")]
[assembly: SuppressMessage("Major Code Smell", "S3445:Exceptions should not be explicitly rethrown", Justification = "As per need.")]
[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "To be changed.")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "All classes will be used together")]
[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1200:Using directives should be placed correctly", Justification = "As per requirement.")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "As per requirement.")]
