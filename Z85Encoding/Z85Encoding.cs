using System;
using System.Diagnostics;

//  --------------------------------------------------------------------------
//  C# Z85 Reference Port Implementation
//  --------------------------------------------------------------------------
//  Copyright (c) 2021 Chris Webb
//  
//  Permission is hereby granted, free of charge, to any person obtaining a 
//  copy of this software and associated documentation files (the "Software"),
//  to deal in the Software without restriction, including without limitation 
//  the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//  and/or sell copies of the Software, and to permit persons to whom the 
//  Software is furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in 
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//  THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
//  DEALINGS IN THE SOFTWARE.
//  --------------------------------------------------------------------------
//  Based on the following:
//  --------------------------------------------------------------------------
//  Reference implementation for rfc.zeromq.org/spec:32/Z85
//
//  This implementation provides a Z85 codec as an easy-to-reuse C# class 
//  designed to be easy to port into other languages.

//  --------------------------------------------------------------------------
//  Copyright (c) 2010-2013 iMatix Corporation and Contributors
//  
//  Permission is hereby granted, free of charge, to any person obtaining a 
//  copy of this software and associated documentation files (the "Software"),
//  to deal in the Software without restriction, including without limitation 
//  the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//  and/or sell copies of the Software, and to permit persons to whom the 
//  Software is furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in 
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//  THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
//  DEALINGS IN THE SOFTWARE.
//  --------------------------------------------------------------------------

namespace Z85Encoding
{
    public class Z85Encoding
    {
        private static Z85Encoding instance = null;
        public static Z85Encoding Instance
        {
            get
            {
                if (instance == null)
                    instance = new Z85Encoding();
                return instance;
            }
        }
        //  Maps base 256 to base 85
        private static readonly char[] encoderMap =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
            'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
            'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
            'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
            'Y', 'Z', '.', '-', ':', '+', '=', '^', '!', '/',
            '*', '?', '&', '<', '>', '(', ')', '[', ']', '{',
            '}', '@', '%', '$', '#'
        };

        //  Maps base 85 to base 256
        //  We chop off lower 32 and higher 128 ranges
        private static readonly byte[] decoderMap =
        {
            0x00, 0x44, 0x00, 0x54, 0x53, 0x52, 0x48, 0x00,
            0x4B, 0x4C, 0x46, 0x41, 0x00, 0x3F, 0x3E, 0x45,
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
            0x08, 0x09, 0x40, 0x00, 0x49, 0x42, 0x4A, 0x47,
            0x51, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A,
            0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30, 0x31, 0x32,
            0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A,
            0x3B, 0x3C, 0x3D, 0x4D, 0x00, 0x4E, 0x43, 0x00,
            0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10,
            0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18,
            0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20,
            0x21, 0x22, 0x23, 0x4F, 0x00, 0x50, 0x00, 0x00
        };

        /// <summary>
        /// Encode a byte array as a string
        /// </summary>
        /// <param name="input">Bytes to encode</param>
        /// <returns>Encoded Z85 string</returns>
        public virtual string Encode(byte[] input)
        {
            if (input == null)
                return String.Empty;

            int size = input.Length;
            //  Accepts only byte arrays bounded to 4 bytes
            if (size % 4 != 0)
                return null;

            int encodedSize = input.Length * 5 / 4;
            char[] encodedChars = new char[encodedSize];

            uint charNbr = 0;
            uint byteNbr = 0;
            UInt32 value = 0;
            while (byteNbr < size)
            {
                //  Accumulate value in base 256 (binary)
                value = value * 256 + input[byteNbr++];
                if (byteNbr % 4 == 0)
                {
                    //  Output value in base 85
                    uint divisor = 85 * 85 * 85 * 85;
                    while (divisor > 0)
                    {
                        encodedChars[charNbr++] = encoderMap[value / divisor % 85];
                        divisor /= 85;
                    }
                    value = 0;
                }
            }
            Debug.Assert(charNbr == encodedSize);
            return new string(encodedChars);
        }


        /// <summary>
        /// Decode an encoded string into a byte array; size of array will be input.Length * 4 / 5.
        /// </summary>
        /// <param name="input">Encoded Z85 string</param>
        /// <returns>Decoded bytes</returns>
        public virtual byte[] Decode(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
                return null;

            //  Accepts only strings bounded to 5 bytes
            if (input.Length % 5 != 0)
                return null;

            int decodedSize = input.Length * 4 / 5;
            byte[] decodedBytes = new byte[decodedSize];

            uint byteNbr = 0;
            uint charNbr = 0;
            UInt32 value = 0;

            while (charNbr < input.Length)
            {
                //  Accumulate value in base 85
                value = value * 85 + decoderMap[(byte)input[(int)charNbr++] - 32];
                if (charNbr % 5 == 0)
                {
                    //  Output value in base 256
                    uint divisor = 256 * 256 * 256;
                    while (divisor > 0)
                    {
                        decodedBytes[byteNbr++] = (byte)(value / divisor % 256);
                        divisor /= 256;
                    }
                    value = 0;
                }
            }
            Debug.Assert(byteNbr == decodedSize);
            return decodedBytes;
        }
    }
}
