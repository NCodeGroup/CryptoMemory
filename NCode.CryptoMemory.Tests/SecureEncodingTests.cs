#region Copyright Preamble

// Copyright @ 2025 NCode Group
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

#endregion

using System.Text;

namespace NCode.CryptoMemory.Tests;

public class SecureEncodingTests
{
    [Fact]
    public void ASCII_Valid()
    {
        var encoding = SecureEncoding.ASCII;
        Assert.IsType<ASCIIEncoding>(encoding);
        Assert.Empty(encoding.GetPreamble());
        Assert.Same(EncoderFallback.ExceptionFallback, encoding.EncoderFallback);
        Assert.Same(DecoderFallback.ExceptionFallback, encoding.DecoderFallback);
    }

    [Fact]
    public void ASCII_EncodeInvalid_Throws()
    {
        var encoding = SecureEncoding.ASCII;
        var exception = Assert.Throws<EncoderFallbackException>(() =>
            encoding.GetBytes("😀"));
        Assert.Equal(
            @"Unable to translate Unicode character \\u1F600 at index 0 to specified code page.",
            exception.Message);
    }

    [Fact]
    public void ASCII_DecodeInvalid_Throws()
    {
        var encoding = SecureEncoding.ASCII;
        var exception = Assert.Throws<DecoderFallbackException>(() =>
            encoding.GetString(new byte[] { 0x80 }));
        Assert.Equal(
            "Unable to translate bytes [80] at index 0 from specified code page to Unicode.",
            exception.Message);
    }

    [Fact]
    public void UTF8_Valid()
    {
        var encoding = SecureEncoding.UTF8;
        Assert.IsType<UTF8Encoding>(encoding);
        Assert.Empty(encoding.GetPreamble());
        Assert.Same(EncoderFallback.ExceptionFallback, encoding.EncoderFallback);
        Assert.Same(DecoderFallback.ExceptionFallback, encoding.DecoderFallback);
    }

    [Fact]
    public void UTF8_EncodeInvalid_Throws()
    {
        var encoding = SecureEncoding.UTF8;
        var exception = Assert.Throws<EncoderFallbackException>(() =>
            encoding.GetBytes("a\ud800b"));
        Assert.Equal(
            @"Unable to translate Unicode character \\uD800 at index 1 to specified code page.",
            exception.Message);
    }

    [Fact]
    public void UTF8_DecodeInvalid_Throws()
    {
        var encoding = SecureEncoding.UTF8;
        var exception = Assert.Throws<DecoderFallbackException>(() =>
            encoding.GetString(new byte[] { 0x80 }));
        Assert.Equal(
            "Unable to translate bytes [80] at index 0 from specified code page to Unicode.",
            exception.Message);
    }
}
