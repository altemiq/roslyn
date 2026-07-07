Imports Microsoft.CodeAnalysis.VisualBasic

Namespace CodeAnalysis.VisualBasic

    Public Class SyntaxFactsExtensionsTests
        <Test>
        <Arguments("Integer", "[Integer]")>
        <Arguments("Type", "[Type]")>
        <Arguments("Other", "Other")>
        Public Shared Async Function Escape(text As String, expected As String) As Task
            Dim unused = Await Assert.That(EscapeKeyword(text)).IsEqualTo(expected)
        End Function
    End Class
End Namespace