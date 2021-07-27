using NUnit.Framework;


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

namespace Z85Encoding.Tests
{
    public class Z85EncodingTests
    {
        [SetUp]
        public void Setup()
        {
            if (Z85Encoding.Instance == null)
                throw new System.Exception("Invalid Exception");
        }

        [Test]
        public void NullEncode()
        {
            string encoded = Z85Encoding.Instance.Encode(null);
            byte[] decoded = Z85Encoding.Instance.Decode(encoded);

            Assert.AreEqual(string.Empty, encoded);
            Assert.AreEqual(null, decoded);
        }

        [Test]
        public void InvalidEncodeLength()
        {
            byte[] testData = {
                0x86, 0x4F, 0xD2
            };
            string encoded = Z85Encoding.Instance.Encode(testData);
            Assert.IsNull(encoded);

        }


        [Test]
        public void ValidEncodeLength()
        {
            byte[] testData = {
                0x86, 0x4F, 0xD2, 0x6F, 0xB5, 0x59, 0xF7, 0x5B
            };
            string encoded = Z85Encoding.Instance.Encode(testData);
            Assert.AreEqual(10, encoded.Length);
            Assert.AreEqual("HelloWorld", encoded);

            byte[] decoded = Z85Encoding.Instance.Decode(encoded);
            Assert.AreEqual(testData, decoded);
        }


        [Test]
        public void ValidEncodeLength2()
        {
            byte[] testData = {
                0x8E, 0x0B, 0xDD, 0x69, 0x76, 0x28, 0xB9, 0x1D,
                0x8F, 0x24, 0x55, 0x87, 0xEE, 0x95, 0xC5, 0xB0,
                0x4D, 0x48, 0x96, 0x3F, 0x79, 0x25, 0x98, 0x77,
                0xB4, 0x9C, 0xD9, 0x06, 0x3A, 0xEA, 0xD3, 0xB7
            };
            string encoded = Z85Encoding.Instance.Encode(testData);
            Assert.AreEqual(40, encoded.Length);
            Assert.AreEqual("JTKVSB%%)wK0E.X)V>+}o?pNmC{O&4W4b!Ni{Lh6", encoded);

            byte[] decoded = Z85Encoding.Instance.Decode(encoded);
            Assert.AreEqual(testData, decoded);

        }
    }
}