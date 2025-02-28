﻿#region Copyright Preamble

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
    internal static MemoryPool<byte> ChoosePool(bool isSensitive) =>
        isSensitive ? SecureMemoryPool<byte>.Shared : MemoryPool<byte>.Shared;

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
        var pool = ChoosePool(isSensitive);
        var lease = pool.Rent(minBufferSize);
        buffer = lease.Memory.Span[..minBufferSize];
        return lease;
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
        var pool = ChoosePool(isSensitive);
        var lease = pool.Rent(minBufferSize);
        buffer = lease.Memory[..minBufferSize];
        return lease;
    }
}
