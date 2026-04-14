// -----------------------------------------------------------------------
// <copyright file="SyntaxFactoryExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.CodeAnalysis.CSharp;

/// <summary>
/// The <see cref="SyntaxFactory"/> extensions.
/// </summary>
public static class SyntaxFactoryExtensions
{
    /// <content>
    /// The <see cref="SyntaxFactory"/> extensions.
    /// </content>
    extension(SyntaxFactory)
    {
        /// <summary>
        /// Creates a qualified <see cref="Syntax.NameSyntax"/> node.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <returns><see cref="Syntax.NameSyntax"/>.</returns>
        public static Syntax.NameSyntax QualifiedName(string fullName) => SyntaxFactory.QualifiedName(fullName.Split('.').Select(SyntaxFactory.IdentifierName));

        /// <summary>
        /// Creates a qualified <see cref="Syntax.NameSyntax"/> node.
        /// </summary>
        /// <param name="names">The name parts.</param>
        /// <returns><see cref="Syntax.NameSyntax"/>.</returns>
        public static Syntax.NameSyntax QualifiedName(IEnumerable<Syntax.SimpleNameSyntax> names) => Syntax.SyntaxExtensions.ToQualifiedName(names);

        /// <summary>
        /// Creates a <see cref="Syntax.TypeSyntax"/> from the <see cref="ITypeSymbol"/>.
        /// </summary>
        /// <param name="symbol">The type symbol.</param>
        /// <returns><see cref="Syntax.TypeSyntax"/>.</returns>
        public static Syntax.TypeSyntax Type(ITypeSymbol symbol) => symbol.ToType();
    }
}