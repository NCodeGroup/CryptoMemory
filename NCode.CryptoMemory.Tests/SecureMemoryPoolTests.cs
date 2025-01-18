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

namespace NCode.CryptoMemory.Tests;

public class SecureMemoryPoolTests
{
    [Fact]
    public void DefaultHighPressureThreshold_Valid()
    {
        const double result = SecureMemoryPool<byte>.DefaultHighPressureThreshold;
        Assert.Equal(0.90, result);
    }

    [Fact]
    public void PageSize_Valid()
    {
        const int result = SecureMemoryPool<byte>.PageSize;
        Assert.Equal(4096, result);
    }

    [Fact]
    public void HighPressureThreshold_Valid()
    {
        using var pool = new SecureMemoryPool<byte>();
        Assert.Equal(0.90, pool.HighPressureThreshold);
        pool.HighPressureThreshold = 0.80;
        Assert.Equal(0.80, pool.HighPressureThreshold);
    }

    [Fact]
    public void MaxBufferSize_Valid()
    {
        using var pool = new SecureMemoryPool<byte>();
        Assert.Equal(Array.MaxLength, pool.MaxBufferSize);
    }

    [Fact]
    public void Dispose_Valid()
    {
        var pool = new SecureMemoryPool<byte>();

        pool.MemoryQueue.Enqueue(new SecureMemory<byte>(pool, 1024));
        Assert.NotEmpty(pool.MemoryQueue);

        pool.Dispose();

        Assert.True(pool.IsDisposed);
        Assert.Empty(pool.MemoryQueue);
    }

    [Fact]
    public void Rent_Disposed()
    {
        var pool = new SecureMemoryPool<byte>();
        pool.Dispose();

        Assert.Throws<ObjectDisposedException>(() =>
            pool.Rent(0));
    }

    [Fact]
    public void Rent_SizeLessThanNegativeOne()
    {
        using var pool = new SecureMemoryPool<byte>();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            pool.Rent(-2));
    }

    [Fact]
    public void Rent_SizeZero()
    {
        using var pool = new SecureMemoryPool<byte>();

        using var lease = pool.Rent(0);

        Assert.Same(EmptyMemory<byte>.Singleton, lease);
    }

    [Fact]
    public void Rent_SizeLessThanPage()
    {
        using var pool = new SecureMemoryPool<byte>();

        const int requestedSize = SecureMemoryPool<byte>.PageSize - 1;
        using var lease = pool.Rent(requestedSize);

        Assert.Equal(SecureMemoryPool<byte>.PageSize, lease.Memory.Length);
    }

    [Fact]
    public void Rent_SizeMoreThanPage()
    {
        using var pool = new SecureMemoryPool<byte>();

        const int requestedSize = SecureMemoryPool<byte>.PageSize + 1;
        using var lease = pool.Rent(requestedSize);

        Assert.Equal(requestedSize, lease.Memory.Length);
    }

    [Fact]
    public void Rent_LeaseReused()
    {
        using var pool = new SecureMemoryPool<byte>();

        const int requestedSize = SecureMemoryPool<byte>.PageSize - 1;
        var lease1 = pool.Rent(requestedSize);
        lease1.Dispose();

        using var lease2 = pool.Rent(requestedSize);
        Assert.Same(lease1, lease2);
    }

    [Fact]
    public void Return_Valid()
    {
        using var pool = new SecureMemoryPool<byte>();
        Assert.Empty(pool.MemoryQueue);

        using var lease = new SecureMemory<byte>(null, 1024);

        pool.Return(lease);
        Assert.Single(pool.MemoryQueue, lease);
    }

    [Fact]
    public void Return_Disposed()
    {
        var pool = new SecureMemoryPool<byte>();
        Assert.Empty(pool.MemoryQueue);

        pool.Dispose();

        using var lease = new SecureMemory<byte>(null, 1024);

        pool.Return(lease);
        Assert.Empty(pool.MemoryQueue);
    }

    [Fact]
    public void TrimMemory_Valid()
    {
        using var pool = new SecureMemoryPool<byte>();

        pool.MemoryQueue.Enqueue(new SecureMemory<byte>(pool, 1024));
        Assert.NotEmpty(pool.MemoryQueue);

        pool.HighPressureThreshold = 0.0;
        pool.TrimMemory();

        Assert.Empty(pool.MemoryQueue);
    }
}
