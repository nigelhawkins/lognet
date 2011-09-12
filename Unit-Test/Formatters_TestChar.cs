using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using LogNet;

[TestFixture]
public class Formatters_test
{

    #region "Internal methods"

    private void TestChar(char _input, string _expected)
    {
        string _actual = Formatters.ASCIIfy( _input );
        Assert.AreEqual( _expected, _actual );
    }

    #endregion

    [Test]
    public void ASC_CharControl()
    {
        char _input = (char)13;
        TestChar( _input, "<CR>" );
    }

    [Test]
    public void ASC_CharSimple()
    {
        char _input = 'D';
        TestChar( _input, "D" );
    }

    [Test]
    public void ASC_CharHigh()
    {
        char _input = (char)150;
        TestChar( _input, "<150>" );
    }

    [Test]
    public void ASC_CharVHigh()
    {
        char _input = (char)1066;
        TestChar( _input, "<1066>" );
    }

}
