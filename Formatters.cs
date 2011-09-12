// Part of the LogNet logging library.
// Copyright (c) 2011 Nigel Hawkins
//
// Licensed under the Gnu LGPL version 3.
// See http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections;
using System.Text;

namespace LogNet
{
    /// <summary>
    /// This class contains some simple methods to format various data types 
    /// into human-readable ASCII strings.
    /// </summary>
    public static class Formatters
    {
        #region "Variables, Constants and Enumerations"

        private static string[] controlChars = new string[]
    		{"<NUL>", "<SOH>", "<STX>", "<ETX>", 
             "<EOT>", "<ENQ>", "<ACK>", "<BEL>", 
			 "<BS>", "<HT>", "<LF>", "<VT>", 
			 "<FF>", "<CR>", "<SO>", "<SI>", 
			 "<DLE>", "<DC1>", "<DC2>", "<DC3>", 
		     "<DC4>", "<NAK>", "<SYN>", "<ETB>", 
			 "<CAN>", "<EM>", "<SUB>", "<ESC>", 
			 "<FS>", "<GS>", "<RS>", "<US>"
		};

        #endregion

        /// <summary>
        /// Convert a list into a human-readable string representation. The 
        /// representation used is: (0 to 2) {1, 2, 3}
        /// </summary>
        /// <param name="data">The array to process</param>
        /// <returns>A string containing the array data.</returns>
        public static string ASCIIfy(Array data)
        {
            if ( data == null )
                return "()";

            StringBuilder sb = new StringBuilder("(");

            sb.Append( String.Format( ResStrings.GetString( "ArrayLimits" ),
                                   data.GetLowerBound( 0 ),
                                   data.GetUpperBound( 0 ) ) );
            sb.Append( ASCIIfy((IList)data) );
            sb.Append( ")" );

            return sb.ToString();
        }

        /// <summary>
        /// Convert a list into a human-readable string representation. The 
        /// representation used is: {1,2,3}
        /// </summary>
        /// <param name="source">The list to be serialised.</param>
        /// <returns>The serialised string.</returns>
        public static string ASCIIfy(IList source)
        {
            if (source == null || source.Count == 0)
                return "{}";

            StringBuilder sb = new StringBuilder("{");

            if ( source != null ) {
                foreach (object obj in source) {
                    sb.Append( ASCIIfy( obj ) + ", " );
                }
                sb.Remove( sb.Length - 2, 2 );
            }
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Convert a single character into a string representation that is 
        /// printable using ASCII characters only.
        /// </summary>
        /// <param name="source">The raw character to be processed.</param>
        /// <returns>An ASCII version of the same character.</returns>
        /// <remarks>The replacement involves replacing all non printable 
        /// characters by a token like &lt;CR&gt; or &lt;255&gt;
        /// <para><example>Passing in the carraige return character would return
        /// "&lt;CR&gt;</example></para></remarks>
        public static string ASCIIfy(char source)
        {
            int code = (int)source;

            if (code >= 0 && code < controlChars.Length) {
                return controlChars[code];
            } else if (code < 127) {
                return source.ToString();
            } else {
                return "<" + code.ToString() + ">";
            }
        }

        /// <summary>
        /// Convert a string into a representation of the same string that is 
        /// printable using ASCII characters only.
        /// </summary>
        /// <param name="vRaw">The raw string to be processed.</param>
        /// <returns>A printable version of the same string.</returns>
        /// <remarks>The replacement involves replacing all non printable 
        /// characters by a token like &lt;CR&gt; or &lt;255&gt;
        /// <para><example>Passing in the string "?ID\r\n" would return
        /// "?ID&lt;CR&gt;&lt;LF&gt;"</example></para></remarks>
        public static string ASCIIfy(string source)
        {

            StringBuilder sb = new StringBuilder("\"");

            if ( !string.IsNullOrEmpty( source ) ) {
                for ( int i = 0; i < source.Length; ++i ) {
                    sb.Append( ASCIIfy( source[ i ] ) );
                }
            }

            sb.Append( "\"" );

            return sb.ToString();
        }

        /// <summary>
        /// Convert an object into an ASCII string representation.
        /// </summary>
        /// <param name="source">The target object to be processed.</param>
        /// <returns>The string representation.</returns>
        /// <remarks>This will farm out to more explicit handlers if 
        /// necessary. If not it just defaults to object.ToString()</remarks>
        public static string ASCIIfy(object source)
        {
            //If we have a  better decoder, make sure we use it.
            if ( source is String ) 
            {
                return ASCIIfy( (string)source );
            } 
            else if ( source is char ) 
            {
                return ASCIIfy( (char)source );
            } 
            else if ( source is Array ) 
            {
                return ASCIIfy( (Array)source );
            } 
            else if ( source is IList ) 
            {
                return ASCIIfy( (IList)source );
            } 
            else 
            {
                return source.ToString();
            }
        }

    }

}
