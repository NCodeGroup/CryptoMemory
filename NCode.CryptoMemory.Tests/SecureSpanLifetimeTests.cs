#region Copyright Preamble

// Copyright @ 2026 NCode Group
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

using System.Security.Cryptography;

namespace NCode.CryptoMemory.Tests;

public class SecureSpanLifetimeTests
{
    [Fact]
    public void Constructor_SetsSpan()
    {
        Span<byte> buffer = stackalloc byte[16];
        buffer.Fill(0xAB);

        var lifetime = new SecureSpanLifetime<byte>(buffer);

        Assert.Equal(16, lifetime.Span.Length);
        Assert.True(lifetime.Span.SequenceEqual(buffer));
    }

    [Fact]
    public void Span_ReturnsOriginalSpan()
    {
        Span<int> buffer = stackalloc int[8];
        for (var i = 0; i < buffer.Length; i++)
            buffer[i] = i * 10;

        var lifetime = new SecureSpanLifetime<int>(buffer);

        for (var i = 0; i < buffer.Length; i++)
            Assert.Equal(i * 10, lifetime.Span[i]);
    }

    [Fact]
    public void ImplicitConversion_ReturnsUnderlyingSpan()
    {
        Span<byte> buffer = stackalloc byte[32];
        RandomNumberGenerator.Fill(buffer);
        var originalCopy = buffer.ToArray();

        var lifetime = new SecureSpanLifetime<byte>(buffer);
        Span<byte> convertedSpan = lifetime;

        Assert.Equal(32, convertedSpan.Length);
        Assert.True(convertedSpan.SequenceEqual(originalCopy));
    }

    [Fact]
    public void Dispose_ZerosMemory_ByteSpan()
    {
        var array = new byte[64];
        RandomNumberGenerator.Fill(array);
        Assert.False(array.All(b => b == 0));

        var lifetime = new SecureSpanLifetime<byte>(array);
        lifetime.Dispose();

        Assert.True(array.All(b => b == 0));
    }

    [Fact]
    public void Dispose_ZerosMemory_IntSpan()
    {
        var array = new int[16];
        for (var i = 0; i < array.Length; i++)
            array[i] = i + 1;

        Assert.False(array.All(x => x == 0));

        var lifetime = new SecureSpanLifetime<int>(array);
        lifetime.Dispose();

        Assert.True(array.All(x => x == 0));
    }

    [Fact]
    public void Dispose_ZerosMemory_LongSpan()
    {
        var array = new long[8];
        for (var i = 0; i < array.Length; i++)
            array[i] = long.MaxValue - i;

        Assert.False(array.All(x => x == 0));

        var lifetime = new SecureSpanLifetime<long>(array);
        lifetime.Dispose();

        Assert.True(array.All(x => x == 0));
    }

    [Fact]
    public void Dispose_ZerosMemory_CustomStruct()
    {
        var array = new TestStruct[4];
        for (var i = 0; i < array.Length; i++)
            array[i] = new TestStruct { A = i, B = (byte)(i + 1), C = i * 1.5 };

        Assert.False(array.All(x => x is { A: 0, B: 0, C: 0 }));

        var lifetime = new SecureSpanLifetime<TestStruct>(array);
        lifetime.Dispose();

        Assert.True(array.All(x => x is { A: 0, B: 0, C: 0 }));
    }

    [Fact]
    public void UsingStatement_ZerosMemoryOnExit()
    {
        var array = new byte[128];
        RandomNumberGenerator.Fill(array);
        Assert.False(array.All(b => b == 0));

        using (var _ = new SecureSpanLifetime<byte>(array))
        {
            // Work with the span
            Assert.False(array.All(b => b == 0));
        }

        Assert.True(array.All(b => b == 0));
    }

    [Fact]
    public void EmptySpan_DoesNotThrowOnDispose()
    {
        var lifetime = new SecureSpanLifetime<byte>(Span<byte>.Empty);

        // Should not throw - if it does, the test will fail
        lifetime.Dispose();
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var array = new byte[32];
        RandomNumberGenerator.Fill(array);

        var lifetime = new SecureSpanLifetime<byte>(array);
        lifetime.Dispose();
        Assert.True(array.All(b => b == 0));

        // Second dispose should not throw - if it does, the test will fail
        lifetime.Dispose();
    }

    [Fact]
    public void SpanModifications_AreReflectedInOriginal()
    {
        var array = new byte[16];
        var lifetime = new SecureSpanLifetime<byte>(array);

        lifetime.Span[0] = 0xFF;
        lifetime.Span[15] = 0xAA;

        Assert.Equal(0xFF, array[0]);
        Assert.Equal(0xAA, array[15]);
    }

    private struct TestStruct
    {
        public int A;
        public byte B;
        public double C;
    }
}
