namespace Altemiq.CodeAnalysis.CSharp;

using NSubstitute;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public class SymbolExtensionsTests
{
    [Test]
    [Arguments(SpecialType.System_Object)]
    [Arguments(SpecialType.System_Boolean)]
    [Arguments(SpecialType.System_Char)]
    [Arguments(SpecialType.System_SByte)]
    [Arguments(SpecialType.System_Byte)]
    [Arguments(SpecialType.System_Int16)]
    [Arguments(SpecialType.System_UInt16)]
    [Arguments(SpecialType.System_Int32)]
    [Arguments(SpecialType.System_UInt32)]
    [Arguments(SpecialType.System_Int64)]
    [Arguments(SpecialType.System_UInt64)]
    [Arguments(SpecialType.System_Decimal)]
    [Arguments(SpecialType.System_Single)]
    [Arguments(SpecialType.System_Double)]
    public async Task ToType(SpecialType specialType)
    {
        var type = Substitute.For<ITypeSymbol>();
        type.SpecialType.Returns(specialType);

        await Assert.That(type.ToType()).IsNotNull();
    }
}