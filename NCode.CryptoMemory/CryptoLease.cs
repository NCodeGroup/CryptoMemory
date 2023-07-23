#region Copyright Preamble
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

internal sealed class CryptoLease : IMemoryOwner<byte>
{
    internal byte[] Buffer { get; private set; }
    internal GCHandle Handle { get; private set; }
    public Memory<byte> Memory { get; private set; }

    public CryptoLease(int minBufferSize)
    {
        Buffer = ArrayPool<byte>.Shared.Rent(minBufferSize);
        Handle = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
        Memory = MemoryMarshal.CreateFromPinnedArray(Buffer, 0, Buffer.Length);
    }

    public void Dispose()
    {
        if (!Handle.IsAllocated) return;
        CryptographicOperations.ZeroMemory(Buffer.AsSpan());
        Handle.Free();
        Handle = new GCHandle();
        ArrayPool<byte>.Shared.Return(Buffer);
        Buffer = Array.Empty<byte>();
        Memory = Memory<byte>.Empty;
    }
}