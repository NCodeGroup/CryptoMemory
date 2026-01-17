[![ci](https://github.com/NCodeGroup/CryptoMemory/actions/workflows/main.yml/badge.svg)](https://github.com/NCodeGroup/CryptoMemory/actions)

# NCode.CryptoMemory

> ⚠️ **DEPRECATED**: This project is deprecated and no longer maintained. Please use [NCode.Buffers](https://github.com/NCodeGroup/Buffers) instead, which combines functionality from multiple projects into one package with additional features.
>
> [![Nuget](https://img.shields.io/nuget/v/NCode.Buffers.svg)](https://www.nuget.org/packages/NCode.Buffers/)

Provides the ability to manage the lifetime of memory by pinning buffers to prevent duplicate
copies in ram and securely zeroing sensitive data when no longer needed. Also provides secure
encodings that throw an exception when invalid bytes are encountered.

## References
* [Pinned Object Heap (POH)](https://devblogs.microsoft.com/dotnet/internals-of-the-poh/)
* [Character Encoding with Exception Fallback](https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-encoding#exception-fallback)

## API

### SecureEncoding

```csharp
namespace NCode.CryptoMemory;

/// <summary>
/// Provides secure encodings that throw an exception when invalid bytes are encountered.
/// </summary>
public static class SecureEncoding
{
    /// <summary>
    /// Gets an ASCII encoding that throws an exception when invalid bytes are encountered.
    /// </summary>
    public static ASCIIEncoding ASCII { get; }

    /// <summary>
    /// Gets a UTF-8 encoding that throws an exception when invalid bytes are encountered.
    /// </summary>
    public static UTF8Encoding UTF8 { get; }
}
```

### CryptoPool

```csharp
namespace NCode.CryptoMemory;

/// <summary>
/// Provides a resource pool that enables reusing instances of byte arrays that are
/// pinned during their lifetime and securely zeroed when returned.
/// </summary>
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
    public static IMemoryOwner<byte> Rent(
        int minBufferSize,
        bool isSensitive,
        out Span<byte> buffer
    );

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
    public static IMemoryOwner<byte> Rent(
        int minBufferSize,
        bool isSensitive,
        out Memory<byte> buffer
    );

    /// <summary>
    /// Creates a pinned byte array wrapped in a <see cref="SecureArrayLifetime{T}"/> that securely zeroes the memory upon disposal.
    /// </summary>
    /// <param name="length">The length of the array to allocate.</param>
    /// <returns>
    /// A <see cref="SecureArrayLifetime{T}"/> that manages the lifetime of the pinned array and ensures
    /// the memory is securely zeroed when disposed.
    /// </returns>
    public static SecureArrayLifetime<byte> CreatePinnedArray(int length);
}
```

### SecureMemoryFactory

```csharp
namespace NCode.CryptoMemory;

/// <summary>
/// Provides factory methods for creating and renting secure memory buffers that can be pinned
/// during their lifetime and securely zeroed when disposed.
/// </summary>
public static class SecureMemoryFactory
{
    /// <summary>
    /// Retrieves a buffer that is at least the requested length.
    /// </summary>
    /// <typeparam name="T">The type of elements in the buffer.</typeparam>
    /// <param name="minBufferSize">The minimum length of the buffer needed.</param>
    /// <param name="isSensitive">Indicates whether the buffer should be pinned during it's lifetime and securely zeroed when returned.</param>
    /// <param name="buffer">When this method returns, contains the buffer with the exact requested size.</param>
    /// <returns>An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.</returns>
    public static IMemoryOwner<T> Rent<T>(int minBufferSize, bool isSensitive, out Span<T> buffer);

    /// <summary>
    /// Retrieves a buffer that is at least the requested length.
    /// </summary>
    /// <param name="minBufferSize">The minimum length of the buffer needed.</param>
    /// <param name="isSensitive">Indicates whether the buffer should be pinned during it's lifetime and securely zeroed when returned.</param>
    /// <param name="buffer">When this method returns, contains the buffer with the exact requested size.</param>
    /// <returns>An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.</returns>
    public static IMemoryOwner<byte> Rent(int minBufferSize, bool isSensitive, out Span<byte> buffer);

    /// <summary>
    /// Retrieves a buffer that is at least the requested length.
    /// </summary>
    /// <typeparam name="T">The type of elements in the buffer.</typeparam>
    /// <param name="minBufferSize">The minimum length of the buffer needed.</param>
    /// <param name="isSensitive">Indicates whether the buffer should be pinned during it's lifetime and securely zeroed when returned.</param>
    /// <param name="buffer">When this method returns, contains the buffer with the exact requested size.</param>
    /// <returns>An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.</returns>
    public static IMemoryOwner<T> Rent<T>(int minBufferSize, bool isSensitive, out Memory<T> buffer);

    /// <summary>
    /// Retrieves a buffer that is at least the requested length.
    /// </summary>
    /// <param name="minBufferSize">The minimum length of the buffer needed.</param>
    /// <param name="isSensitive">Indicates whether the buffer should be pinned during it's lifetime and securely zeroed when returned.</param>
    /// <param name="buffer">When this method returns, contains the buffer with the exact requested size.</param>
    /// <returns>An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.</returns>
    public static IMemoryOwner<byte> Rent(int minBufferSize, bool isSensitive, out Memory<byte> buffer);

    /// <summary>
    /// Creates a pinned array of the specified type wrapped in a <see cref="SecureArrayLifetime{T}"/> that securely zeroes the memory upon disposal.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array. Must be an unmanaged value type.</typeparam>
    /// <param name="length">The length of the array to allocate.</param>
    /// <returns>A <see cref="SecureArrayLifetime{T}"/> that manages the lifetime of the pinned array.</returns>
    public static SecureArrayLifetime<T> CreatePinnedArray<T>(int length) where T : struct;

    /// <summary>
    /// Creates a pinned byte array wrapped in a <see cref="SecureArrayLifetime{T}"/> that securely zeroes the memory upon disposal.
    /// </summary>
    /// <param name="length">The length of the array to allocate.</param>
    /// <returns>A <see cref="SecureArrayLifetime{T}"/> that manages the lifetime of the pinned array.</returns>
    public static SecureArrayLifetime<byte> CreatePinnedArray(int length);

    /// <summary>
    /// Creates a new <see cref="SecureBufferWriter{T}"/> for building sequences of data that will be securely zeroed when disposed.
    /// </summary>
    /// <typeparam name="T">The type of elements in the buffer.</typeparam>
    /// <param name="minimumSpanLength">The minimum length for each span segment allocated by the buffer writer.</param>
    /// <returns>A new <see cref="SecureBufferWriter{T}"/> instance.</returns>
    public static SecureBufferWriter<T> CreateSecureBuffer<T>(int minimumSpanLength = 0);

    /// <summary>
    /// Creates a new <see cref="SecureBufferWriter{T}"/> for building sequences of byte data that will be securely zeroed when disposed.
    /// </summary>
    /// <param name="minimumSpanLength">The minimum length for each span segment allocated by the buffer writer.</param>
    /// <returns>A new <see cref="SecureBufferWriter{T}"/> instance.</returns>
    public static SecureBufferWriter<byte> CreateSecureBuffer(int minimumSpanLength = 0);
}
```

### SecureSpanLifetime

```csharp
namespace NCode.CryptoMemory;

/// <summary>
/// A ref struct that wraps a <see cref="Span{T}"/> and securely zeroes its memory upon disposal.
/// This struct assumes lifecycle ownership of the span, ensuring sensitive data is cleared
/// when the lifetime ends.
/// </summary>
/// <typeparam name="T">The type of elements in the span. Must be an unmanaged value type.</typeparam>
public readonly ref struct SecureSpanLifetime<T> : IDisposable
    where T : struct
{
    /// <summary>
    /// Gets the underlying span managed by this lifetime.
    /// </summary>
    public Span<T> Span { get; }

    /// <summary>
    /// Implicitly converts a <see cref="SecureSpanLifetime{T}"/> to its underlying <see cref="Span{T}"/>.
    /// </summary>
    public static implicit operator Span<T>(SecureSpanLifetime<T> lifetime);

    /// <summary>
    /// Securely zeroes the memory of the underlying span using cryptographic memory clearing.
    /// </summary>
    public void Dispose();
}
```

### SecureArrayLifetime

```csharp
namespace NCode.CryptoMemory;

/// <summary>
/// A ref struct that allocates a pinned array and securely zeroes its memory upon disposal.
/// This struct assumes lifecycle ownership of the allocated array, ensuring sensitive data
/// is cleared when the lifetime ends.
/// </summary>
/// <typeparam name="T">The type of elements in the array. Must be an unmanaged value type.</typeparam>
public readonly ref struct SecureArrayLifetime<T> : IDisposable
    where T : struct
{
    /// <summary>
    /// Creates a new <see cref="SecureArrayLifetime{T}"/> with the specified length.
    /// </summary>
    /// <param name="length">The length of the array to allocate.</param>
    /// <returns>A new <see cref="SecureArrayLifetime{T}"/> instance.</returns>
    public static SecureArrayLifetime<T> Create(int length);

    /// <summary>
    /// Gets the pinned array managed by this lifetime.
    /// </summary>
    public T[] PinnedArray { get; }

    /// <summary>
    /// Gets the length of the pinned array.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Implicitly converts a <see cref="SecureArrayLifetime{T}"/> to a <typeparamref name="T"/> array.
    /// </summary>
    public static implicit operator T[](SecureArrayLifetime<T> lifetime);

    /// <summary>
    /// Implicitly converts a <see cref="SecureArrayLifetime{T}"/> to a <see cref="Span{T}"/>.
    /// </summary>
    public static implicit operator Span<T>(SecureArrayLifetime<T> lifetime);

    /// <summary>
    /// Securely zeroes the memory of the pinned array using cryptographic memory clearing.
    /// </summary>
    public void Dispose();
}
```

### SecureBufferWriter

```csharp
namespace NCode.CryptoMemory;

/// <summary>
/// Provides a secure buffer writer that uses <see cref="SecureMemoryPool{T}"/> for memory allocation,
/// ensuring sensitive data is securely managed and cleared when disposed.
/// </summary>
public class SecureBufferWriter<T> : IBufferWriter<T>, IDisposable
{
    /// <summary>
    /// Gets the length of the written data.
    /// </summary>
    public long Length { get; }

    /// <summary>
    /// Gets the written data as a read-only sequence.
    /// </summary>
    public ReadOnlySequence<T> AsReadOnlySequence { get; }

    /// <summary>
    /// Converts a <see cref="SecureBufferWriter{T}"/> to the underlying <see cref="Sequence{T}"/>.
    /// </summary>
    /// <param name="buffer">The buffer to convert. May be <see langword="null"/>.</param>
    /// <returns>
    /// The underlying <see cref="Sequence{T}"/> used for buffering data,
    /// or <see langword="null"/> if <paramref name="buffer"/> is <see langword="null"/>.
    /// </returns>
    public static implicit operator Sequence<T>?(SecureBufferWriter<T>? buffer);

    /// <summary>
    /// Converts a <see cref="SecureBufferWriter{T}"/> to a <see cref="ReadOnlySequence{T}"/>.
    /// </summary>
    /// <param name="buffer">The buffer to convert. May be <see langword="null"/>.</param>
    /// <returns>
    /// A <see cref="ReadOnlySequence{T}"/> containing all data written to the buffer,
    /// or <see cref="ReadOnlySequence{T}.Empty"/> if <paramref name="buffer"/> is <see langword="null"/>.
    /// </returns>
    public static implicit operator ReadOnlySequence<T>(SecureBufferWriter<T>? buffer);

    /// <summary>
    /// Disposes the buffer writer and releases all associated memory.
    /// </summary>
    public void Dispose();

    /// <summary>
    /// Advances the writer by the specified number of elements.
    /// </summary>
    /// <param name="count">The number of elements written.</param>
    public void Advance(int count);

    /// <summary>
    /// Returns a <see cref="Span{T}"/> to write to that is at least the requested size.
    /// </summary>
    /// <param name="sizeHint">The minimum length of the returned span. If 0, a non-empty buffer is returned.</param>
    /// <returns>A span of at least <paramref name="sizeHint"/> in length.</returns>
    public Span<T> GetSpan(int sizeHint = 0);

    /// <summary>
    /// Returns a <see cref="Memory{T}"/> to write to that is at least the requested size.
    /// </summary>
    /// <param name="sizeHint">The minimum length of the returned memory. If 0, a non-empty buffer is returned.</param>
    /// <returns>A memory block of at least <paramref name="sizeHint"/> in length.</returns>
    public Memory<T> GetMemory(int sizeHint = 0);
}
```

### RefSpanLease

```csharp
namespace NCode.CryptoMemory;

/// <summary>
/// Represents a ref struct that holds a leased <see cref="ReadOnlySpan{T}"/> and manages the lifetime of its underlying owner.
/// When disposed, the owner is also disposed, releasing the leased memory back to its source.
/// </summary>
/// <typeparam name="T">The type of elements in the span.</typeparam>
public readonly ref struct RefSpanLease<T> : IDisposable
{
    /// <summary>
    /// Gets the leased <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public ReadOnlySpan<T> Span { get; }

    /// <summary>
    /// Disposes the underlying owner, releasing the leased memory back to its source.
    /// </summary>
    public void Dispose();
}
```

### SequenceExtensions

```csharp
namespace NCode.CryptoMemory;

/// <summary>
/// Provides extension methods for <see cref="Sequence{T}"/> to enable secure memory operations.
/// </summary>
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
        public RefSpanLease<T> GetSpanLease(bool isSensitive);

        /// <summary>
        /// Consumes the sequence and returns an <see cref="IDisposable"/> owner along with a contiguous <see cref="ReadOnlySpan{T}"/> of the data.
        /// This method transfers ownership of the underlying buffer to the caller and disposes the original sequence.
        /// </summary>
        /// <param name="isSensitive">
        /// <see langword="true"/> if the data is sensitive and should be securely cleared when disposed; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="span">
        /// When this method returns, contains a <see cref="ReadOnlySpan{T}"/> representing the contiguous data from the sequence.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> that owns the underlying buffer. The caller must dispose this owner to release the underlying resources.
        /// For single-segment sequences, this is the original sequence buffer.
        /// For multi-segment sequences, this is a rented buffer from <see cref="CryptoPool{T}"/>.
        /// </returns>
        public IDisposable ConsumeAsContiguousSpan(bool isSensitive, out ReadOnlySpan<T> span);
    }
}
```

### FixedSpanBufferWriter

```csharp
namespace NCode.CryptoMemory;

/// <summary>
/// A high-performance ref struct implementation of <see cref="IBufferWriter{T}"/> that writes to a fixed-size buffer.
/// Unlike growable buffer writers, this implementation cannot resize and will throw if the buffer capacity is exceeded.
/// </summary>
/// <typeparam name="T">The type of elements in the buffer.</typeparam>
public ref struct FixedSpanBufferWriter<T> : IBufferWriter<T>
{
    /// <summary>
    /// Gets the total capacity of the underlying buffer.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets the number of elements that have been written to the buffer.
    /// </summary>
    public int WrittenCount { get; }

    /// <summary>
    /// Gets the amount of free space remaining in the buffer.
    /// </summary>
    public int FreeCapacity { get; }

    /// <summary>
    /// Gets a <see cref="ReadOnlySpan{T}"/> of the data that has been written to the buffer.
    /// </summary>
    public ReadOnlySpan<T> WrittenSpan { get; }

    /// <summary>
    /// Clears the written data by resetting the write position to zero.
    /// Does not clear the underlying buffer contents.
    /// </summary>
    public void Clear();

    /// <summary>
    /// Resets the buffer writer by clearing all written data and zeroing out the underlying buffer.
    /// Use this method when the buffer may contain sensitive data that should be securely cleared.
    /// </summary>
    public void Reset();

    /// <summary>
    /// Advances the write position by the specified count.
    /// </summary>
    /// <param name="count">The number of elements that have been written to the buffer.</param>
    public void Advance(int count);

    /// <summary>
    /// Returns a <see cref="Span{T}"/> to write to that is at least the requested size.
    /// </summary>
    /// <param name="sizeHint">The minimum length of the returned span. If 0, a non-empty buffer is returned if space is available.</param>
    /// <returns>A span representing the available space for writing.</returns>
    public Span<T> GetSpan(int sizeHint = 0);

    /// <summary>
    /// This method is not supported. Use <see cref="GetSpan"/> instead.
    /// </summary>
    public Memory<T> GetMemory(int sizeHint = 0);
}
```

### FixedMemoryBufferWriter

```csharp
namespace NCode.CryptoMemory;

/// <summary>
/// A high-performance class implementation of <see cref="IBufferWriter{T}"/> that writes to a fixed-size buffer.
/// Unlike growable buffer writers, this implementation cannot resize and will throw if the buffer capacity is exceeded.
/// </summary>
/// <typeparam name="T">The type of elements in the buffer.</typeparam>
/// <remarks>
/// Unlike <see cref="FixedSpanBufferWriter{T}"/>, this class can be stored in fields and passed to async methods
/// because it uses <see cref="Memory{T}"/> instead of <see cref="Span{T}"/>.
/// </remarks>
public class FixedMemoryBufferWriter<T> : IBufferWriter<T>
{
    /// <summary>
    /// Gets the total capacity of the underlying buffer.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets the number of elements that have been written to the buffer.
    /// </summary>
    public int WrittenCount { get; }

    /// <summary>
    /// Gets the amount of free space remaining in the buffer.
    /// </summary>
    public int FreeCapacity { get; }

    /// <summary>
    /// Gets a <see cref="ReadOnlyMemory{T}"/> of the data that has been written to the buffer.
    /// </summary>
    public ReadOnlyMemory<T> WrittenMemory { get; }

    /// <summary>
    /// Gets a <see cref="ReadOnlySpan{T}"/> of the data that has been written to the buffer.
    /// </summary>
    public ReadOnlySpan<T> WrittenSpan { get; }

    /// <summary>
    /// Clears the written data by resetting the write position to zero.
    /// Does not clear the underlying buffer contents.
    /// </summary>
    public void Clear();

    /// <summary>
    /// Resets the buffer writer by clearing all written data and zeroing out the underlying buffer.
    /// Use this method when the buffer may contain sensitive data that should be securely cleared.
    /// </summary>
    public void Reset();

    /// <summary>
    /// Advances the write position by the specified count.
    /// </summary>
    /// <param name="count">The number of elements that have been written to the buffer.</param>
    public void Advance(int count);

    /// <summary>
    /// Returns a <see cref="Span{T}"/> to write to that is at least the requested size.
    /// </summary>
    /// <param name="sizeHint">The minimum length of the returned span. If 0, a non-empty buffer is returned if space is available.</param>
    /// <returns>A span representing the available space for writing.</returns>
    public Span<T> GetSpan(int sizeHint = 0);

    /// <summary>
    /// Returns a <see cref="Memory{T}"/> to write to that is at least the requested size.
    /// </summary>
    /// <param name="sizeHint">The minimum length of the returned memory. If 0, a non-empty buffer is returned if space is available.</param>
    /// <returns>A memory block representing the available space for writing.</returns>
    public Memory<T> GetMemory(int sizeHint = 0);
}
```

### BufferExtensions

```csharp
namespace NCode.CryptoMemory;

/// <summary>
/// Provides extension methods for <see cref="Span{T}"/> and <see cref="Memory{T}"/> to enable buffer writing operations.
/// </summary>
public static class BufferExtensions
{
    /// <summary>
    /// Wraps this span in a <see cref="SecureSpanLifetime{T}"/> that securely zeroes the memory upon disposal.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The span to wrap.</param>
    /// <returns>A <see cref="SecureSpanLifetime{T}"/> that manages the lifetime of this span.</returns>
    public static SecureSpanLifetime<T> GetSecureLifetime<T>(this Span<T> span) where T : struct;

    /// <summary>
    /// Creates a <see cref="FixedSpanBufferWriter{T}"/> that writes to this span.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The span to write to.</param>
    /// <returns>A <see cref="FixedSpanBufferWriter{T}"/> that can be used to write data to the span.</returns>
    public static FixedSpanBufferWriter<T> GetFixedBufferWriter<T>(this Span<T> span);

    /// <summary>
    /// Creates a <see cref="FixedMemoryBufferWriter{T}"/> that writes to this memory.
    /// </summary>
    /// <typeparam name="T">The type of elements in the memory.</typeparam>
    /// <param name="memory">The memory to write to.</param>
    /// <returns>A <see cref="FixedMemoryBufferWriter{T}"/> that can be used to write data to the memory.</returns>
    public static FixedMemoryBufferWriter<T> GetFixedBufferWriter<T>(this Memory<T> memory);
}
```

## Release Notes

* v1.0.0 - Initial release
* v1.0.1 - Updating readme
* v2.0.0 - Net8 upgrade. Refactored to use SecureMemoryPool. Added SecureEncoding. Removed HeapMemoryManager.
* v2.1.0 - Net10 upgrade. Added SecureBufferWriter.
* v2.1.1 - Fixing CI build
* v2.2.0 - Added RefSpanLease and SequenceExtensions for secure span operations on sequences.
* v2.3.0 - Added FixedSpanBufferWriter and BufferExtensions for high-performance fixed-size buffer writing.
* v2.4.0 - Added FixedMemoryBufferWriter for async-compatible fixed-size buffer writing. Renamed SpanExtensions to BufferExtensions.
* v2.5.0 - Added implicit conversion operators on SecureBufferWriter to Sequence<T> and ReadOnlySequence<T>.
* v2.6.0 - Updated the ownership for RefSpanLease when a single segment is returned.
* v2.7.0 - Added ConsumeAsContiguousSpan extension method for ownership-transferring span extraction from sequences.
* v2.8.0 - Added SecureSpanLifetime and SecureArrayLifetime ref structs for secure memory lifetime management. Added GetSecureLifetime extension method and CryptoPool.CreatePinnedArray for creating pinned arrays with secure disposal.
* v3.0.0 - Added SecureMemoryFactory as a unified API for secure memory operations. Added generic Rent<T> and CreatePinnedArray<T> methods for working with any struct type.
* v3.1.0 - Added additional implicit conversion operators on SecureArrayLifetime for easier usage.
* v3.2.0 - Added Length property on SecureArrayLifetime for easier usage.
