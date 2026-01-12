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

public class SecureArrayLifetimeTests
{
    [Fact]
    public void Create_ReturnsLifetimeWithCorrectLength()
    {
        using var lifetime = SecureArrayLifetime<byte>.Create(64);

        Assert.Equal(64, lifetime.PinnedArray.Length);
    }

    [Fact]
    public void Constructor_AllocatesPinnedArray()
    {
        using var lifetime = new SecureArrayLifetime<byte>(128);

        Assert.NotNull(lifetime.PinnedArray);
        Assert.Equal(128, lifetime.PinnedArray.Length);
    }

    [Fact]
    public void PinnedArray_ReturnsSameInstance()
    {
        using var lifetime = new SecureArrayLifetime<int>(32);

        var array1 = lifetime.PinnedArray;
        var array2 = lifetime.PinnedArray;

        Assert.Same(array1, array2);
    }

    [Fact]
    public void ImplicitConversion_ReturnsSpanOverPinnedArray()
    {
        using var lifetime = new SecureArrayLifetime<byte>(64);
        lifetime.PinnedArray[0] = 0xAB;
        lifetime.PinnedArray[63] = 0xCD;

        Span<byte> span = lifetime;

        Assert.Equal(64, span.Length);
        Assert.Equal(0xAB, span[0]);
        Assert.Equal(0xCD, span[63]);
    }

    [Fact]
    public void Dispose_ZerosMemory_ByteArray()
    {
        var lifetime = SecureArrayLifetime<byte>.Create(64);
        RandomNumberGenerator.Fill(lifetime.PinnedArray);
        Assert.False(lifetime.PinnedArray.All(b => b == 0));

        lifetime.Dispose();

        Assert.True(lifetime.PinnedArray.All(b => b == 0));
    }

    [Fact]
    public void Dispose_ZerosMemory_IntArray()
    {
        var lifetime = new SecureArrayLifetime<int>(16);
        for (var i = 0; i < lifetime.PinnedArray.Length; i++)
            lifetime.PinnedArray[i] = i + 1;

        Assert.False(lifetime.PinnedArray.All(x => x == 0));

        lifetime.Dispose();

        Assert.True(lifetime.PinnedArray.All(x => x == 0));
    }

    [Fact]
    public void Dispose_ZerosMemory_LongArray()
    {
        var lifetime = new SecureArrayLifetime<long>(8);
        for (var i = 0; i < lifetime.PinnedArray.Length; i++)
            lifetime.PinnedArray[i] = long.MaxValue - i;

        Assert.False(lifetime.PinnedArray.All(x => x == 0));

        lifetime.Dispose();

        Assert.True(lifetime.PinnedArray.All(x => x == 0));
    }

    [Fact]
    public void Dispose_ZerosMemory_CustomStruct()
    {
        var lifetime = new SecureArrayLifetime<TestStruct>(4);
        for (var i = 0; i < lifetime.PinnedArray.Length; i++)
            lifetime.PinnedArray[i] = new TestStruct { A = i, B = (byte)(i + 1), C = i * 1.5 };

        Assert.False(lifetime.PinnedArray.All(x => x is { A: 0, B: 0, C: 0 }));

        lifetime.Dispose();

        Assert.True(lifetime.PinnedArray.All(x => x is { A: 0, B: 0, C: 0 }));
    }

    [Fact]
    public void UsingStatement_ZerosMemoryOnExit()
    {
        byte[] capturedArray;

        using (var lifetime = SecureArrayLifetime<byte>.Create(128))
        {
            capturedArray = lifetime.PinnedArray;
            RandomNumberGenerator.Fill(capturedArray);
            Assert.False(capturedArray.All(b => b == 0));
        }

        Assert.True(capturedArray.All(b => b == 0));
    }

    [Fact]
    public void EmptyArray_DoesNotThrowOnDispose()
    {
        var lifetime = new SecureArrayLifetime<byte>(0);

        // Should not throw - if it does, the test will fail
        lifetime.Dispose();

        Assert.Empty(lifetime.PinnedArray);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var lifetime = SecureArrayLifetime<byte>.Create(32);
        RandomNumberGenerator.Fill(lifetime.PinnedArray);

        lifetime.Dispose();
        Assert.True(lifetime.PinnedArray.All(b => b == 0));

        // Second dispose should not throw - if it does, the test will fail
        lifetime.Dispose();
    }

    [Fact]
    public void SpanModifications_AreReflectedInPinnedArray()
    {
        using var lifetime = new SecureArrayLifetime<byte>(16);

        Span<byte> span = lifetime;
        span[0] = 0xFF;
        span[15] = 0xAA;

        Assert.Equal(0xFF, lifetime.PinnedArray[0]);
        Assert.Equal(0xAA, lifetime.PinnedArray[15]);
    }

    [Fact]
    public void PinnedArrayModifications_AreReflectedInSpan()
    {
        using var lifetime = new SecureArrayLifetime<byte>(16);

        lifetime.PinnedArray[0] = 0x11;
        lifetime.PinnedArray[15] = 0x22;

        Span<byte> span = lifetime;
        Assert.Equal(0x11, span[0]);
        Assert.Equal(0x22, span[15]);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(16)]
    [InlineData(256)]
    [InlineData(1024)]
    [InlineData(4096)]
    public void Create_VariousSizes_AllocatesCorrectLength(int length)
    {
        using var lifetime = SecureArrayLifetime<byte>.Create(length);

        Assert.Equal(length, lifetime.PinnedArray.Length);
    }

    private struct TestStruct
    {
        public int A;
        public byte B;
        public double C;
    }
}
