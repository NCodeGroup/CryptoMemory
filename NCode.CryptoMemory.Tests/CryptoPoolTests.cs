#region Copyright Preamble

//
//    Copyright @ 2023 NCode Group
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

public class CryptoPoolTests
{
    [Fact]
    public void ChoosePool_Secure()
    {
        const bool isSensitive = true;
        var pool = CryptoPool.ChoosePool(isSensitive);
        Assert.Same(SecureMemoryPool<byte>.Shared, pool);
    }

    [Fact]
    public void ChoosePool_Standard()
    {
        const bool isSensitive = false;
        var pool = CryptoPool.ChoosePool(isSensitive);
        Assert.Same(MemoryPool<byte>.Shared, pool);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Rent_Span_Valid(bool isSensitive)
    {
        const int minBufferSize = 1024;
        using var lease = CryptoPool.Rent(minBufferSize, isSensitive, out Span<byte> buffer);

        if (isSensitive)
        {
            Assert.IsType<SecureMemory<byte>>(lease);
        }
        else
        {
            Assert.IsNotType<SecureMemory<byte>>(lease);
        }

        Assert.InRange(lease.Memory.Length, minBufferSize, int.MaxValue);
        Assert.Equal(minBufferSize, buffer.Length);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Rent_Memory_Valid(bool isSensitive)
    {
        const int minBufferSize = 1024;
        using var lease = CryptoPool.Rent(minBufferSize, isSensitive, out Memory<byte> buffer);

        if (isSensitive)
        {
            Assert.IsType<SecureMemory<byte>>(lease);
        }
        else
        {
            Assert.IsNotType<SecureMemory<byte>>(lease);
        }

        Assert.InRange(lease.Memory.Length, minBufferSize, int.MaxValue);
        Assert.Equal(minBufferSize, buffer.Length);
    }
}
