' -----------------------------------------------------------------------
' <copyright file="SyntaxFactsExtensions.vb" company="Altemiq">
' Copyright (c) Altemiq. All rights reserved.
' </copyright>
' -----------------------------------------------------------------------

Public Module SyntaxFactsExtensions
    ''' <summary>
    ''' Escapes the text if it is a keyword.
    ''' </summary>
    ''' <param name="text">The text to test.</param>
    ''' <returns>
    ''' The escaped text if <paramref name="text"/> represents a keyword; otherwise <paramref name="text"/>.
    ''' </returns>
    Public Function EscapeKeyword(text As String) As String
        If String.IsNullOrWhiteSpace(text) Then
            Return text
        End If

        ' Check if it's a reserved keyword or a contextual keyword
        If SyntaxFacts.GetKeywordKind(text) <> SyntaxKind.None OrElse SyntaxFacts.GetContextualKeywordKind(text) <> SyntaxKind.None Then
            Return $"[{text}]"
        End If

        Return text
    End Function
End Module