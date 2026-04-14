// -----------------------------------------------------------------------
// <copyright file="SymbolExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.CodeAnalysis.CSharp;

#pragma warning disable CA1708, RCS1263, SA1101

/// <summary>
/// The symbol extensions.
/// </summary>
public static class SymbolExtensions
{
    /// <content>
    /// The <see cref="ITypeSymbol"/> extensions.
    /// </content>
    /// <param name="type">The type symbol.</param>
    extension(ITypeSymbol type)
    {
        /// <summary>
        /// Gets the type syntax from the type symbol.
        /// </summary>
        /// <returns>The type syntax.</returns>
        public Syntax.TypeSyntax ToType()
        {
            return type switch
            {
                { SpecialType: SpecialType.System_Object } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),

                // Bytes
                { SpecialType: SpecialType.System_Boolean } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                { SpecialType: SpecialType.System_Char } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.CharKeyword)),
                { SpecialType: SpecialType.System_SByte } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.SByteKeyword)),
                { SpecialType: SpecialType.System_Byte } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword)),

                // Integer
                { SpecialType: SpecialType.System_Int16 } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ShortKeyword)),
                { SpecialType: SpecialType.System_UInt16 } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword)),
                { SpecialType: SpecialType.System_Int32 } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                { SpecialType: SpecialType.System_UInt32 } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UIntKeyword)),
                { SpecialType: SpecialType.System_Int64 } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword)),
                { SpecialType: SpecialType.System_UInt64 } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ULongKeyword)),

                // Floating-point
                { SpecialType: SpecialType.System_Decimal } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DecimalKeyword)),
                { SpecialType: SpecialType.System_Single } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword)),
                { SpecialType: SpecialType.System_Double } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword)),

                // String
                { SpecialType: SpecialType.System_String, NullableAnnotation: NullableAnnotation.Annotated } => SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))),
                { SpecialType: SpecialType.System_String } => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),

                // Nullable
                INamedTypeSymbol { NullableAnnotation: NullableAnnotation.Annotated, TypeArguments: var typeArguments } when TryGetSingle(typeArguments, out var typeArgument) => SyntaxFactory.NullableType(typeArgument!.ToType()),
                INamedTypeSymbol { SpecialType: SpecialType.System_Nullable_T, TypeArguments: var typeArguments } when TryGetSingle(typeArguments, out var typeArgument) => SyntaxFactory.NullableType(typeArgument!.ToType()),

                // Array
                IArrayTypeSymbol { NullableAnnotation: NullableAnnotation.Annotated, ElementType: { } elementType } => SyntaxFactory.NullableType(GetArray(elementType)),
                IArrayTypeSymbol { ElementType: { } elementType } => GetArray(elementType),

                // Pointers
                IPointerTypeSymbol { PointedAtType: { } pointedAtType } => SyntaxFactory.PointerType(pointedAtType.ToType()),

                // Non-special
                INamedTypeSymbol namedTypeSymbol => GetFullName(namedTypeSymbol),

                // anything not supported yet
                _ => throw new NotSupportedException(),
            };

            static bool TryGetSingle<T>(IEnumerable<T> source, out T? value)
            {
                using var enumerator = source.GetEnumerator();
                if (!enumerator.MoveNext())
                {
                    value = default;
                    return false;
                }

                value = enumerator.Current;
                return !enumerator.MoveNext();
            }

            static Syntax.ArrayTypeSyntax GetArray(ITypeSymbol elementType)
            {
                return SyntaxFactory.ArrayType(elementType.ToType())
                    .WithRankSpecifiers(
                        SyntaxFactory.SingletonList(
                            SyntaxFactory.ArrayRankSpecifier(
                                SyntaxFactory.SingletonSeparatedList<Syntax.ExpressionSyntax>(
                                    SyntaxFactory.OmittedArraySizeExpression()))));
            }

            static Syntax.TypeSyntax GetFullName(INamedTypeSymbol type)
            {
                var @namespace = GetNamespace(type.ContainingNamespace);

                // is this generic?
                return type.IsGenericType
                    ? SyntaxFactory.QualifiedName(@namespace, SyntaxFactory.GenericName(type.Name).WithTypeArgumentList(SyntaxFactory.TypeArgumentList(GetTypeArguments(type))))
                    : SyntaxFactory.QualifiedName(@namespace, SyntaxFactory.IdentifierName(type.Name));

                static Syntax.NameSyntax GetNamespace(INamespaceSymbol? namespaceSymbol)
                {
                    return SyntaxFactory.QualifiedName(
                        GetNamespaceParts(namespaceSymbol)
                            .Reverse()
                            .Select(SyntaxFactory.IdentifierName));

                    static IEnumerable<string> GetNamespaceParts(INamespaceSymbol? namespaceSymbol)
                    {
                        while (namespaceSymbol is { Name: { Length: > 0 } name })
                        {
                            yield return name;
                            namespaceSymbol = namespaceSymbol.ContainingNamespace;
                        }
                    }
                }

                static SeparatedSyntaxList<Syntax.TypeSyntax> GetTypeArguments(INamedTypeSymbol type)
                {
                    var typeArguments = type.TypeArguments.GetEnumerator();

                    if (!typeArguments.MoveNext())
                    {
                        throw new InvalidOperationException();
                    }

                    var first = typeArguments.Current;

                    if (!typeArguments.MoveNext())
                    {
                        return SyntaxFactory.SingletonSeparatedList(first.ToType());
                    }

                    ICollection<Syntax.TypeSyntax> arguments = [first.ToType(), typeArguments.Current.ToType()];

                    while (typeArguments.MoveNext())
                    {
                        arguments.Add(typeArguments.Current.ToType());
                    }

                    return SyntaxFactory.SeparatedList(arguments);
                }
            }
        }
    }

    /// <content>
    /// The <see cref="IMethodSymbol"/> extensions.
    /// </content>
    /// <param name="method">The method symbol.</param>
    extension(IMethodSymbol method)
    {
        /// <summary>
        /// Gets the parameter list.
        /// </summary>
        /// <returns>The parameter list.</returns>
        public Syntax.ParameterListSyntax GetParameterList() =>
            SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(
                    method.Parameters.Select(parameter =>
                        SyntaxFactory.Parameter(
                                SyntaxFactory.Identifier(parameter.Name))
                            .WithType(parameter.Type.ToType()))));

        /// <summary>
        /// Gets the object creation.
        /// </summary>
        /// <returns>The object creation list.</returns>
        public Syntax.ObjectCreationExpressionSyntax GetObjectCreation()
        {
            if (method is { MethodKind: MethodKind.Constructor, ReceiverType: { } receiverType })
            {
                return SyntaxFactory.ObjectCreationExpression(
                        SyntaxFactory.QualifiedName(receiverType.ToString()))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList(
                                method.Parameters.Select(static parameter =>
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameter.Name))))));
            }

            throw new InvalidOperationException();
        }
    }
}