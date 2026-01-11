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

using JetBrains.Annotations;

namespace NCode.CryptoMemory;

/// <summary>
/// Represents a ref struct that holds a leased <see cref="ReadOnlySpan{T}"/> and manages the lifetime of its underlying owner.
/// When disposed, the owner is also disposed, releasing the leased memory back to its source.
/// </summary>
/// <typeparam name="T">The type of elements in the span.</typeparam>
/// <param name="owner">The <see cref="IDisposable"/> owner that manages the underlying memory, or <see langword="null"/> if no owner is associated.</param>
/// <param name="span">The <see cref="ReadOnlySpan{T}"/> representing the leased memory.</param>
[PublicAPI]
public readonly ref struct RefSpanLease<T>(IDisposable? owner, ReadOnlySpan<T> span) : IDisposable
{
    private IDisposable? Owner { get; } = owner;

    /// <summary>
    /// Gets the leased <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public ReadOnlySpan<T> Span { get; } = span;

    /// <summary>
    /// Disposes the underlying owner, releasing the leased memory back to its source.
    /// </summary>
    public void Dispose()
    {
        Owner?.Dispose();
    }
}
