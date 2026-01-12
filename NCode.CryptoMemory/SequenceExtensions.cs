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
using System.Diagnostics;
using JetBrains.Annotations;
using Nerdbank.Streams;

namespace NCode.CryptoMemory;

/// <summary>
/// Provides extension methods for <see cref="Sequence{T}"/> to enable secure memory operations.
/// </summary>
[PublicAPI]
public static class SequenceExtensions
{
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    extension<T>(Sequence<T> buffer)
    {
        /// <summary>
        /// Gets a <see cref="RefSpanLease{T}"/> that provides access to the underlying data as a contiguous <see cref="ReadOnlySpan{T}"/>.
        /// If the sequence is a single segment, the span is returned directly without allocation. Otherwise, the data is copied to a rented buffer from the crypto pool.
        /// </summary>
        /// <param name="isSensitive">
        /// <see langword="true"/> if the data is sensitive and should be securely cleared when disposed; otherwise, <see langword="false"/>.
        /// </param>
        /// <returns>
        /// A <see cref="RefSpanLease{T}"/> that provides access to the sequence data as a contiguous <see cref="ReadOnlySpan{T}"/>.
        /// The caller must dispose the lease to release the underlying resources.
        /// </returns>
        /// <remarks>
        /// <para>
        /// When the sequence consists of a single segment, the span is returned directly with the sequence as the owner,
        /// meaning no additional memory allocation occurs and the caller retains ownership of the original sequence.
        /// </para>
        /// <para>
        /// When the sequence spans multiple segments, a buffer is rented from <see cref="CryptoPool{T}"/> and the data is copied into it.
        /// If <paramref name="isSensitive"/> is <see langword="true"/>, the rented buffer will be securely cleared upon disposal.
        /// </para>
        /// </remarks>
        /// <exception cref="Exception">Any exception thrown during the copy operation will result in the rented buffer being disposed before re-throwing.</exception>
        [PublicAPI]
        public RefSpanLease<T> GetSpanLease(bool isSensitive)
        {
            var sequence = buffer.AsReadOnlySequence;
            if (sequence.IsSingleSegment)
            {
                return new RefSpanLease<T>(buffer, sequence.First.Span);
            }

            Debug.Assert(buffer.Length <= int.MaxValue, "Sequence length exceeds int.MaxValue.");
            var owner = CryptoPool<T>.Rent((int)buffer.Length, isSensitive, out Span<T> destination);
            try
            {
                sequence.CopyTo(destination);
                return new RefSpanLease<T>(owner, destination);
            }
            catch
            {
                owner.Dispose();
                throw;
            }
        }
    }
}
