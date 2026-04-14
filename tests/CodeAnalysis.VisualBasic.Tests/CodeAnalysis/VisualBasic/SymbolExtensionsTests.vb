Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports NSubstitute

Namespace CodeAnalysis.VisualBasic

Public Class SymbolExtensionsTests
    <Test>
    <Arguments(SpecialType.System_Object)>
    <Arguments(SpecialType.System_Boolean)>
    <Arguments(SpecialType.System_Char)>
    <Arguments(SpecialType.System_SByte)>
    <Arguments(SpecialType.System_Byte)>
    <Arguments(SpecialType.System_Int16)>
    <Arguments(SpecialType.System_UInt16)>
    <Arguments(SpecialType.System_Int32)>
    <Arguments(SpecialType.System_UInt32)>
    <Arguments(SpecialType.System_Int64)>
    <Arguments(SpecialType.System_UInt64)>
    <Arguments(SpecialType.System_Decimal)>
    <Arguments(SpecialType.System_Single)>
    <Arguments(SpecialType.System_Double)>
    Public Async Function ToType(specialType As SpecialType) As Task
        Dim typeSymbolSubstitute = Substitute.For(Of ITypeSymbol)
        typeSymbolSubstitute.SpecialType.Returns(specialType)
        
        Await Assert.That(typeSymbolSubstitute.ToType()).IsNotNull()
    End Function
End Class
End Namespace
