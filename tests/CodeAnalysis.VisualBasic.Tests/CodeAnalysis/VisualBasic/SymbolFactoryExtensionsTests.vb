Imports System.Diagnostics.CodeAnalysis
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace CodeAnalysis.VisualBasic

    Public Class SymbolFactoryExtensionsTests
        <Test>
        Public Shared Async Function QualifiedNameFromNames() As Task
            Dim first = SyntaxFactory.IdentifierName("First")
            Dim second = SyntaxFactory.IdentifierName("Second")
            Dim third = SyntaxFactory.IdentifierName("Third")

            Dim qualified = SyntaxFactory.QualifiedName(
                SyntaxFactory.QualifiedName(
                    first,
                    second),
                third)

            Dim names As New List(Of SimpleNameSyntax) From {first, second, third}

            Dim unused = Await Assert.That(QualifiedName(names)).IsEqualTo(qualified, New NameSynaxComparer())
        End Function

        Private NotInheritable Class NameSynaxComparer : Implements IEqualityComparer(Of NameSyntax)
            Public Shadows Function Equals(x As NameSyntax, y As NameSyntax) As Boolean Implements IEqualityComparer(Of NameSyntax).Equals
                Return If(x Is Nothing,
                    y Is Nothing,
                    y IsNot Nothing AndAlso x.GetType() = y.GetType() AndAlso x.GetText().Lines _
                        .Zip(y.GetText().Lines) _
                        .All(Function(z) z.First.Span.Equals(z.Second.Span)))
            End Function

            Public Shadows Function GetHashCode(<DisallowNull> obj As NameSyntax) As Integer Implements IEqualityComparer(Of NameSyntax).GetHashCode
                Throw New NotSupportedException()
            End Function
        End Class
    End Class
End Namespace