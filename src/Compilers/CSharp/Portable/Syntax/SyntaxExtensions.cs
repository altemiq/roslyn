// -----------------------------------------------------------------------
// <copyright file="SyntaxExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.CodeAnalysis.CSharp.Syntax;

#pragma warning disable RCS1263, SA1101

/// <summary>
/// The syntax extensions.
/// </summary>
public static class SyntaxExtensions
{
    /// <summary>
    /// Converts the simple names to a qualified name.
    /// </summary>
    /// <param name="names">The names to qualify.</param>
    /// <returns>The qualified name.</returns>
    /// <exception cref="InvalidOperationException">Could not generate the name.</exception>
    public static NameSyntax ToQualifiedName(this IEnumerable<SimpleNameSyntax> names)
    {
        var enumerator = names.GetEnumerator();
        _ = enumerator.MoveNext();

        NameSyntax? name = enumerator.Current;
        while (enumerator.MoveNext() && name is not null && enumerator.Current is not null)
        {
            name = SyntaxFactory.QualifiedName(name, enumerator.Current);
        }

        enumerator.Dispose();
        return name ?? throw new InvalidOperationException();
    }
}