Imports System.Runtime.CompilerServices
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax


''' <content>
''' The <see cref="ITypeSymbol"/> extensions
''' </content>
Public Module SymbolExtensions
    ''' <summary>
    ''' Gets the type syntax from the type symbol.
    ''' </summary>
    ''' <returns>The type syntax.</returns>
    <Extension>
    Public Function ToType(type as ITypeSymbol) As TypeSyntax
        If type.SpecialType = SpecialType.System_Object Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword))
        End If

        ' Bytes
        If type.SpecialType = SpecialType.System_Boolean Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BooleanKeyword))
        End If
        If type.SpecialType = SpecialType.System_Char Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.CharKeyword))
        End If
        If type.SpecialType = SpecialType.System_SByte Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.SByteKeyword))
        End If
        If type.SpecialType = SpecialType.System_Byte Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword))
        End If

        ' Integer
        If type.SpecialType = SpecialType.System_Int16 Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ShortKeyword))
        End If
        If type.SpecialType = SpecialType.System_UInt16 Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword))
        End If
        If type.SpecialType = SpecialType.System_Int32 Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntegerKeyword))
        End If
        If type.SpecialType = SpecialType.System_UInt32 Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UIntegerKeyword))
        End If
        If type.SpecialType = SpecialType.System_Int64 Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword))
        End If
        If type.SpecialType = SpecialType.System_UInt64 Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ULongKeyword))
        End If

        ' Floating-point
        If type.SpecialType = SpecialType.System_Decimal Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DecimalKeyword))
        End If
        If type.SpecialType = SpecialType.System_Single Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.SingleKeyword))
        End If
        If type.SpecialType = SpecialType.System_Double Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword))
        End If

        ' String
        If type.SpecialType = SpecialType.System_String And type.NullableAnnotation = NullableAnnotation.Annotated _
            Then
            Return _
                SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)))
        End If
        If type.SpecialType = SpecialType.System_String Then
            Return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))
        End If

        ' Nullable
        Dim namedTypeSymbol = TryCast(type, INamedTypeSymbol)
        Dim typeArgument As ITypeSymbol = Nothing
        If namedTypeSymbol IsNot Nothing Then
            If namedTypeSymbol.NullableAnnotation = NullableAnnotation.Annotated AndAlso
               TryGetSingle(namedTypeSymbol.TypeArguments, typeArgument) Then
                Return SyntaxFactory.NullableType(typeArgument.ToType)
            End If

            If namedTypeSymbol.SpecialType = SpecialType.System_Nullable_T AndAlso
               TryGetSingle(namedTypeSymbol.TypeArguments, typeArgument) Then
                Return SyntaxFactory.NullableType(typeArgument.ToType)
            End If
        End If

        ' Array
        Dim arrayTypeSymbol = TryCast(type, IArrayTypeSymbol)
        If arrayTypeSymbol IsNot Nothing Then
            If arrayTypeSymbol.NullableAnnotation = NullableAnnotation.Annotated AndAlso
               arrayTypeSymbol.ElementType IsNot Nothing Then
                Return SyntaxFactory.NullableType(GetArray(arrayTypeSymbol.ElementType))
            End If

            If arrayTypeSymbol.ElementType IsNot Nothing Then
                Return GetArray(arrayTypeSymbol.ElementType)
            End If

        End If

        ' Non-special
        If namedTypeSymbol IsNot Nothing Then
            Return GetFullName(namedTypeSymbol)
        End If

        ' anything not supported yet
        Throw New NotSupportedException
    End Function

    Private Function TryGetSingle (Of T)(source As Immutable.ImmutableArray(Of T), ByRef value As T) As Boolean
        Dim enumerator = source.GetEnumerator
        If Not enumerator.MoveNext Then
            Return False
        End If

        value = enumerator.Current
        Return Not enumerator.MoveNext
    End Function

    Private Function GetArray(elementType As ITypeSymbol) As ArrayTypeSyntax
        Return SyntaxFactory.ArrayType(elementType.ToType) _
            .WithRankSpecifiers(
                SyntaxFactory.List(
                    New ArrayRankSpecifierSyntax() {SyntaxFactory.ArrayRankSpecifier}))
    End Function

    Private Function GetFullName(type As INamedTypeSymbol) As TypeSyntax
        Dim namespaceSyntax = GetNamespace(type.ContainingNamespace)

        If type.IsGenericType Then
            Return _
                SyntaxFactory.QualifiedName(
                    namespaceSyntax,
                    SyntaxFactory.GenericName(
                        type.Name,
                        SyntaxFactory.TypeArgumentList(
                            GetTypeArguments(type))))
        End If

        Return SyntaxFactory.QualifiedName(namespaceSyntax, SyntaxFactory.IdentifierName(type.Name))
    End Function

    Private Function GetNamespace(namespaceSymbol As INamespaceSymbol) As NameSyntax
        Return GetNamespaceParts(namespaceSymbol) _
            .Reverse _
            .Select(Function(n) SyntaxFactory.IdentifierName(n)) _
            .ToQualifiedName
    End Function

    Private Iterator Function GetNamespaceParts(namespaceSymbol As INamespaceSymbol) As IEnumerable(Of String)
        While namespaceSymbol IsNot Nothing And namespaceSymbol.Name IsNot Nothing And namespaceSymbol.Name.Length > 0
            Yield namespaceSymbol.Name
            namespaceSymbol = namespaceSymbol.ContainingNamespace
        End While
    End Function

    Private Function GetTypeArguments(type As INamedTypeSymbol) As SeparatedSyntaxList(Of TypeSyntax)
        Dim typeArguments = type.TypeArguments.GetEnumerator

        If Not typeArguments.MoveNext Then
            Throw New InvalidOperationException
        End If

        Dim first = typeArguments.Current

        If Not typeArguments.MoveNext Then
            Return SyntaxFactory.SingletonSeparatedList(first.ToType)
        End If

        Dim arguments As New List(Of TypeSyntax) From { first.ToType, typeArguments.Current.ToType }

        While typeArguments.MoveNext
            arguments.Add(typeArguments.Current.ToType)
        End While

        Return SyntaxFactory.SeparatedList(arguments)
    End Function
End Module