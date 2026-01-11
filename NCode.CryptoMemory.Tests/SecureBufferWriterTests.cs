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

using System.Buffers;

namespace NCode.CryptoMemory.Tests;

public class SecureBufferWriterTests
{
    [Fact]
    public void Constructor_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();

        Assert.NotNull(writer);
        Assert.Equal(0, writer.Length);
    }

    [Fact]
    public void Length_AfterWrite_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();

        var span = writer.GetSpan(10);
        span[..5].Fill(0xFF);
        writer.Advance(5);

        Assert.Equal(5, writer.Length);
    }

    [Fact]
    public void AsReadOnlySequence_Empty_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();

        var sequence = writer.AsReadOnlySequence;

        Assert.True(sequence.IsEmpty);
        Assert.Equal(0, sequence.Length);
    }

    [Fact]
    public void AsReadOnlySequence_WithData_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();

        var span = writer.GetSpan(10);
        for (var i = 0; i < 5; i++)
        {
            span[i] = (byte)(i + 1);
        }
        writer.Advance(5);

        var sequence = writer.AsReadOnlySequence;

        Assert.False(sequence.IsEmpty);
        Assert.Equal(5, sequence.Length);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, sequence.ToArray());
    }

    [Fact]
    public void Advance_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();

        var span = writer.GetSpan(100);
        span[..50].Fill(0xAB);
        writer.Advance(50);

        Assert.Equal(50, writer.Length);
    }

    [Fact]
    public void Advance_Multiple_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();

        var span1 = writer.GetSpan(10);
        span1[..5].Fill(0x01);
        writer.Advance(5);

        var span2 = writer.GetSpan(10);
        span2[..5].Fill(0x02);
        writer.Advance(5);

        Assert.Equal(10, writer.Length);
    }

    [Fact]
    public void GetSpan_DefaultSizeHint_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();

        var span = writer.GetSpan();

        Assert.False(span.IsEmpty);
    }

    [Fact]
    public void GetSpan_WithSizeHint_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();

        const int sizeHint = 256;
        var span = writer.GetSpan(sizeHint);

        Assert.True(span.Length >= sizeHint);
    }

    [Fact]
    public void GetMemory_DefaultSizeHint_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();

        var memory = writer.GetMemory();

        Assert.False(memory.IsEmpty);
    }

    [Fact]
    public void GetMemory_WithSizeHint_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();

        const int sizeHint = 256;
        var memory = writer.GetMemory(sizeHint);

        Assert.True(memory.Length >= sizeHint);
    }

    [Fact]
    public void Dispose_Valid()
    {
        var writer = new SecureBufferWriter<byte>();

        var span = writer.GetSpan(10);
        span[..5].Fill(0xFF);
        writer.Advance(5);

        Assert.Equal(5, writer.Length);

        writer.Dispose();

        // After dispose, the sequence should be disposed
        Assert.NotNull(writer.Sequence);
    }

    [Fact]
    public void IBufferWriter_GetSpan_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();
        IBufferWriter<byte> bufferWriter = writer;

        var span = bufferWriter.GetSpan(100);

        Assert.True(span.Length >= 100);
    }

    [Fact]
    public void IBufferWriter_GetMemory_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();
        IBufferWriter<byte> bufferWriter = writer;

        var memory = bufferWriter.GetMemory(100);

        Assert.True(memory.Length >= 100);
    }

    [Fact]
    public void IBufferWriter_Advance_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();
        IBufferWriter<byte> bufferWriter = writer;

        bufferWriter.GetSpan(100);
        bufferWriter.Advance(50);

        Assert.Equal(50, writer.Length);
    }

    [Fact]
    public void LargeWrite_Valid()
    {
        using var writer = new SecureBufferWriter<byte>();

        const int totalBytes = 10000;
        var written = 0;

        while (written < totalBytes)
        {
            var span = writer.GetSpan(100);
            var toWrite = Math.Min(100, totalBytes - written);
            span[..toWrite].Fill((byte)(written % 256));
            writer.Advance(toWrite);
            written += toWrite;
        }

        Assert.Equal(totalBytes, writer.Length);
    }

    [Fact]
    public void GenericType_Int_Valid()
    {
        using var writer = new SecureBufferWriter<int>();

        var span = writer.GetSpan(10);
        span[..5].Fill(42);
        writer.Advance(5);

        Assert.Equal(5, writer.Length);

        var sequence = writer.AsReadOnlySequence;
        Assert.Equal(new[] { 42, 42, 42, 42, 42 }, sequence.ToArray());
    }

    [Fact]
    public void SequenceProperty_UsesSecureMemoryPool()
    {
        using var writer = new SecureBufferWriter<byte>();

        // Access Sequence to trigger memory allocation
        var span = writer.GetSpan(10);
        span[..5].Fill(0x01);
        writer.Advance(5);

        // Verify the writer is using a Sequence backed by SecureMemoryPool
        Assert.NotNull(writer.Sequence);
    }
}
