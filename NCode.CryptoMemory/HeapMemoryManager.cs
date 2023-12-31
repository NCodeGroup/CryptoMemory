﻿#region Copyright Preamble
// 
//    Copyright @ 2023 NCode Group
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
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace NCode.CryptoMemory;

/// <summary>
/// Provides an implementation of <see cref="MemoryManager{T}"/> that allocates a byte buffer from the heap and
/// securely zeroes the allocated memory when the managed is disposed.
/// </summary>
public class HeapMemoryManager : MemoryManager<byte>
{
    internal int Length { get; }
    internal bool ZeroOnDispose { get; }
    internal IntPtr BufferPtr { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeapMemoryManager"/> class.
    /// </summary>
    /// <param name="length">The length in bytes of the allocated memory.</param>
    /// <param name="zeroOnDispose">Specifies whether the allocated memory should be zeroed when the manager is disposed.</param>
    public HeapMemoryManager(int length, bool zeroOnDispose = true)
    {
        Length = length;
        ZeroOnDispose = zeroOnDispose;
        BufferPtr = Marshal.AllocHGlobal(length);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (BufferPtr == IntPtr.Zero) return;
        if (ZeroOnDispose) CryptographicOperations.ZeroMemory(GetSpan());
        Marshal.FreeHGlobal(BufferPtr);
        BufferPtr = IntPtr.Zero;
    }

    /// <inheritdoc />
    public override Memory<byte> Memory =>
        BufferPtr == IntPtr.Zero ? throw new ObjectDisposedException(GetType().FullName) : CreateMemory(Length);

    /// <inheritdoc />
    public override unsafe Span<byte> GetSpan() =>
        BufferPtr == IntPtr.Zero
            ? throw new ObjectDisposedException(GetType().FullName)
            : new Span<byte>(BufferPtr.ToPointer(), Length);

    /// <inheritdoc />
    public override unsafe MemoryHandle Pin(int elementIndex = 0) =>
        BufferPtr == IntPtr.Zero
            ? throw new ObjectDisposedException(GetType().FullName)
            : new MemoryHandle((byte*)BufferPtr + elementIndex);

    /// <inheritdoc />
    public override void Unpin()
    {
        // nothing
    }
}