[![ci](https://github.com/NCodeGroup/CryptoMemory/actions/workflows/main.yml/badge.svg)](https://github.com/NCodeGroup/CryptoMemory/actions)

# NCode.CryptoMemory

Provides the ability to manage the lifetime of memory by pinning buffers to prevent duplicate
copies in ram and securely zeroing sensitive data when no longer needed.

## API

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
    /// <returns>
    /// An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.
    /// </returns>
    public static IMemoryOwner<byte> Rent(
        int minBufferSize);

    /// <summary>
    /// Retrieves a buffer that is at least the requested length.
    /// </summary>
    /// <param name="minBufferSize">The minimum length of the buffer needed.</param>
    /// <param name="isSensitive">Indicates whether the buffer should be pinned during it's lifetime
    /// and securely zeroed when returned. When <c>false></c>, this implementation delegates to
    /// <c>MemoryPool&lt;byte&gt;.Shared.Rent</c>.</param>
    /// <returns>
    /// An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.
    /// </returns>
    public static IMemoryOwner<byte> Rent(
        int minBufferSize,
        bool isSensitive);

    /// <summary>
    /// Retrieves a buffer that is at least the requested length.
    /// </summary>
    /// <param name="minBufferSize">The minimum length of the buffer needed.</param>
    /// <param name="buffer">When this method returns, contains the buffer with the exact
    /// requested size.</param>
    /// <returns>
    /// An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.
    /// </returns>
    public static IMemoryOwner<byte> Rent(
        int minBufferSize,
        out Span<byte> buffer);

    /// <summary>
    /// Retrieves a buffer that is at least the requested length.
    /// </summary>
    /// <param name="minBufferSize">The minimum length of the buffer needed.</param>
    /// <param name="isSensitive">Indicates whether the buffer should be pinned during it's lifetime
    /// and securely zeroed when returned. When <c>false></c>, this implementation delegates to
    /// <c>MemoryPool&lt;byte&gt;.Shared.Rent</c>.</param>
    /// <param name="buffer">When this method returns, contains the buffer with the exact
    /// requested size.</param>
    /// <returns>
    /// An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.
    /// </returns>
    public static IMemoryOwner<byte> Rent(
        int minBufferSize,
        bool isSensitive,
        out Span<byte> buffer);

    /// <summary>
    /// Retrieves a buffer that is at least the requested length.
    /// </summary>
    /// <param name="minBufferSize">The minimum length of the buffer needed.</param>
    /// <param name="buffer">When this method returns, contains the buffer with the exact
    /// requested size.</param>
    /// <returns>
    /// An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.
    /// </returns>
    public static IMemoryOwner<byte> Rent(
        int minBufferSize,
        out Memory<byte> buffer);

    /// <summary>
    /// Retrieves a buffer that is at least the requested length.
    /// </summary>
    /// <param name="minBufferSize">The minimum length of the buffer needed.</param>
    /// <param name="isSensitive">Indicates whether the buffer should be pinned during it's lifetime
    /// and securely zeroed when returned. When <c>false></c>, this implementation delegates to
    /// <c>MemoryPool&lt;byte&gt;.Shared.Rent</c>.</param>
    /// <param name="buffer">When this method returns, contains the buffer with the exact
    /// requested size.</param>
    /// <returns>
    /// An <see cref="IMemoryOwner{T}"/> that manages the lifetime of the lease.
    /// </returns>
    public static IMemoryOwner<byte> Rent(
        int minBufferSize,
        bool isSensitive,
        out Memory<byte> buffer);
}
```

### HeapMemoryManager

```csharp
namespace NCode.CryptoMemory;

/// <summary>
/// Provides an implementation of <see cref="MemoryManager{T}"/> that allocates a byte buffer from
/// the heap and securely zeroes the allocated memory when the managed is disposed.
/// </summary>
public class HeapMemoryManager : MemoryManager<byte>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HeapMemoryManager"/> class.
    /// </summary>
    /// <param name="length">The length in bytes of the allocated memory.</param>
    /// <param name="zeroOnDispose">Specifies whether the allocated memory should be zeroed when
    /// the manager is disposed.</param>
    public HeapMemoryManager(int length, bool zeroOnDispose = true);

    /// <summary>
    /// Gets the memory block handled by this <see cref="MemoryManager{T}"/>.
    /// </summary>
    public Memory<byte> Memory { get; }

    /// <summary>
    /// Returns a memory span that wraps the underlying memory buffer.
    /// </summary>
    public Span<byte> GetSpan();

    /// <summary>
    /// Returns a handle to the memory that has been pinned and whose address can be taken.
    /// </summary>
    /// <param name="elementIndex">The offset to the element in the memory buffer at which the
    /// returned <see cref="MemoryHandle"/> points.</param>
    public MemoryHandle Pin(int elementIndex = 0);

    /// <summary>
    /// Unpins pinned memory so that the garbage collector is free to move it.
    /// </summary>
    /// <remarks>
    /// Since this <see cref="MemoryManager{T}"/> uses memory from the heap which is already
    /// pinned, this implemtation does nothing.
    /// </remarks>
    public void Unpin();
}
```

## Release Notes

* v1.0.0 - Initial release
* v1.0.1 - Updating readme
