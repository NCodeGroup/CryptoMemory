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
using System.ComponentModel;
using JetBrains.Annotations;
using Nerdbank.Streams;

namespace NCode.CryptoMemory;

/// <summary>
/// Provides a secure buffer writer that uses <see cref="SecureMemoryPool{T}"/> for memory allocation,
/// ensuring sensitive data is securely managed and cleared when disposed.
/// </summary>
/// <typeparam name="T">The type of elements in the buffer.</typeparam>
/// <remarks>
/// This class wraps a <see cref="Nerdbank.Streams.Sequence{T}"/> backed by <see cref="SecureMemoryPool{T}.Shared"/>
/// to provide automatic secure memory management. When disposed, all underlying memory buffers are securely
/// cleared to prevent sensitive data from lingering in memory.
/// </remarks>
[PublicAPI]
public sealed class SecureBufferWriter<T> : IBufferWriter<T>, IDisposable
{
    /// <summary>
    /// Gets the underlying <see cref="Nerdbank.Streams.Sequence{T}"/> used for buffering data.
    /// </summary>
    /// <remarks>
    /// This property is hidden from IntelliSense and is intended for advanced scenarios or internal use only.
    /// Direct manipulation of the sequence may bypass secure memory management features.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Sequence<T> Sequence { get; } = new(SecureMemoryPool<T>.Shared);

    /// <summary>
    /// Gets the total length of the data written to the buffer.
    /// </summary>
    /// <value>The number of elements that have been written to the buffer.</value>
    public long Length => AsReadOnlySequence.Length;

    /// <summary>
    /// Gets the written data as a <see cref="ReadOnlySequence{T}"/>.
    /// </summary>
    /// <value>A <see cref="ReadOnlySequence{T}"/> containing all data written to the buffer.</value>
    public ReadOnlySequence<T> AsReadOnlySequence => Sequence;

    /// <summary>
    /// Releases all resources used by the <see cref="SecureBufferWriter{T}"/> and securely clears the underlying memory.
    /// </summary>
    public void Dispose()
    {
        Sequence.Dispose();
    }

    /// <summary>
    /// Notifies the buffer writer that <paramref name="count"/> elements have been written to the output <see cref="Span{T}"/> or <see cref="Memory{T}"/>.
    /// </summary>
    /// <param name="count">The number of elements written to the <see cref="Span{T}"/> or <see cref="Memory{T}"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="count"/> is negative or greater than the size of the last requested buffer.
    /// </exception>
    public void Advance(int count) => Sequence.Advance(count);

    /// <summary>
    /// Returns a <see cref="Span{T}"/> to write to that is at least the requested size.
    /// </summary>
    /// <param name="sizeHint">The minimum length of the returned <see cref="Span{T}"/>. If 0, a non-empty buffer is returned.</param>
    /// <returns>A <see cref="Span{T}"/> of at least <paramref name="sizeHint"/> size. If <paramref name="sizeHint"/> is 0, returns a non-empty buffer.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="sizeHint"/> is negative.</exception>
    public Span<T> GetSpan(int sizeHint = 0) => Sequence.GetSpan(sizeHint);

    /// <summary>
    /// Returns a <see cref="Memory{T}"/> to write to that is at least the requested size.
    /// </summary>
    /// <param name="sizeHint">The minimum length of the returned <see cref="Memory{T}"/>. If 0, a non-empty buffer is returned.</param>
    /// <returns>A <see cref="Memory{T}"/> of at least <paramref name="sizeHint"/> size. If <paramref name="sizeHint"/> is 0, returns a non-empty buffer.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="sizeHint"/> is negative.</exception>
    public Memory<T> GetMemory(int sizeHint = 0) => Sequence.GetMemory(sizeHint);
}
