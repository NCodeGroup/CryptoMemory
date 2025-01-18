#region Copyright Preamble

// Copyright @ 2025 NCode Group
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

public class EmptyMemoryTests
{
    [Fact]
    public void Singleton_Valid()
    {
        var value1 = EmptyMemory<byte>.Singleton;
        var value2 = EmptyMemory<byte>.Singleton;
        Assert.Same(EmptyMemory<byte>.Singleton, value1);
        Assert.Same(EmptyMemory<byte>.Singleton, value2);
    }

    [Fact]
    public void Memory_Valid()
    {
        var value1 = EmptyMemory<byte>.Singleton;
        var value2 = new EmptyMemory<byte>();
        Assert.Equal(Memory<byte>.Empty, value1.Memory);
        Assert.Equal(Memory<byte>.Empty, value2.Memory);
    }

    [Fact]
    public void Dispose_Valid()
    {
        var value1 = EmptyMemory<byte>.Singleton;
        value1.Dispose();

        var value2 = new EmptyMemory<byte>();
        value2.Dispose();

        value1.Dispose();
        value2.Dispose();
    }
}
