using AssemblyInterpret.Scanners.DataSection;

namespace Tests;

public class DataSectionScannerTests
{
    
    [Fact]
    public void Empty_Should_Return_Eof_Token()
    {
        var sut = new DataSectionScanner("");
        sut.Scan();

        var token = sut.GetToken();
        
        Assert.True(token is {Type: DataSectionTokenType.Eof});
    }
    
    [Fact]
    public void NewLine_Should_Return_NewLine_Token()
    {
        var sut = new DataSectionScanner("\r\n");
        sut.Scan();

        var firstToken = sut.GetToken();
        var secondToken = sut.GetToken();
        
        Assert.True(firstToken is {Type: DataSectionTokenType.Newline});
        Assert.True(secondToken is {Type: DataSectionTokenType.Eof});
    }
    
    [Fact]
    public void Word_Should_Return_Word_Token()
    {
        var sut = new DataSectionScanner("neco");
        sut.Scan();

        var firstToken = sut.GetToken();
        var secondToken = sut.GetToken();
        
        Assert.True(firstToken is {Type: DataSectionTokenType.Word});
        Assert.True(secondToken is {Type: DataSectionTokenType.Eof});
    }
    
    [Fact]
    public void DB_Should_ReturnKeyword_DataByte_Token()
    {
        var sut = new DataSectionScanner("DB");
        sut.Scan();

        var firstToken = sut.GetToken();
        var secondToken = sut.GetToken();
        
        Assert.True(firstToken is {Type: DataSectionTokenType.KeywordDataByte});
        Assert.True(secondToken is {Type: DataSectionTokenType.Eof});
    }
    
    [Fact]
    public void DB_Should_Return_KeywordDataWord_Token()
    {
        var sut = new DataSectionScanner("DW");
        sut.Scan();

        var firstToken = sut.GetToken();
        var secondToken = sut.GetToken();
        
        Assert.True(firstToken is {Type: DataSectionTokenType.KeywordDataWord});
        Assert.True(secondToken is {Type: DataSectionTokenType.Eof});
    }
    
    [Fact]
    public void DB_Should_Return_KeywordDataDoubleWord_Token()
    {
        var sut = new DataSectionScanner("DD");
        sut.Scan();

        var firstToken = sut.GetToken();
        var secondToken = sut.GetToken();
        
        Assert.True(firstToken is {Type: DataSectionTokenType.KeywordDataDoubleWord});
        Assert.True(secondToken is {Type: DataSectionTokenType.Eof});
    }
    
    [Fact]
    public void DUP_Should_Return_KeywordDup_Token()
    {
        var sut = new DataSectionScanner("DUP");
        sut.Scan();

        var firstToken = sut.GetToken();
        var secondToken = sut.GetToken();
        
        Assert.True(firstToken is {Type: DataSectionTokenType.KeywordDup});
        Assert.True(secondToken is {Type: DataSectionTokenType.Eof});
    }
    
    [Fact]
    public void Word_With_Numbers_Should_Return_Word_Token()
    {
        var sut = new DataSectionScanner("DB485asd");
        sut.Scan();

        var firstToken = sut.GetToken();
        var secondToken = sut.GetToken();
        
        Assert.True(firstToken is {Type: DataSectionTokenType.Word});
        Assert.True(secondToken is {Type: DataSectionTokenType.Eof});
    }
    
    [Fact]
    public void Number_Should_Return_Number_Token()
    {
        var sut = new DataSectionScanner("5484");
        sut.Scan();

        var firstToken = sut.GetToken();
        var secondToken = sut.GetToken();
        
        Assert.True(firstToken is {Type: DataSectionTokenType.Number});
        Assert.True(secondToken is {Type: DataSectionTokenType.Eof});
    }
    
    [Fact]
    public void String_Literal_Should_Return_StringLiteral_Token()
    {
        var sut = new DataSectionScanner("'ASD 78915'");
        sut.Scan();

        var firstToken = sut.GetToken();
        var secondToken = sut.GetToken();
        
        Assert.True(firstToken is {Type: DataSectionTokenType.StringLiteral});
        Assert.Equal("'ASD 78915'", firstToken.Lexeme);
        Assert.True(secondToken is {Type: DataSectionTokenType.Eof});
    }
    
    [Fact]
    public void Numbers_Separated_By_Separators_Should_Return_Multiple_Numbers_And_Separators_Token()
    {
        var sut = new DataSectionScanner("69,420,72");
        sut.Scan();

        List<DataSectionToken> tokens = [];
        for (int i = 0; i < 6; i++)
        {
            tokens.Add(sut.GetToken());
        }
        
        Assert.True(tokens.Select(x=>x.Type).ToList() is 
            [
                DataSectionTokenType.Number,
                DataSectionTokenType.Separator, 
                DataSectionTokenType.Number, 
                DataSectionTokenType.Separator,
                DataSectionTokenType.Number, 
                DataSectionTokenType.Eof
            ]);
    }
}