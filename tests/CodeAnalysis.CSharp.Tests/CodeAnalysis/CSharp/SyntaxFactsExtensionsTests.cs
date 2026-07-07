
using Microsoft.CodeAnalysis.CSharp;

namespace Altemiq.CodeAnalysis.CSharp
{
    public class SyntaxFactsExtensionsTests
    {
        [Test]
        [Arguments("int", "@int")]
        [Arguments("var", "@var")]
        [Arguments("other", "other")]
        public async Task Escape(string text, string expected)
        {
            _ = await Assert.That(SyntaxFacts.EscapeKeyword(text)).IsEqualTo(expected);
        }
    }
}