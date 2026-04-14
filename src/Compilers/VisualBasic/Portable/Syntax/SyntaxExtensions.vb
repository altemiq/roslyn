' -----------------------------------------------------------------------
' <copyright file="SyntaxExtensions.vb" company="Altemiq">
' Copyright (c) Altemiq. All rights reserved.
' </copyright>
' -----------------------------------------------------------------------

Namespace Syntax
    ''' <summary>
    ''' The syntax extensions.
    ''' </summary>
    Public Module SyntaxExtensions
        ''' <summary>
        ''' Converts the simple names to a qualified name.
        ''' </summary>
        ''' <param name="names">The names to qualify.</param>
        ''' <returns>The qualified name.</returns>
        ''' <exception cref="InvalidOperationException">Could not generate the name.</exception>
        <Runtime.CompilerServices.Extension>
        Public Function ToQualifiedName(names As IEnumerable(Of SimpleNameSyntax)) As NameSyntax
            Dim enumerator = names.GetEnumerator
            enumerator.MoveNext()

            Dim name As NameSyntax = enumerator.Current
            While enumerator.MoveNext AndAlso name IsNot Nothing AndAlso enumerator.Current IsNot Nothing
                Dim nextName = DirectCast(SyntaxFactory.QualifiedName(name, enumerator.Current), NameSyntax)
                name = nextName
            End While

            enumerator.Dispose()

            If name IsNot Nothing Then
                Return name
            End If

            Throw New InvalidOperationException
        End Function
    End Module
End Namespace