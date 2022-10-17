using Microsoft.Extensions.Logging;
using Moq.AutoMock;
using Moq.AutoMock.Resolvers;
using System;
using System.Linq;
using System.Reflection;

namespace Lombiq.Tests.Resolvers;

/// <summary>
/// Resolver for a type with a single generic type argument.
/// </summary>
public abstract class GenericTypeResolver : IMockResolver
{
    /// <summary>
    /// Gets the generic type to be resolved, such as <c>typeof(ILogger&lt;&gt;)</c>.
    /// </summary>
    protected abstract Type ExpectedType { get; }

    /// <summary>
    /// Resolves <see cref="ILogger{TCategoryName}"/> types.
    /// </summary>
    /// <param name="context">The resolution context.</param>
    public void Resolve(MockResolutionContext context)
    {
        var (mocker, serviceType, _) = context;

        if (!serviceType.GetTypeInfo().IsGenericType || serviceType.GetGenericTypeDefinition() != ExpectedType) return;

        var genericType = serviceType.GetGenericArguments().Single();
        if (ResolveValue(mocker, serviceType, genericType) is { } value) context.Value = value;
    }

    protected abstract object ResolveValue(AutoMocker mocker, Type serviceType, Type genericType);
}
