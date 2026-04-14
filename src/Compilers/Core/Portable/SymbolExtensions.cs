// -----------------------------------------------------------------------
// <copyright file="SymbolExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130, CheckNamespace
namespace Microsoft.CodeAnalysis;
#pragma warning restore IDE0130, CheckNamespace

#pragma warning disable RCS1263, SA1101

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
        /// Gets a value indicating whether this instance is a primitive, or a nullable primitive.
        /// </summary>
        public bool IsPrimitiveOrNullablePrimitive
        {
            get
            {
                return IsPrimitive(type)
                       || IsEnum(type)
                       || IsNullableAnnotated(type)
                       || IsSystemNullable(type);

                static bool IsPrimitive(ITypeSymbol typeSymbol)
                {
                    return typeSymbol.SpecialType is
                        SpecialType.System_Boolean or
                        SpecialType.System_Char or
                        SpecialType.System_SByte or
                        SpecialType.System_Byte or
                        SpecialType.System_Int16 or
                        SpecialType.System_UInt16 or
                        SpecialType.System_Int32 or
                        SpecialType.System_UInt32 or
                        SpecialType.System_Int64 or
                        SpecialType.System_UInt64 or
                        SpecialType.System_Single or
                        SpecialType.System_Double;
                }

                static bool IsEnum(ITypeSymbol typeSymbol)
                {
                    return typeSymbol is { SpecialType: SpecialType.System_Enum } or { TypeKind: TypeKind.Enum };
                }

                static bool IsNullableAnnotated(ITypeSymbol typeSymbol)
                {
                    return typeSymbol is INamedTypeSymbol { NullableAnnotation: NullableAnnotation.Annotated, IsValueType: true, TypeArguments: var typeArguments }
                           && TryGetSingle(typeArguments, out var typeArgument)
                           && typeArgument!.IsPrimitiveOrNullablePrimitive;
                }

                static bool IsSystemNullable(ITypeSymbol typeSymbol)
                {
                    return typeSymbol is INamedTypeSymbol { SpecialType: SpecialType.System_Nullable_T, TypeArguments: var typeArguments }
                           && TryGetSingle(typeArguments, out var typeArgument)
                           && typeArgument!.IsPrimitiveOrNullablePrimitive;
                }

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
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a collection.
        /// </summary>
        public bool IsCollection(ITypeSymbol? collectionType) => type switch
        {
            { SpecialType: SpecialType.System_Collections_Generic_ICollection_T } => true,
            { OriginalDefinition: { } originalDefinition } when SymbolEqualityComparer.Default.Equals(originalDefinition, collectionType) => true,
            { AllInterfaces: { Length: not 0 } interfaces } when interfaces.Select(i => i.OriginalDefinition).Contains(collectionType, SymbolEqualityComparer.Default) => true,
            _ => false,
        };

        /// <summary>
        /// Gets a value indicating whether this instance is a dictionary.
        /// </summary>
        public bool IsDictionary(ITypeSymbol? dictionaryType) => type switch
        {
            { OriginalDefinition: { } originalDefinition } when SymbolEqualityComparer.Default.Equals(originalDefinition, dictionaryType) => true,
            { AllInterfaces: { Length: not 0 } interfaces } when interfaces.Select(i => i.OriginalDefinition).Contains(dictionaryType, SymbolEqualityComparer.Default) => true,
            _ => false,
        };
    }
}