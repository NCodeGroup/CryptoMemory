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
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace NCode.CryptoMemory;

/// <summary>
/// A high-performance class implementation of <see cref="IBufferWriter{T}"/> that writes to a fixed-size buffer.
/// Unlike growable buffer writers, this implementation cannot resize and will throw if the buffer capacity is exceeded.
/// </summary>
/// <typeparam name="T">The type of elements in the buffer.</typeparam>
/// <remarks>
/// <para>
/// This class is designed for scenarios where the maximum buffer size is known upfront and no heap allocations are desired
/// during write operations.
/// </para>
/// <para>
/// Unlike <see cref="FixedSpanBufferWriter{T}"/>, this class can be stored in fields and passed to async methods
/// because it uses <see cref="Memory{T}"/> instead of <see cref="Span{T}"/>.
/// </para>
/// </remarks>
[PublicAPI]
public class FixedMemoryBufferWriter<T> : IBufferWriter<T>
{
    private Memory<T> Buffer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedMemoryBufferWriter{T}"/> class with the specified buffer.
    /// </summary>
    /// <param name="buffer">The fixed-size buffer to write to.</param>
    public FixedMemoryBufferWriter(Memory<T> buffer)
    {
        Buffer = buffer;
    }

    /// <summary>
    /// Gets the total capacity of the underlying buffer.
    /// </summary>
    /// <value>The total number of elements that the buffer can hold.</value>
    public int Capacity => Buffer.Length;

    /// <summary>
    /// Gets the number of elements that have been written to the buffer.
    /// </summary>
    /// <value>The count of elements written so far.</value>
    public int WrittenCount { get; private set; }

    /// <summary>
    /// Gets the amount of free space remaining in the buffer.
    /// </summary>
    /// <value>The number of elements that can still be written before the buffer is full.</value>
    public int FreeCapacity => Buffer.Length - WrittenCount;

    /// <summary>
    /// Gets a <see cref="ReadOnlyMemory{T}"/> of the data that has been written to the buffer.
    /// </summary>
    /// <value>A read-only memory containing all written elements.</value>
    public ReadOnlyMemory<T> WrittenMemory
    {
        get
        {
            Debug.Assert(WrittenCount >= 0);
            return Buffer[..WrittenCount];
        }
    }

    /// <summary>
    /// Gets a <see cref="ReadOnlySpan{T}"/> of the data that has been written to the buffer.
    /// </summary>
    /// <value>A read-only span containing all written elements.</value>
    public ReadOnlySpan<T> WrittenSpan
    {
        get
        {
            Debug.Assert(WrittenCount >= 0);
            return Buffer.Span[..WrittenCount];
        }
    }

    /// <summary>
    /// Clears the written data by resetting the write position to zero.
    /// </summary>
    /// <remarks>
    /// This method does not clear the underlying buffer contents. To securely clear sensitive data,
    /// use <see cref="Reset"/> instead.
    /// </remarks>
    public void Clear()
    {
        Debug.Assert(WrittenCount >= 0);
        WrittenCount = 0;
    }

    /// <summary>
    /// Resets the buffer writer by clearing all written data and zeroing out the underlying buffer.
    /// </summary>
    /// <remarks>
    /// Use this method when the buffer may contain sensitive data that should be securely cleared.
    /// </remarks>
    public void Reset()
    {
        Debug.Assert(WrittenCount >= 0);
        Buffer.Span[..WrittenCount].Clear();
        WrittenCount = 0;
    }

    /// <summary>
    /// Advances the write position by the specified count.
    /// </summary>
    /// <param name="count">The number of elements that have been written to the buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="count"/> is negative.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Advancing by <paramref name="count"/> would exceed the buffer capacity.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        Debug.Assert(WrittenCount >= 0);

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "Count cannot be negative.");
        }

        if (WrittenCount + count > Buffer.Length)
        {
            throw new InvalidOperationException(
                $"Cannot advance past the end of the buffer. Current position: {WrittenCount}, Capacity: {Buffer.Length}, Requested advance: {count}.");
        }

        WrittenCount += count;
    }

    /// <summary>
    /// Returns a <see cref="Memory{T}"/> to write to that is at least the requested size.
    /// </summary>
    /// <param name="sizeHint">The minimum length of the returned <see cref="Memory{T}"/>. If 0, a non-empty buffer is returned if space is available.</param>
    /// <returns>A <see cref="Memory{T}"/> representing the available space for writing.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="sizeHint"/> is negative.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The requested size exceeds the available free capacity. This buffer cannot grow.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<T> GetMemory(int sizeHint = 0)
    {
        Debug.Assert(WrittenCount >= 0);

        if (sizeHint < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeHint), sizeHint, "Size hint cannot be negative.");
        }

        var freeCapacity = Buffer.Length - WrittenCount;

        if (sizeHint > freeCapacity)
        {
            throw new InvalidOperationException(
                $"The requested size ({sizeHint}) exceeds the available free capacity ({freeCapacity}). This buffer cannot grow.");
        }

        return Buffer[WrittenCount..];
    }

    /// <summary>
    /// Returns a <see cref="Span{T}"/> to write to that is at least the requested size.
    /// </summary>
    /// <param name="sizeHint">The minimum length of the returned <see cref="Span{T}"/>. If 0, a non-empty buffer is returned if space is available.</param>
    /// <returns>A <see cref="Span{T}"/> representing the available space for writing.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="sizeHint"/> is negative.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The requested size exceeds the available free capacity. This buffer cannot grow.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> GetSpan(int sizeHint = 0)
    {
        Debug.Assert(WrittenCount >= 0);

        if (sizeHint < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeHint), sizeHint, "Size hint cannot be negative.");
        }

        var freeCapacity = Buffer.Length - WrittenCount;

        if (sizeHint > freeCapacity)
        {
            throw new InvalidOperationException(
                $"The requested size ({sizeHint}) exceeds the available free capacity ({freeCapacity}). This buffer cannot grow.");
        }

        return Buffer.Span[WrittenCount..];
    }
}

