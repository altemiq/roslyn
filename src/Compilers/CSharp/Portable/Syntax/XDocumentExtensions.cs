// -----------------------------------------------------------------------
// <copyright file="XDocumentExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Xml.Linq;

/// <summary>
/// The <see cref="System.Xml.Linq"/> extensions.
/// </summary>
public static class XDocumentExtensions
{
    /// <summary>
    /// Converts the container in to <see cref="XmlNodeSyntax"/> elements.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <returns>The transformed nodes.</returns>
    public static IEnumerable<XmlNodeSyntax> ToXmlNodes(this XContainer container) => container.Nodes().ToXmlNodes();

    /// <summary>
    /// Converts the nodes in to <see cref="XmlNodeSyntax"/> elements.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
    /// <returns>The transformed nodes.</returns>
    public static IEnumerable<XmlNodeSyntax> ToXmlNodes(this IEnumerable<XNode> nodes) => nodes.Select(node => node.ToXmlNode());

    /// <summary>
    /// Converts the document in to <see cref="XmlNodeSyntax"/> elements.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns>The transformed nodes.</returns>
    public static XmlNodeSyntax ToXmlNode(this XDocument document) => document.Root is { } root
        ? root.ToXmlNode()
        : throw new InvalidOperationException();

    /// <summary>
    /// Converts the node into a <see cref="XmlNodeSyntax"/>.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>The transformed node.</returns>
    public static XmlNodeSyntax ToXmlNode(this XNode node)
    {
        return node switch
        {
            // Text
            XText text => SyntaxFactory.XmlText(text.Value),

            // documentation specific attributes
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Parameter } element => SyntaxFactory.XmlParamElement(GetName(element), element.ToXmlNodes().ToArray()),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.ParameterReference } element => SyntaxFactory.XmlParamRefElement(GetName(element)),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Summary } element => SyntaxFactory.XmlSummaryElement(element.ToXmlNodes().ToArray()),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Returns } element => SyntaxFactory.XmlReturnsElement(element.ToXmlNodes().ToArray()),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Remarks } element => SyntaxFactory.XmlRemarksElement(element.ToXmlNodes().ToArray()),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Para } element => SyntaxFactory.XmlParaElement(element.ToXmlNodes().ToArray()),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.See } element => CreateSeeElement(element),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.SeeAlso } element => CreateSeeAlsoElement(element),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Exception } element => SyntaxFactory.XmlExceptionElement(CreateCrefElement(element), element.ToXmlNodes().ToArray()),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Permission } element => SyntaxFactory.XmlPermissionElement(CreateCrefElement(element), element.ToXmlNodes().ToArray()),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Example } element => SyntaxFactory.XmlExampleElement(element.ToXmlNodes().ToArray()),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Value } element => SyntaxFactory.XmlValueElement(element.ToXmlNodes().ToArray()),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Placeholder } element => SyntaxFactory.XmlPlaceholderElement(element.ToXmlNodes().ToArray()),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.ThreadSafety } => SyntaxFactory.XmlThreadSafetyElement(),

            // Generic
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.TypeParameter } element => SyntaxFactory.XmlElement(Roslyn.Utilities.DocumentationCommentXmlNames.Elements.TypeParameter, SyntaxFactory.List(element.ToXmlNodes())).AddStartTagAttributes(SyntaxFactory.XmlNameAttribute(GetName(element))),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.TypeParameterReference } element => SyntaxFactory.XmlElement(Roslyn.Utilities.DocumentationCommentXmlNames.Elements.TypeParameterReference, SyntaxFactory.List(element.ToXmlNodes())).AddStartTagAttributes(SyntaxFactory.XmlNameAttribute(GetName(element))),

            // List
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.List } element => SyntaxFactory.XmlElement(Roslyn.Utilities.DocumentationCommentXmlNames.Elements.List, SyntaxFactory.List(element.ToXmlNodes())).AddStartTagAttributes(SyntaxFactory.XmlTextAttribute(Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Type, GetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Type))),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.ListHeader } element => SyntaxFactory.XmlElement(Roslyn.Utilities.DocumentationCommentXmlNames.Elements.ListHeader, SyntaxFactory.List(element.ToXmlNodes())),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Item } element => SyntaxFactory.XmlElement(Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Item, SyntaxFactory.List(element.ToXmlNodes())),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Term } element => SyntaxFactory.XmlElement(Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Term, SyntaxFactory.List(element.ToXmlNodes())),
            XElement { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Description } element => SyntaxFactory.XmlElement(Roslyn.Utilities.DocumentationCommentXmlNames.Elements.Description, SyntaxFactory.List(element.ToXmlNodes())),

            // Just translate directly
            XElement { Name.LocalName: { } name, HasAttributes: true } element => SyntaxFactory.XmlElement(name, SyntaxFactory.List(element.ToXmlNodes())).AddStartTagAttributes([.. GetAttributes(element)]),
            XElement { Name.LocalName: { } name } element => SyntaxFactory.XmlElement(name, SyntaxFactory.List(element.ToXmlNodes())),

            _ => throw new NotSupportedException(),
        };

        static string GetName(XElement element)
        {
            return GetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Name);
        }

        static string GetAttributeValue(XElement element, string name)
        {
            return element.Attribute(name)!.Value;
        }

        static bool TryGetAttributeValue(XElement element, string name, out string value)
        {
            if (element.Attribute(name) is { } attribute)
            {
                value = attribute.Value;
                return true;
            }

            value = string.Empty;
            return false;
        }

        static IEnumerable<XmlAttributeSyntax> GetAttributes(XElement element)
        {
            return element.Attributes().Select<XAttribute, XmlAttributeSyntax>(attribute => attribute switch
            {
                { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Name, Value: { } value } => SyntaxFactory.XmlNameAttribute(value),
                { Name.LocalName: Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Cref, Value: { } value } => SyntaxFactory.XmlCrefAttribute(GetCrefElement(value)),
                { Name.LocalName: { } name, Value: { } value } => SyntaxFactory.XmlTextAttribute(name, value),
            });
        }

        static XmlEmptyElementSyntax CreateSeeElement(XElement element)
        {
            if (TryGetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Cref, out var cref))
            {
                return SyntaxFactory.XmlSeeElement(GetCrefElement(cref));
            }

            if (TryGetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Langword, out var keyword))
            {
                return SyntaxFactory.XmlEmptyElement(Roslyn.Utilities.DocumentationCommentXmlNames.Elements.See).AddAttributes(
                    SyntaxFactory.XmlTextAttribute(Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Langword, keyword));
            }

            if (TryGetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Href, out var href))
            {
                return SyntaxFactory.XmlEmptyElement(Roslyn.Utilities.DocumentationCommentXmlNames.Elements.See).AddAttributes(
                    SyntaxFactory.XmlTextAttribute(Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Href, href));
            }

            throw new InvalidOperationException();
        }

        static XmlEmptyElementSyntax CreateSeeAlsoElement(XElement element)
        {
            if (TryGetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Cref, out var cref))
            {
                return SyntaxFactory.XmlSeeAlsoElement(GetCrefElement(cref));
            }

            if (TryGetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Href, out var href))
            {
                return SyntaxFactory.XmlEmptyElement(Roslyn.Utilities.DocumentationCommentXmlNames.Elements.See).AddAttributes(
                    SyntaxFactory.XmlTextAttribute(Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Href, href));
            }

            throw new InvalidOperationException();
        }

        static CrefSyntax CreateCrefElement(XElement element)
        {
            return TryGetAttributeValue(element, Roslyn.Utilities.DocumentationCommentXmlNames.Attributes.Cref, out var cref)
                ? GetCrefElement(cref)
                : throw new InvalidOperationException();
        }

        static CrefSyntax GetCrefElement(string cref)
        {
            var colonIndex = cref.IndexOf(':');
            if (colonIndex < 0)
            {
                return SyntaxFactory.NameMemberCref(SyntaxFactory.QualifiedName(cref));
            }

            var type = cref.Substring(0, colonIndex);
            colonIndex++;
            return type switch
            {
                "T" => SyntaxFactory.TypeCref(SyntaxFactory.QualifiedName(cref.Substring(colonIndex))),
                _ => SyntaxFactory.NameMemberCref(SyntaxFactory.QualifiedName(cref.Substring(colonIndex))),
            };
        }
    }
}