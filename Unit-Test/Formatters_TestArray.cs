using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using LogNet;

[TestFixture]
public class Formatters_TestArray
{

    #region "Internal methods"

    private void TestArray(Array _input, string _expected)
    {
        string _actual = Formatters.ASCIIfy( _input );
        Assert.AreEqual( _expected, _actual );
    }

    #endregion

    [Test]
    public void ASC_Null()
    {
        int[] _input = null;
        TestArray( _input, "()" );
    }

    [Test]
    public void ASC_Simple()
    {
        int[] _input = new int[] { 1, 2, 3 };
        TestArray( _input, "(0 to 2 : {1, 2, 3})" );
    }

}
