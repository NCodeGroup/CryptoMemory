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
using JetBrains.Annotations;
using Nerdbank.Streams;

namespace NCode.CryptoMemory;

/// <summary>
/// Provides a secure buffer writer that uses <see cref="SecureMemoryPool{T}"/> for memory allocation,
/// ensuring sensitive data is securely managed and cleared when disposed.
/// </summary>
[PublicAPI]
public class SecureBufferWriter<T> : IBufferWriter<T>, IDisposable
{
    internal Sequence<T> Sequence { get; } = new(SecureMemoryPool<T>.Shared);

    /// <summary>
    /// Gets the length of the written data.
    /// </summary>
    public long Length => AsReadOnlySequence.Length;

    /// <summary>
    /// Gets the written data as a read-only sequence.
    /// </summary>
    public ReadOnlySequence<T> AsReadOnlySequence => Sequence;

    /// <inheritdoc />
    public void Dispose()
    {
        Sequence.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public void Advance(int count) => Sequence.Advance(count);

    /// <inheritdoc />
    public Span<T> GetSpan(int sizeHint = 0) => Sequence.GetSpan(sizeHint);

    /// <inheritdoc />
    public Memory<T> GetMemory(int sizeHint = 0) => Sequence.GetMemory(sizeHint);
}
