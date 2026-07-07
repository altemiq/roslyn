namespace Altemiq.CodeAnalysis.CSharp;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        await Assert.That(SyntaxFactory.QualifiedName(names)).IsEqualTo(qualified, new NameSynaxComparer());
    }

    private sealed class NameSynaxComparer : IEqualityComparer<NameSyntax>
    {
        public bool Equals(NameSyntax? x, NameSyntax? y)
        {
            if (x is null)
            {
                return y is null;
            }

            if (y is null)
            {
                return false;
            }
            
            if (x.GetType() == y.GetType())
            {
                return x.GetText().Lines
                    .Zip(y.GetText().Lines)
                    .All(_ => _.First.Span.Equals(_.Second.Span));
            }

            return false;


        }

        public int GetHashCode([System.Diagnostics.CodeAnalysis.DisallowNull] NameSyntax obj)
        {
            throw new NotImplementedException();
        }
    }
}
