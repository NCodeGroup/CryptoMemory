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

using JetBrains.Annotations;

namespace NCode.CryptoMemory;

/// <summary>
/// Provides extension methods for <see cref="Span{T}"/> to enable buffer writing operations.
/// </summary>
[PublicAPI]
public static class SpanExtensions
{
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    extension<T>(Span<T> span)
    {
        /// <summary>
        /// Creates a <see cref="RefFixedBufferWriter{T}"/> that writes to this span.
        /// </summary>
        /// <returns>
        /// A <see cref="RefFixedBufferWriter{T}"/> that can be used to write data to the span.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The returned buffer writer has a fixed capacity equal to the length of this span and cannot grow.
        /// </para>
        /// <para>
        /// Since <see cref="RefFixedBufferWriter{T}"/> is a ref struct, it can only be used on the stack
        /// and cannot be stored in fields of reference types.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// Span&lt;byte&gt; buffer = stackalloc byte[100];
        /// var writer = buffer.GetFixedBufferWriter();
        /// var span = writer.GetSpan(10);
        /// // Write data to span...
        /// writer.Advance(10);
        /// </code>
        /// </example>
        [PublicAPI]
        public RefFixedBufferWriter<T> GetFixedBufferWriter()
        {
            return new RefFixedBufferWriter<T>(span);
        }
    }
}
