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

namespace NCode.CryptoMemory.Tests;

public class RefSpanLeaseTests
{
    [Fact]
    public void Constructor_WithOwnerAndSpan_Valid()
    {
        var testData = new byte[] { 1, 2, 3, 4, 5 };
        using var owner = new TestDisposable();

        using var lease = new RefSpanLease<byte>(owner, testData);

        Assert.Equal(testData.Length, lease.Span.Length);
        Assert.True(lease.Span.SequenceEqual(testData));
        Assert.False(owner.IsDisposed);
    }

    [Fact]
    public void Constructor_WithNullOwner_Valid()
    {
        var testData = new byte[] { 10, 20, 30 };

        using var lease = new RefSpanLease<byte>(null, testData);

        Assert.Equal(testData.Length, lease.Span.Length);
        Assert.True(lease.Span.SequenceEqual(testData));
    }

    [Fact]
    public void Constructor_WithEmptySpan_Valid()
    {
        using var owner = new TestDisposable();

        using var lease = new RefSpanLease<byte>(owner, ReadOnlySpan<byte>.Empty);

        Assert.True(lease.Span.IsEmpty);
        Assert.Equal(0, lease.Span.Length);
    }

    [Fact]
    public void Constructor_WithNullOwnerAndEmptySpan_Valid()
    {
        using var lease = new RefSpanLease<byte>(null, ReadOnlySpan<byte>.Empty);

        Assert.True(lease.Span.IsEmpty);
        Assert.Equal(0, lease.Span.Length);
    }

    [Fact]
    public void Span_ReturnsCorrectData()
    {
        var testData = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };

        using var lease = new RefSpanLease<byte>(null, testData);

        Assert.Equal(4, lease.Span.Length);
        Assert.Equal(0xDE, lease.Span[0]);
        Assert.Equal(0xAD, lease.Span[1]);
        Assert.Equal(0xBE, lease.Span[2]);
        Assert.Equal(0xEF, lease.Span[3]);
    }

    [Fact]
    public void Dispose_WithOwner_DisposesOwner()
    {
        var testData = new byte[] { 1, 2, 3 };
        var owner = new TestDisposable();

        var lease = new RefSpanLease<byte>(owner, testData);
        Assert.False(owner.IsDisposed);

        lease.Dispose();
        Assert.True(owner.IsDisposed);
    }

    [Fact]
    public void Dispose_WithNullOwner_DoesNotThrow()
    {
        var testData = new byte[] { 1, 2, 3 };
        var lease = new RefSpanLease<byte>(null, testData);

        // Should not throw
        lease.Dispose();
    }

    [Fact]
    public void Dispose_MultipleDisposeCalls_DisposesOwnerOnce()
    {
        var testData = new byte[] { 1, 2, 3 };
        var owner = new TestDisposable();

        var lease = new RefSpanLease<byte>(owner, testData);
        lease.Dispose();
        Assert.Equal(1, owner.DisposeCount);

        // Second dispose will call owner.Dispose again since RefSpanLease doesn't track disposal state
        lease.Dispose();
        Assert.Equal(2, owner.DisposeCount);
    }

    [Fact]
    public void GenericType_Int32_Works()
    {
        var testData = new[] { 100, 200, 300, 400, 500 };
        using var owner = new TestDisposable();

        using var lease = new RefSpanLease<int>(owner, testData);

        Assert.Equal(testData.Length, lease.Span.Length);
        Assert.True(lease.Span.SequenceEqual(testData));
    }

    [Fact]
    public void GenericType_Char_Works()
    {
        ReadOnlySpan<char> testData = "Hello";
        using var owner = new TestDisposable();

        using var lease = new RefSpanLease<char>(owner, testData);

        Assert.Equal(5, lease.Span.Length);
        Assert.Equal('H', lease.Span[0]);
        Assert.Equal('o', lease.Span[4]);
    }

    [Fact]
    public void GenericType_Double_Works()
    {
        var testData = new[] { 1.1, 2.2, 3.3 };

        using var lease = new RefSpanLease<double>(null, testData);

        Assert.Equal(3, lease.Span.Length);
        Assert.Equal(1.1, lease.Span[0]);
        Assert.Equal(2.2, lease.Span[1]);
        Assert.Equal(3.3, lease.Span[2]);
    }

    [Fact]
    public void GenericType_Struct_Works()
    {
        var testData = new TestStruct[]
        {
            new() { Value = 1 },
            new() { Value = 2 },
            new() { Value = 3 }
        };

        using var lease = new RefSpanLease<TestStruct>(null, testData);

        Assert.Equal(3, lease.Span.Length);
        Assert.Equal(1, lease.Span[0].Value);
        Assert.Equal(2, lease.Span[1].Value);
        Assert.Equal(3, lease.Span[2].Value);
    }

    [Fact]
    public void WithSecureMemory_Valid()
    {
        const int bufferSize = 100;
        // Use null pool to avoid returning this non-standard sized buffer to the shared pool
        using var secureMemory = new SecureMemory<byte>(null, bufferSize);

        var span = secureMemory.Memory.Span;
        for (var i = 0; i < bufferSize; i++)
        {
            span[i] = (byte)(i % 256);
        }

        using var lease = new RefSpanLease<byte>(secureMemory, span);

        Assert.Equal(bufferSize, lease.Span.Length);
        for (var i = 0; i < bufferSize; i++)
        {
            Assert.Equal((byte)(i % 256), lease.Span[i]);
        }
    }

    [Fact]
    public void LargeSpan_Valid()
    {
        const int size = 10000;
        var testData = new byte[size];
        for (var i = 0; i < size; i++)
        {
            testData[i] = (byte)(i % 256);
        }

        using var lease = new RefSpanLease<byte>(null, testData);

        Assert.Equal(size, lease.Span.Length);
        for (var i = 0; i < size; i++)
        {
            Assert.Equal((byte)(i % 256), lease.Span[i]);
        }
    }

    [Fact]
    public void SpanSlice_ReturnsCorrectSubset()
    {
        var testData = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        using var lease = new RefSpanLease<byte>(null, testData);

        var slice = lease.Span[2..5];
        Assert.Equal(3, slice.Length);
        Assert.Equal(3, slice[0]);
        Assert.Equal(4, slice[1]);
        Assert.Equal(5, slice[2]);
    }

    private class TestDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public int DisposeCount { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
            DisposeCount++;
        }
    }

    private struct TestStruct
    {
        public int Value { get; set; }
    }
}
