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

namespace NCode.CryptoMemory.Tests;

public class HeapMemoryManagerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Ctor_Valid(bool zeroOnDispose)
    {
        const int length = 1024;
        using var manager = new HeapMemoryManager(length, zeroOnDispose);
        Assert.Equal(length, manager.Length);
        Assert.Equal(zeroOnDispose, manager.ZeroOnDispose);
        Assert.NotEqual(IntPtr.Zero, manager.BufferPtr);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Dispose_Valid(bool zeroOnDispose)
    {
        const int length = 1024;
        using var manager = new HeapMemoryManager(length, zeroOnDispose);
        ((IDisposable)manager).Dispose();
        Assert.Equal(IntPtr.Zero, manager.BufferPtr);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Memory_Valid(bool zeroOnDispose)
    {
        const int length = 1024;
        using var manager = new HeapMemoryManager(length, zeroOnDispose);
        Assert.Equal(length, manager.Memory.Length);

        ((IDisposable)manager).Dispose();

        Assert.Throws<ObjectDisposedException>(() =>
            manager.Memory.Length);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetSpan_Valid(bool zeroOnDispose)
    {
        const int length = 1024;
        using var manager = new HeapMemoryManager(length, zeroOnDispose);
        Assert.Equal(length, manager.GetSpan().Length);

        ((IDisposable)manager).Dispose();

        Assert.Throws<ObjectDisposedException>(() =>
            manager.GetSpan());
    }

    [Theory]
    [InlineData(true, 0)]
    [InlineData(true, 3)]
    [InlineData(false, 0)]
    [InlineData(false, 7)]
    public unsafe void Pin_Valid(bool zeroOnDispose, int elementIndex)
    {
        const int length = 1024;
        using var manager = new HeapMemoryManager(length, zeroOnDispose);
        using (var handle = manager.Pin(elementIndex))
        {
            Assert.Equal(manager.BufferPtr + elementIndex, (IntPtr)handle.Pointer);
        }

        ((IDisposable)manager).Dispose();

        Assert.Throws<ObjectDisposedException>(() =>
            manager.Pin());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Unpin_Valid(bool zeroOnDispose)
    {
        const int length = 1024;
        using var manager = new HeapMemoryManager(length, zeroOnDispose);

        manager.Unpin();
        ((IDisposable)manager).Dispose();
        manager.Unpin();
    }
}