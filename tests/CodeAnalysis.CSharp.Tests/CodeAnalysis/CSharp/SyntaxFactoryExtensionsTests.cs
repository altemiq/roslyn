
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Altemiq.CodeAnalysis.CSharp
{
    public class SyntaxFactoryExtensionsTests
    {
        [Test]
        public async Task QualifiedNameFromNames()
        {
            var first = SyntaxFactory.IdentifierName("First");
            var second = SyntaxFactory.IdentifierName("Second");
            var third = SyntaxFactory.IdentifierName("Third");

            var qualified = SyntaxFactory.QualifiedName(
                SyntaxFactory.QualifiedName(
                    first,
                    second),
                third);

            IEnumerable<SimpleNameSyntax> names = [first, second, third];

            _ = await Assert.That(SyntaxFactory.QualifiedName(names)).IsEqualTo(qualified, new NameSynaxComparer());
        }

        private sealed class NameSynaxComparer : IEqualityComparer<NameSyntax>
        {
            public bool Equals(NameSyntax? x, NameSyntax? y)
            {
                return (x, y) switch
                {
                    (null, null) => true,
                    (not null, not null) => x.GetType() == y.GetType()
                        && x.GetText().Lines
                            .Zip(y.GetText().Lines)
                            .All(static _ => _.First.Span.Equals(_.Second.Span)),
                    _ => false,
                };
            }

            public int GetHashCode([System.Diagnostics.CodeAnalysis.DisallowNull] NameSyntax obj)
            {
                throw new NotSupportedException();
            }
        }
    }
}