using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using LogNet;

[TestFixture]
public class Formatters_TestString
{

    #region "Internal methods"

    private void TestString(string _input, string _expected)
    {
        string _actual = Formatters.ASCIIfy( _input );
        Assert.AreEqual( _expected, _actual );
    }

    #endregion

    [Test]
    public void ASC_StringEasy()
    {
        string _input = "Abcde";
        TestString( _input, "\"Abcde\"" );
    }

    [Test]
    public void ASC_StringControl()
    {
        string _input = ((char)2).ToString() + "Abcde" + ((char)3).ToString() + ((char)13).ToString();
        TestString( _input, "\"<STX>Abcde<ETX><CR>\"" );
    }

    [Test]
    public void ASC_StringHigh()
    {
        string _input = ((char)127).ToString() + ((char)255).ToString() + ((char)65535).ToString();
        TestString( _input, "\"<127><255><65535>\"" );
    }

    [Test]
    public void ASC_StringNull()
    {
        //NULL string should return ""
        string _input = null;
        TestString( _input, "\"\"" );
    }

    [Test]
    public void ASC_StringEmpty()
    {
        //Empty string should return ""
        TestString( string.Empty, "\"\"" );
    }

}
