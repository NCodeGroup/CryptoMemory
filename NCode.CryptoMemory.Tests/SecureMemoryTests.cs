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

using System.Security.Cryptography;
using Moq;

namespace NCode.CryptoMemory.Tests;

public class SecureMemoryTests
{
    [Fact]
    public void Dispose_ZerosMemory()
    {
        var lease = new SecureMemory<byte>(null, 1024);

        RandomNumberGenerator.Fill(lease.Memory.Span);
        Assert.False(lease.Memory.Span.ToArray().All(b => b == 0));

        lease.Dispose();

        Assert.True(lease.Memory.Span.ToArray().All(b => b == 0));
    }

    [Fact]
    public void Dispose_Once()
    {
        var lease = new SecureMemory<byte>(null, 1024);

        RandomNumberGenerator.Fill(lease.Memory.Span);
        Assert.False(lease.Memory.Span.ToArray().All(b => b == 0));

        lease.Dispose();

        Assert.True(lease.Memory.Span.ToArray().All(b => b == 0));

        RandomNumberGenerator.Fill(lease.Memory.Span);
        Assert.False(lease.Memory.Span.ToArray().All(b => b == 0));

        lease.Dispose();

        Assert.False(lease.Memory.Span.ToArray().All(b => b == 0));
    }

    [Fact]
    public void Dispose_ReturnsLease()
    {
        var mockLease = new Mock<SecureMemory<byte>>(MockBehavior.Strict, null!, 1024);
        mockLease
            .Setup(x => x.Return())
            .Verifiable();

        var lease = mockLease.Object;
        lease.Dispose();

        mockLease.VerifyAll();
    }
}
