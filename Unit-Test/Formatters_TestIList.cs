using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using LogNet;

[TestFixture]
public class Formatters_TestIList
{

    #region "Internal methods"

    private void TestIList(IList _input, string _expected)
    {
        string _actual = Formatters.ASCIIfy( _input );
        Assert.AreEqual( _expected, _actual );
    }

    #endregion

    [Test]
    public void ASC_ListNull()
    {
        List<int> _input = null;
        TestIList( _input, "{}" );
    }

    [Test]
    public void ASC_ListEmpty()
    {
        List<int> _input = new List<int>();
        TestIList( _input, "{}" );
    }

    [Test]
    public void ASC_ListSimple()
    {
        List<int> _input = new List<int>();
        _input.Add( 1 );
        _input.Add( 2 );
        _input.Add( 3 );
        TestIList( _input, "{1, 2, 3}" );
    }

    [Test]
    public void ASC_ListMixed()
    {
        ArrayList _input = new ArrayList();
        _input.Add( 1 );
        _input.Add( "Two" );
        _input.Add( new List<int>() );
        TestIList( _input, "{1, \"Two\", {}}" );
    }

}
