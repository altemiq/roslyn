' -----------------------------------------------------------------------
' <copyright file="XDocumentExtensions.vb" company="Altemiq">
' Copyright (c) Altemiq. All rights reserved.
' </copyright>
' -----------------------------------------------------------------------

Imports System.Runtime.CompilerServices

Namespace Syntax
    ''' <summary>
    ''' The <see cref="System.Xml.Linq"/> extensions.
    ''' </summary>
    Public Module XDocumentExtensions
        ''' <summary>
        ''' Converts the container in to <see cref="XmlNodeSyntax"/> elements.
        ''' </summary>
        ''' <param name="container">The container.</param>
        ''' <returns>The transformed nodes.</returns>
        <Extension>
        Public Function ToXmlNodes(container As XContainer) As IEnumerable(Of XmlNodeSyntax)
            Return container.Nodes.ToXmlNodes
        End Function

        ''' <summary>
        ''' Converts the nodes in to <see cref="XmlNodeSyntax"/> elements.
        ''' </summary>
        ''' <param name="nodes">The nodes.</param>
        ''' <returns>The transformed nodes.</returns>
        <Extension>
        Public Function ToXmlNodes(nodes As IEnumerable(Of XNode)) As IEnumerable(Of XmlNodeSyntax)
            Return nodes.Select(Function(node) node.ToXmlNode)
        End Function

        ''' <summary>
        ''' Converts the document in to <see cref="XmlNodeSyntax"/> elements.
        ''' </summary>
        ''' <param name="document">The document.</param>
        ''' <returns>The transformed nodes.</returns>
        <Extension>
        Public Function ToXmlNode(document As XDocument) As XmlNodeSyntax
            If document.Root IsNot Nothing Then
                Return document.Root.ToXmlNode
            End If

            Throw New InvalidOperationException
        End Function

        ''' <summary>
        ''' Converts the node into a <see cref="XmlNodeSyntax"/>.
        ''' </summary>
        ''' <param name="node">The node.</param>
        ''' <returns>The transformed node.</returns>
        <Extension>
        Public Function ToXmlNode(node As XNode) As XmlNodeSyntax
            Dim text = TryCast(node, XText)
            If text IsNot Nothing Then
                Return SyntaxFactory.XmlText(text.Value)
            End If

            Dim element = TryCast(node, XElement)
            If element IsNot Nothing Then
                Dim name As String = element.Name.LocalName

                ' documentation specific attributes
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Parameter Then
                    Return SyntaxFactory.XmlParamElement(GetName(element), element.ToXmlNodes.ToArray)
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.ParameterReference Then
                    Return SyntaxFactory.XmlParamRefElement(GetName(element))
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Summary Then
                    Return SyntaxFactory.XmlSummaryElement(element.ToXmlNodes.ToArray)
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Returns Then
                    Return SyntaxFactory.XmlReturnsElement(element.ToXmlNodes.ToArray)
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Remarks Then
                    Return SyntaxFactory.XmlRemarksElement(element.ToXmlNodes.ToArray)
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Para Then
                    Return SyntaxFactory.XmlParaElement(element.ToXmlNodes.ToArray)
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.See Then
                    Return CreateSeeElement(element)
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.SeeAlso Then
                    Return CreateSeeAlsoElement(element)
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Exception Then
                    Return SyntaxFactory.XmlExceptionElement(CreateCrefElement(element), element.ToXmlNodes.ToArray)
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Permission Then
                    Return SyntaxFactory.XmlPermissionElement(CreateCrefElement(element), element.ToXmlNodes.ToArray)
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Example Then
                    Return SyntaxFactory.XmlExampleElement(element.ToXmlNodes.ToArray)
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Value Then
                    Return SyntaxFactory.XmlValueElement(element.ToXmlNodes.ToArray)
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Placeholder Then
                    Return SyntaxFactory.XmlPlaceholderElement(element.ToXmlNodes.ToArray)
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.ThreadSafety Then
                    Return SyntaxFactory.XmlThreadSafetyElement
                End If

                ' Generic
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.TypeParameter Then
                    Return SyntaxFactory.XmlElement(
                        Roslyn.Utilities.DocumentationCommentXmlNames.Elements.TypeParameter,
                        SyntaxFactory.List(element.ToXmlNodes)).AddStartTagAttributes(SyntaxFactory.XmlNameAttribute(GetName(element)))
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.TypeParameterReference Then
                    Return SyntaxFactory.XmlElement(
                        Roslyn.Utilities.DocumentationCommentXmlNames.Elements.TypeParameterReference,
                        SyntaxFactory.List(element.ToXmlNodes)).AddStartTagAttributes(SyntaxFactory.XmlNameAttribute(GetName(element)))
                End If

                ' List
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.List Then
                    Return SyntaxFactory.XmlElement(
                        Roslyn.Utilities.DocumentationCommentXmlNames.Elements.List,
                        SyntaxFactory.List(element.ToXmlNodes)).AddStartTagAttributes(
                            CreateAttribute(
                                Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Type,
                                GetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Type)))
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.ListHeader Then
                    Return SyntaxFactory.XmlElement(
                        Roslyn.Utilities.DocumentationCommentXmlNames.Elements.ListHeader,
                        SyntaxFactory.List(element.ToXmlNodes))
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Item Then
                    Return SyntaxFactory.XmlElement(
                        Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Item,
                        SyntaxFactory.List(element.ToXmlNodes))
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Term Then
                    Return SyntaxFactory.XmlElement(
                        Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Term,
                        SyntaxFactory.List(element.ToXmlNodes))
                End If
                If name = Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Description Then
                    Return SyntaxFactory.XmlElement(
                        Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Description,
                        SyntaxFactory.List(element.ToXmlNodes))
                End If

                ' Just translate directly
                If element.HasAttributes Then
                    Return SyntaxFactory.XmlElement(name, SyntaxFactory.List(element.ToXmlNodes)).AddStartTagAttributes(
                        GetAttributes(element).ToArray())
                End If

                Return SyntaxFactory.XmlElement(name, SyntaxFactory.List(element.ToXmlNodes))
            End If

            Throw New NotSupportedException
        End Function

        Private Function GetName(element As XElement) As String
            Return GetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Name)
        End Function

        Private Function GetAttributeValue(element As XElement, name As String) As String
            Return element.Attribute(name).Value
        End Function

        Private Function TryGetAttributeValue(element As XElement, name As String, ByRef value As String) As Boolean
            Dim attribute = element.Attribute(name)
            If attribute IsNot Nothing Then
                value = attribute.Value
                Return True
            End If

            value = String.Empty
            Return False
        End Function

        Private Function GetAttributes(element As XElement) As IEnumerable(Of BaseXmlAttributeSyntax)
            Return element.Attributes.Select(Function(attribute As XAttribute) As BaseXmlAttributeSyntax
                                                 If attribute.Value IsNot Nothing Then
                                                     If attribute.Name.LocalName = Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Name Then
                                                         Return SyntaxFactory.XmlNameAttribute(attribute.Value)
                                                     End If

                                                     If attribute.Name.LocalName = Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Cref Then
                                                         Return SyntaxFactory.XmlCrefAttribute(GetCrefElement(attribute.Value))
                                                     End If

                                                     Return CreateAttribute(
                                                         attribute.Name.LocalName,
                                                         attribute.Value)
                                                 End If

                                                 Throw New InvalidOperationException
                                             End Function)
        End Function

        Private Function CreateAttribute(name As String, value As String) As XmlAttributeSyntax
            Return SyntaxFactory.XmlAttribute(
                SyntaxFactory.XmlName(
                    Nothing,
                    SyntaxFactory.XmlNameToken(name, SyntaxKind.XmlNameToken)),
                SyntaxFactory.XmlString(
                    SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken),
                    SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(value)),
                    SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken)).AddTextTokens()).
                WithLeadingTrivia(SyntaxFactory.Whitespace(" "))
        End Function

        Private Function CreateSeeElement(element As XElement) As XmlEmptyElementSyntax
            Dim value As String = Nothing
            If TryGetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Cref, value) Then
                Return SyntaxFactory.XmlSeeElement(GetCrefElement(value))
            End If

            If TryGetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Langword, value) Then
                Return SyntaxFactory.XmlEmptyElement(Roslyn.Utilities.DocumentationCommentXmlNames.Elements.See).AddAttributes(
                    CreateAttribute(Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Langword, GetLangword(value)))
            End If

            If TryGetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Href, value) Then
                Return SyntaxFactory.XmlEmptyElement(Roslyn.Utilities.DocumentationCommentXmlNames.Elements.See).AddAttributes(
                    CreateAttribute(Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Href, value))
            End If

            Throw New InvalidOperationException
        End Function

        Private Function CreateSeeAlsoElement(element As XElement) As XmlEmptyElementSyntax
            Dim value As String = Nothing
            If TryGetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Cref, value) Then
                Return SyntaxFactory.XmlSeeAlsoElement(GetCrefElement(value))
            End If

            If TryGetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Href, value) Then
                Return SyntaxFactory.XmlEmptyElement(Roslyn.Utilities.DocumentationCommentXmlNames.Elements.See).AddAttributes(
                    CreateAttribute(
                        Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Href,
                        value))
            End If

            Throw New InvalidOperationException
        End Function

        Private Function CreateCrefElement(element As XElement) As CrefReferenceSyntax
            Dim value As String = Nothing
            If TryGetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Cref, value) Then
                Return GetCrefElement(value)
            End If
            Throw New InvalidOperationException
        End Function

        Private Function GetCrefElement(cref As String) As CrefReferenceSyntax
            Dim colonIndex = cref.IndexOf(":"c)
            If colonIndex < 0 Then
                Return SyntaxFactory.CrefReference(QualifiedName(cref))
            End If

            colonIndex += 1
            Return SyntaxFactory.CrefReference(QualifiedName(cref.Substring(colonIndex)))
        End Function

        Private Function GetLangword(value As String) As String
            If value = "null" Then
                Return "Nothing"
            End If

            If value = "true" Then
                Return Boolean.TrueString
            End If

            If value = "false" Then
                Return Boolean.FalseString
            End If

            If value = "static" Then
                Return "Shared"
            End If

            If value = "this" Then
                Return "Me"
            End If

            If value = "base" Then
                Return "MyBase"
            End If

            If value = "overload" Then
                Return "Overloads"
            End If

            If value = "override" Then
                Return "Overrides"
            End If

            Return value
        End Function

        <Extension>
        Private Function AddStartTagAttributes(element As XmlElementSyntax, ParamArray items() As BaseXmlAttributeSyntax) As XmlElementSyntax
            Return element.WithStartTag(element.StartTag.WithAttributes(element.StartTag.Attributes.AddRange(items)))
        End Function
    End Module
End Namespace