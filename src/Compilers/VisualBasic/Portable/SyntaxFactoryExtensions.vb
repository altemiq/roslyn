Public Module SyntaxFactoryExtensions
    ''' <summary>
    ''' Creates a qualified <see cref="Syntax.NameSyntax"/> node.
    ''' </summary>
    ''' <param name="fullName">The full name.</param>
    ''' <returns><see cref="Syntax.NameSyntax"/>.</returns>
    public Function QualifiedName(fullName As String) As Syntax.NameSyntax
        Return QualifiedName(fullName.Split("."C).Select(Function(n) SyntaxFactory.IdentifierName(n)))
    End Function

    ''' <summary>
    ''' Creates a qualified <see cref="Syntax.NameSyntax"/> node.
    ''' </summary>
    ''' <param name="names">The name parts.</param>
    ''' <returns><see cref="Syntax.NameSyntax"/>.</returns>
    public Function QualifiedName(names As IEnumerable(Of Syntax.SimpleNameSyntax)) As Syntax.NameSyntax
        Return Syntax.SyntaxExtensions.ToQualifiedName(names)
    End Function

    ''' <summary>
    ''' Creates a <see cref="Syntax.TypeSyntax"/> from the <see cref="ITypeSymbol"/>.
    ''' </summary>
    ''' <param name="symbol">The type symbol.</param>
    ''' <returns><see cref="Syntax.TypeSyntax"/>.</returns>
    public Function Type(symbol As ITypeSymbol) As Syntax.TypeSyntax
        Return symbol.ToType
    End Function
End Module