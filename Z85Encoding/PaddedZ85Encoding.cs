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
    public class PaddedZ85Encoding : Z85Encoding
    {
        private static PaddedZ85Encoding instance = null;
        public new static PaddedZ85Encoding Instance
        {
            get
            {
                if (instance == null)
                    instance = new PaddedZ85Encoding();
                return instance;
            }
        }

        /// <summary>
        /// Encode bytes into a Z85 string adding padding information; size of array will be roughly input.Length * 5 / 4 plus 1 plus any extra padding needed to round to up to a 32 bit word.
        /// </summary>
        /// <param name="input">Bytes to encode</param>
        /// <returns>Encoded PaddedZ85 string</returns>
        public override string Encode(byte[] input)
        {
            if (input == null)
                return base.Encode(input);

            int inputLength = input.Length;
            int paddingLength = 4 - ((inputLength + 1) % 4);
            if (paddingLength == 4)
                paddingLength = 0;

            byte[] paddedInput = new byte[input.Length + paddingLength + 1];
            paddedInput[0] = (byte)paddingLength;
            for (int i = 0; i < paddingLength; i++)
                paddedInput[1 + i] = 0x00;

            for (int i = 1 + paddingLength, j = 0; i < paddedInput.Length; i++)
                paddedInput[i] = input[j++];
            return base.Encode(paddedInput);
        }


        /// <summary>
        /// Decode an encoded string into a byte array; size of array will be roughly input.Length * 4 / 5 minus 1 and minus any extra padding added.
        /// </summary>
        /// <param name="input">PaddedZ85 string to decode</param>
        /// <returns>Decoded bytes</returns>
        public override byte[] Decode(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
                return base.Decode(input);

            byte[] rawDecoded = base.Decode(input);
            if (rawDecoded == null)
                return rawDecoded;
            int paddingLength = rawDecoded[0];
            if (paddingLength > 3 || paddingLength < 0)
                throw new ArgumentOutOfRangeException(nameof(input), rawDecoded[0], "Padded Z85 String must start with padding length of 0-4");
            byte[] processedDecoded = new byte[rawDecoded.Length - 1 - paddingLength];
            for (int i = 0, j = 1 + paddingLength; i < processedDecoded.Length; i++)
                processedDecoded[i] = rawDecoded[j++];
            return processedDecoded;
        }
    }
}
