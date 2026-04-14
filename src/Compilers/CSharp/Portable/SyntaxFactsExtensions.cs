// -----------------------------------------------------------------------
// <copyright file="SyntaxFactsExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.CodeAnalysis.CSharp;

/// <summary>
/// The <see cref="SyntaxFacts"/> extensions.
/// </summary>
public static class SyntaxFactsExtensions
{
    /// <content>
    /// The <see cref="SyntaxFacts"/> extensions.
    /// </content>
    extension(SyntaxFacts)
    {
        /// <summary>
        /// Escapes the text if it is a keyword.
        /// </summary>
        /// <param name="text">The text to test.</param>
        /// <returns>The escaped text if <paramref name="text"/> represents a keyword; otherwise <paramref name="text"/>.</returns>
        public static string EscapeKeyword(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            if (

                // Check if it's a reserved keyword
                SyntaxFacts.GetKeywordKind(text) is not SyntaxKind.None

                // Check if it's a contextual keyword
                || SyntaxFacts.GetContextualKeywordKind(text) is not SyntaxKind.None)
            {
                // prepend with an '@' symbol
                return "@" + text;
            }

            return text;
        }
    }
}