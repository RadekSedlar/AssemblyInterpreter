using AssemblyInterpret.Scanners.TextSection;

namespace Tests;

public class TextSectionScannerTests
{
    [Fact]
    public void Empty_Should_Return_Eof_Token()
    {
        var sut = new TextSectionScanner("");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.Eof});
    }
    
    [Fact]
    public void Comma_Should_Return_Separator_Token()
    {
        var sut = new TextSectionScanner(" , ");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.Separator});
    }
    
    [Fact]
    public void OpenSquareBracket_Should_Return_SqBracketOpen_Token()
    {
        var sut = new TextSectionScanner(" [ ");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.SqBracketOpen});
    }
    
    [Fact]
    public void CloseSquareBracket_Should_Return_SqBracketClose_Token()
    {
        var sut = new TextSectionScanner(" ] ");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.SqBracketClose});
    }
    
    [Fact]
    public void BYTE_Should_Return_DataTypeByte_Token()
    {
        var sut = new TextSectionScanner(" BYTE ");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.DataTypeByte});
    }
    
    [Fact]
    public void WORD_Should_Return_DataTypeWord_Token()
    {
        var sut = new TextSectionScanner("WORD ");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.DataTypeWord});
    }
    
    [Fact]
    public void DWORD_Should_Return_DataTypeDouble_Token()
    {
        var sut = new TextSectionScanner("DWORD ");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.DataTypeDouble});
    }
    
    [Fact]
    public void Single_Digit_Should_Return_Number_Token()
    {
        var sut = new TextSectionScanner("3");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.Number});
    }
    
    [Fact]
    public void Multiple_Digits_Should_Return_Number_Token()
    {
        var sut = new TextSectionScanner(" 666 ");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.Number});
    }
    
    [Fact]
    public void Single_Alphabetical_Character_Should_Return_Word_Token()
    {
        var sut = new TextSectionScanner("a ");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.Word});
    }
    
    [Fact]
    public void Alphabetical_Characters_Should_Return_Word_Token()
    {
        var sut = new TextSectionScanner(" drip ");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.Word});
    }
    
    [Fact]
    public void Alphanumerical_Characters_Should_Return_Word_Token()
    {
        var sut = new TextSectionScanner(" sdllll69x ");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.Word});
    }
    
    [Fact]
    public void Plus_Sign_Should_Return_Plus_Token()
    {
        var sut = new TextSectionScanner(" + ");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.Plus});
    }
    
    [Fact]
    public void Minus_Sign_Should_Return_Minus_Token()
    {
        var sut = new TextSectionScanner(" -");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.Minus});
    }
    
    [Fact]
    public void Times_Sign_Should_Return_Times_Token()
    {
        var sut = new TextSectionScanner("*");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.Times});
    }
    
    [Fact]
    public void Label_Should_Return_Label_Token()
    {
        var sut = new TextSectionScanner(".labelName");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.Label, Lexeme: ".labelName"});
    }
    
    [Fact]
    public void Series_Of_All_Registers_Should_Return_All_Registers_Token()
    {
        var sut = new TextSectionScanner("eax ebx ecx edx  ax bx cx dx  ah al bh bl ch cl dh dl ");
        sut.Scan();

        var tokenEax = sut.GetToken();
        var tokenEbx = sut.GetToken();
        var tokenEcx = sut.GetToken();
        var tokenEdx = sut.GetToken();
        var tokenAx = sut.GetToken();
        var tokenBx = sut.GetToken();
        var tokenCx = sut.GetToken();
        var tokenDx = sut.GetToken();
        var tokenAh = sut.GetToken();
        var tokenAl = sut.GetToken();
        var tokenBh = sut.GetToken();
        var tokenBl = sut.GetToken();
        var tokenCh = sut.GetToken();
        var tokenCl = sut.GetToken();
        var tokenDh = sut.GetToken();
        var tokenDl = sut.GetToken();
        
        Assert.True(tokenEax is {Type: TextSectionTokenType.Register});
        Assert.True(tokenEbx is {Type: TextSectionTokenType.Register});
        Assert.True(tokenEcx is {Type: TextSectionTokenType.Register});
        Assert.True(tokenEdx is {Type: TextSectionTokenType.Register});
        Assert.True(tokenAx is {Type: TextSectionTokenType.Register});
        Assert.True(tokenBx is {Type: TextSectionTokenType.Register});
        Assert.True(tokenCx is {Type: TextSectionTokenType.Register});
        Assert.True(tokenDx is {Type: TextSectionTokenType.Register});
        Assert.True(tokenAh is {Type: TextSectionTokenType.Register});
        Assert.True(tokenAl is {Type: TextSectionTokenType.Register});
        Assert.True(tokenBh is {Type: TextSectionTokenType.Register});
        Assert.True(tokenBl is {Type: TextSectionTokenType.Register});
        Assert.True(tokenCh is {Type: TextSectionTokenType.Register});
        Assert.True(tokenCl is {Type: TextSectionTokenType.Register});
        Assert.True(tokenDh is {Type: TextSectionTokenType.Register});
        Assert.True(tokenDl is {Type: TextSectionTokenType.Register});
    }
    
    [Fact]
    public void RN_Should_Return_NewLine_Token()
    {
        var sut = new TextSectionScanner("\r\n");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: TextSectionTokenType.NewLine});
    }
}