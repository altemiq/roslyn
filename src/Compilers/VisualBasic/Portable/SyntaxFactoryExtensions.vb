' -----------------------------------------------------------------------
' <copyright file="SyntaxFactoryExtensions.vb" company="Altemiq">
' Copyright (c) Altemiq. All rights reserved.
' </copyright>
' -----------------------------------------------------------------------
Public Module SyntaxFactoryExtensions
    ''' <summary>
    ''' Creates a qualified <see cref="Syntax.NameSyntax"/> node.
    ''' </summary>
    ''' <param name="fullName">The full name.</param>
    ''' <returns><see cref="Syntax.NameSyntax"/>.</returns>
    Public Function QualifiedName(fullName As String) As Syntax.NameSyntax
        Return QualifiedName(fullName.Split("."c).Select(Function(n) SyntaxFactory.IdentifierName(n)))
    End Function

    ''' <summary>
    ''' Creates a qualified <see cref="Syntax.NameSyntax"/> node.
    ''' </summary>
    ''' <param name="names">The name parts.</param>
    ''' <returns><see cref="Syntax.NameSyntax"/>.</returns>
    Public Function QualifiedName(names As IEnumerable(Of Syntax.SimpleNameSyntax)) As Syntax.NameSyntax
        Return Syntax.SyntaxExtensions.ToQualifiedName(names)
    End Function

    ''' <summary>
    ''' Creates a <see cref="Syntax.TypeSyntax"/> from the <see cref="ITypeSymbol"/>.
    ''' </summary>
    ''' <param name="symbol">The type symbol.</param>
    ''' <returns><see cref="Syntax.TypeSyntax"/>.</returns>
    Public Function Type(symbol As ITypeSymbol) As Syntax.TypeSyntax
        Return symbol.ToType
    End Function
End Module