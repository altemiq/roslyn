' -----------------------------------------------------------------------
' <copyright file="TypeExtensions.vb" company="Altemiq">
' Copyright (c) Altemiq. All rights reserved.
' </copyright>
' -----------------------------------------------------------------------

Public Module TypeExtensions
    ''' <summary>
    ''' Converts the type to the <see cref="Syntax.TypeSyntax"/>.
    ''' </summary>
    ''' <param name="parameters">The type parameters.</param>
    ''' <returns>The type syntax.</returns>
    ''' <exception cref="ArgumentOutOfRangeException">The parameters are the wrong length.</exception>
    <Runtime.CompilerServices.Extension>
    Public Function ToTypeSyntax(type As Type, ParamArray parameters() As Syntax.TypeSyntax) As Syntax.NameSyntax
        If Not type.IsGenericType Then
            If type.FullName IsNot Nothing Then
                Return QualifiedName(type.FullName)
            End If

            Throw New InvalidOperationException
        End If

        Dim index = type.Name.IndexOf("`"c)
        Dim count = Integer.Parse(type.Name.Substring(index + 1), Globalization.CultureInfo.InvariantCulture)

        Dim parameterList = SyntaxFactory.SeparatedList(parameters)
        If count <> parameterList.Count Then
            Throw New ArgumentOutOfRangeException(NameOf(parameters))
        End If

        Dim name = type.Name.Substring(0, index)
        Dim genericName = SyntaxFactory.GenericName(SyntaxFactory.Identifier(name), SyntaxFactory.TypeArgumentList(parameterList))
        If type.Namespace IsNot Nothing Then
            Return SyntaxFactory.QualifiedName(QualifiedName(type.Namespace), genericName)
        End If

        Return genericName
    End Function
End Module