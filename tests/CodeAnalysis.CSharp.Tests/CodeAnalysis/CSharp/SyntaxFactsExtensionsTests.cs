namespace Altemiq.CodeAnalysis.CSharp;

using Microsoft.CodeAnalysis.CSharp;

public class SyntaxFactsExtensionsTests
{
    [Test]
    [Arguments("int", "@int")]
    [Arguments("var", "@var")]
    [Arguments("other", "other")]
    public async Task Escape(string text, string expected)
    {
        await Assert.That(SyntaxFacts.EscapeKeyword(text)).IsEqualTo(expected);
    }
}
