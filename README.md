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

## Release Notes

* v1.0.0 - Initial release
* v1.0.1 - Updating readme
* v2.0.0 - Net8 upgrade. Refactored to use SecureMemoryPool. Added SecureEncoding. Removed HeapMemoryManager.
