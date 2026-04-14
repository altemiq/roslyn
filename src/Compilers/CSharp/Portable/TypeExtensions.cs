// -----------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.CodeAnalysis.CSharp;

#pragma warning disable RCS1263, SA1101

/// <summary>
/// The type extensions.
/// </summary>
public static class TypeExtensions
{
    /// <content>
    /// The <see cref="Type"/> extensions.
    /// </content>
    /// <param name="type">The type.</param>
    extension(Type type)
    {
        /// <summary>
        /// Converts the type to the <see cref="Syntax.TypeSyntax"/>.
        /// </summary>
        /// <param name="parameters">The type parameters.</param>
        /// <returns>The type syntax.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters are the wrong length.</exception>
        public Syntax.NameSyntax ToTypeSyntax(IEnumerable<Syntax.TypeSyntax> parameters)
        {
            if (!type.IsGenericTypeDefinition)
            {
                return type.FullName is { } fullName
                    ? SyntaxFactory.QualifiedName(fullName)
                    : throw new InvalidOperationException();
            }

            var index = type.Name.IndexOf('`');
            var count = int.Parse(type.Name.Substring(index + 1), System.Globalization.CultureInfo.InvariantCulture);

            var parameterList = SyntaxFactory.SeparatedList(parameters);
            if (count != parameterList.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(parameters));
            }

            var name = type.Name.Substring(0, index);
            var genericName = SyntaxFactory.GenericName(SyntaxFactory.Identifier(name), SyntaxFactory.TypeArgumentList(parameterList));
            return type is { Namespace: { } n }
                ? SyntaxFactory.QualifiedName(SyntaxFactory.QualifiedName(n), genericName)
                : genericName;
        }
    }
}