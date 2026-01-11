[![ci](https://github.com/NCodeGroup/CryptoMemory/actions/workflows/main.yml/badge.svg)](https://github.com/NCodeGroup/CryptoMemory/actions)

# NCode.CryptoMemory

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
    /// <summary>
    /// Gets a <see cref="RefSpanLease{T}"/> that provides access to the underlying data as a contiguous <see cref="ReadOnlySpan{T}"/>.
    /// If the sequence is a single segment, the span is returned directly without allocation. Otherwise, the data is copied to a rented buffer from the crypto pool.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="sequence">The sequence to convert.</param>
    /// <param name="isSensitive">
    /// <see langword="true"/> if the data is sensitive and should be securely cleared when disposed; otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>
    /// A <see cref="RefSpanLease{T}"/> that provides access to the sequence data as a contiguous span.
    /// The caller must dispose the lease to release the underlying resources.
    /// </returns>
    public static RefSpanLease<T> GetSpanLease<T>(this Sequence<T> sequence, bool isSensitive);
}
```

## Release Notes

* v1.0.0 - Initial release
* v1.0.1 - Updating readme
* v2.0.0 - Net8 upgrade. Refactored to use SecureMemoryPool. Added SecureEncoding. Removed HeapMemoryManager.
* v2.1.0 - Net10 upgrade. Added SecureBufferWriter.
* v2.1.1 - Fixing CI build
* v2.2.0 - Added RefSpanLease and SequenceExtensions for secure span operations on sequences.
