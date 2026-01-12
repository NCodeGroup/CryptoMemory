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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace NCode.CryptoMemory;

/// <summary>
/// Provides factory methods for creating and renting secure memory buffers that can be pinned
/// during their lifetime and securely zeroed when disposed.
/// </summary>
/// <remarks>
/// <para>
/// This factory provides a unified API for working with secure memory in cryptographic scenarios.
/// It offers methods to rent buffers from a pool or create pinned arrays with automatic secure disposal.
/// </para>
/// <para>
/// When <c>isSensitive</c> is <c>true</c>, buffers are:
/// <list type="bullet">
/// <item><description>Pinned in memory to prevent the garbage collector from moving them</description></item>
/// <item><description>Securely zeroed using <see cref="System.Security.Cryptography.CryptographicOperations.ZeroMemory"/> when returned</description></item>
/// </list>
/// </para>
/// <para>
/// When <c>isSensitive</c> is <c>false</c>, this implementation delegates to <see cref="System.Buffers.MemoryPool{T}.Shared"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Rent a sensitive buffer
/// using var owner = SecureMemoryFactory.Rent(256, isSensitive: true, out Span&lt;byte&gt; buffer);
/// // Use buffer for cryptographic operations
/// // Memory is automatically zeroed when owner is disposed
///
/// // Create a pinned array
/// using var lifetime = SecureMemoryFactory.CreatePinnedArray(128);
/// Span&lt;byte&gt; pinnedBuffer = lifetime;
/// // Memory is automatically zeroed when lifetime is disposed
///
/// // Create a secure buffer writer
/// using var writer = SecureMemoryFactory.CreateSecureBuffer&lt;byte&gt;();
/// // Write data to the buffer
/// // All segments are securely zeroed when writer is disposed
/// </code>
/// </example>
[PublicAPI]
[ExcludeFromCodeCoverage]
public static class SecureMemoryFactory
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
    public static IMemoryOwner<T> Rent<T>(int minBufferSize, bool isSensitive, out Span<T> buffer)
        => CryptoPool<T>.Rent(minBufferSize, isSensitive, out buffer);

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
        => CryptoPool<byte>.Rent(minBufferSize, isSensitive, out buffer);

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
    public static IMemoryOwner<T> Rent<T>(int minBufferSize, bool isSensitive, out Memory<T> buffer)
        => CryptoPool<T>.Rent(minBufferSize, isSensitive, out buffer);

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
        => CryptoPool<byte>.Rent(minBufferSize, isSensitive, out buffer);

    /// <summary>
    /// Creates a pinned array of the specified type wrapped in a <see cref="SecureArrayLifetime{T}"/> that securely zeroes the memory upon disposal.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array. Must be an unmanaged value type.</typeparam>
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
    /// using var lifetime = SecureMemoryFactory.CreatePinnedArray&lt;int&gt;(64);
    /// Span&lt;int&gt; buffer = lifetime;
    /// // Use buffer for operations requiring pinned memory
    /// // Memory is automatically zeroed when lifetime is disposed
    /// </code>
    /// </example>
    public static SecureArrayLifetime<T> CreatePinnedArray<T>(int length)
        where T : struct
        => SecureArrayLifetime<T>.Create(length);

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
    /// This is a convenience overload for the common case of working with byte arrays.
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
    /// using var lifetime = SecureMemoryFactory.CreatePinnedArray(256);
    /// Span&lt;byte&gt; buffer = lifetime;
    /// // Use buffer for cryptographic operations
    /// // Memory is automatically zeroed when lifetime is disposed
    /// </code>
    /// </example>
    public static SecureArrayLifetime<byte> CreatePinnedArray(int length)
        => SecureArrayLifetime<byte>.Create(length);

    /// <summary>
    /// Creates a new <see cref="SecureBufferWriter{T}"/> for building sequences of data that will be securely zeroed when disposed.
    /// </summary>
    /// <typeparam name="T">The type of elements in the buffer.</typeparam>
    /// <param name="minimumSpanLength">The minimum length for each span segment allocated by the buffer writer. Default is 0, which uses the default segment size.</param>
    /// <returns>
    /// A new <see cref="SecureBufferWriter{T}"/> instance that can be used to write data and will securely zero all memory when disposed.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The returned buffer writer allocates memory from a secure memory pool and ensures all segments
    /// are securely zeroed using <see cref="System.Security.Cryptography.CryptographicOperations.ZeroMemory"/> when disposed.
    /// </para>
    /// <para>
    /// Use this method when you need to build up a sequence of sensitive data incrementally.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// using var writer = SecureMemoryFactory.CreateSecureBuffer&lt;byte&gt;(minimumSpanLength: 256);
    /// var span = writer.GetSpan(100);
    /// // Write data to span
    /// writer.Advance(100);
    /// // All memory is securely zeroed when writer is disposed
    /// </code>
    /// </example>
    public static SecureBufferWriter<T> CreateSecureBuffer<T>(int minimumSpanLength = 0)
        => new()
        {
            MinimumSpanLength = minimumSpanLength
        };

    /// <summary>
    /// Creates a new <see cref="SecureBufferWriter{T}"/> for building sequences of byte data that will be securely zeroed when disposed.
    /// </summary>
    /// <param name="minimumSpanLength">The minimum length for each span segment allocated by the buffer writer. Default is 0, which uses the default segment size.</param>
    /// <returns>
    /// A new <see cref="SecureBufferWriter{T}"/> instance that can be used to write byte data and will securely zero all memory when disposed.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is a convenience overload for the common case of working with byte buffers.
    /// The returned buffer writer allocates memory from a secure memory pool and ensures all segments
    /// are securely zeroed using <see cref="System.Security.Cryptography.CryptographicOperations.ZeroMemory"/> when disposed.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// using var writer = SecureMemoryFactory.CreateSecureBuffer(minimumSpanLength: 256);
    /// var span = writer.GetSpan(100);
    /// // Write sensitive byte data to span
    /// writer.Advance(100);
    /// // All memory is securely zeroed when writer is disposed
    /// </code>
    /// </example>
    public static SecureBufferWriter<byte> CreateSecureBuffer(int minimumSpanLength = 0)
        => new()
        {
            MinimumSpanLength = minimumSpanLength
        };
}
