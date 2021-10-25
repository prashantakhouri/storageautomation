// <copyright file="ExtensionMethod.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

namespace ExtensionMethod
{
    /// <summary>
    ///   <br />
    /// </summary>
    public static class ExtensionMethod
    {
        /// <summary>Removes the special chars.</summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static string RemoveSpecialChars(this string input)
        {
            return Regex.Replace(input, "[@,//./;'\\\\]", string.Empty);
        }

        /// <summary>
        /// Removes the slash.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public static string RemoveSlash(this string input)
        {
            return Regex.Replace(input, "[@//./;'\\\\]", string.Empty);
        }
    }
}
