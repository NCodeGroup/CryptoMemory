#region Copyright Preamble

// Copyright @ 2024 NCode Group
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
using JetBrains.Annotations;

namespace NCode.CryptoMemory;

/// <summary>
/// Provides a resource pool that enables reusing instances of byte arrays that are pinned during their lifetime and securely zeroed when returned.
/// </summary>
[PublicAPI]
public static class CryptoPool
{
    /// <summary>
    /// Retrieves a buffer that is at least the requested length.
    /// </summary>
    /// <param name="minBufferSize">The minimum length of the buffer needed.</param>
    /// <param name="isSensitive">Indicates whether the buffer should be pinned during it's lifetime and securely zeroed when returned.
    /// When <c>false></c>, this implementation delegates to <c>MemoryPool&lt;byte&gt;.Shared.Rent</c>.</param>
    /// <param name="buffer">When this method returns, contains the buffer with the exact requested size.</param>
    /// <returns>
    /// An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.
    /// </returns>
    public static IMemoryOwner<byte> Rent(int minBufferSize, bool isSensitive, out Span<byte> buffer)
    {
        return CryptoPool<byte>.Rent(minBufferSize, isSensitive, out buffer);
    }

    /// <summary>
    /// Retrieves a buffer that is at least the requested length.
    /// </summary>
    /// <param name="minBufferSize">The minimum length of the buffer needed.</param>
    /// <param name="isSensitive">Indicates whether the buffer should be pinned during it's lifetime and securely zeroed when returned.
    /// When <c>false></c>, this implementation delegates to <c>MemoryPool&lt;byte&gt;.Shared.Rent</c>.</param>
    /// <param name="buffer">When this method returns, contains the buffer with the exact requested size.</param>
    /// <returns>
    /// An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.
    /// </returns>
    public static IMemoryOwner<byte> Rent(int minBufferSize, bool isSensitive, out Memory<byte> buffer)
    {
        return CryptoPool<byte>.Rent(minBufferSize, isSensitive, out buffer);
    }

    /// <summary>
    /// Creates a pinned byte array wrapped in a <see cref="SecureArrayLifetime{T}"/> that securely zeroes the memory upon disposal.
    /// </summary>
    /// <param name="length">The length of the array to allocate.</param>
    /// <returns>
    /// A <see cref="SecureArrayLifetime{T}"/> that manages the lifetime of the pinned array and ensures
    /// the memory is securely zeroed when disposed.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The returned array is allocated using <see cref="GC.AllocateUninitializedArray{T}(int, bool)"/>
    /// with pinned set to true, ensuring the garbage collector will not move it in memory.
    /// This is essential for cryptographic operations or interop scenarios.
    /// </para>
    /// <para>
    /// Since <see cref="SecureArrayLifetime{T}"/> is a ref struct, it can only be used on the stack
    /// and cannot be stored in fields of reference types.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// using var lifetime = CryptoPool.CreatePinnedArray(256);
    /// Span&lt;byte&gt; buffer = lifetime;
    /// // Use buffer for cryptographic operations
    /// // Memory is automatically zeroed when lifetime is disposed
    /// </code>
    /// </example>
    public static SecureArrayLifetime<byte> CreatePinnedArray(int length) =>
        SecureArrayLifetime<byte>.Create(length);
}

/// <summary>
/// Provides a resource pool that enables reusing instances of arrays that are pinned during their lifetime and securely zeroed when returned.
/// </summary>
[PublicAPI]
public static class CryptoPool<T>
{
    internal static MemoryPool<T> ChoosePool(bool isSensitive) =>
        isSensitive ? SecureMemoryPool<T>.Shared : MemoryPool<T>.Shared;

    /// <summary>
    /// Retrieves a buffer that is at least the requested length.
    /// </summary>
    /// <param name="minBufferSize">The minimum length of the buffer needed.</param>
    /// <param name="isSensitive">Indicates whether the buffer should be pinned during it's lifetime and securely zeroed when returned.
    /// When <c>false></c>, this implementation delegates to <c>MemoryPool&lt;&gt;.Shared.Rent</c>.</param>
    /// <param name="buffer">When this method returns, contains the buffer with the exact requested size.</param>
    /// <returns>
    /// An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.
    /// </returns>
    public static IMemoryOwner<T> Rent(int minBufferSize, bool isSensitive, out Span<T> buffer)
    {
        var pool = ChoosePool(isSensitive);
        var lease = pool.Rent(minBufferSize);
        buffer = lease.Memory.Span.Slice(0, Math.Min(lease.Memory.Length, minBufferSize));
        return lease;
    }

    /// <summary>
    /// Retrieves a buffer that is at least the requested length.
    /// </summary>
    /// <param name="minBufferSize">The minimum length of the buffer needed.</param>
    /// <param name="isSensitive">Indicates whether the buffer should be pinned during it's lifetime and securely zeroed when returned.
    /// When <c>false></c>, this implementation delegates to <c>MemoryPool&lt;&gt;.Shared.Rent</c>.</param>
    /// <param name="buffer">When this method returns, contains the buffer with the exact requested size.</param>
    /// <returns>
    /// An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.
    /// </returns>
    public static IMemoryOwner<T> Rent(int minBufferSize, bool isSensitive, out Memory<T> buffer)
    {
        var pool = ChoosePool(isSensitive);
        var lease = pool.Rent(minBufferSize);
        buffer = lease.Memory.Slice(0, Math.Min(lease.Memory.Length, minBufferSize));
        return lease;
    }
}
