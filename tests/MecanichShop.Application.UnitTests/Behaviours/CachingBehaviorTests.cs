using MechanicShop.Application.Common.Behaviours;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace MechanicShop.Application.UnitTests.Behaviours;

public class CachingBehaviorTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();
    private readonly ILogger<CachingBehavior<CachedQuery, Result<string>>> _logger = Substitute.For<ILogger<CachingBehavior<CachedQuery, Result<string>>>>();

    private readonly CachingBehavior<CachedQuery, Result<string>> _sut;

    public CachingBehaviorTests()
    {
        _sut = new CachingBehavior<CachedQuery, Result<string>>(_cache, _logger);

        _cache.GetOrCreateAsync(
            Arg.Any<string>(),
            Arg.Any<Func<CancellationToken, ValueTask<Result<string>>>>(),
            Arg.Any<HybridCacheEntryOptions>(),
            Arg.Any<IEnumerable<string>>(),
            Arg.Any<CancellationToken>()
        ).Returns(callInfo =>
        {
            var factory = callInfo.Arg<Func<CancellationToken, ValueTask<Result<string>>>>();
            return factory(CancellationToken.None);
        });
    }

    [Fact]
    public async Task Handle_WhenNotCachedQuery_ShouldSkipCacheAndReturnResult()
    {
        var uncachedRequest = new NonCachedQuery();
        var behavior = new CachingBehavior<NonCachedQuery, string>(_cache, Substitute.For<ILogger<CachingBehavior<NonCachedQuery, string>>>());

        var result = await behavior.Handle(uncachedRequest, _ => Task.FromResult("OK"), CancellationToken.None);

        Assert.Equal("OK", result);
    }

    [Fact]
    public async Task Handle_WhenCachedQueryAndResultIsSuccess_ShouldCacheResult()
    {
        var request = new CachedQuery();
        var response = (Result<string>)"test-value";

        var result = await _sut.Handle(request, _ => Task.FromResult(response), CancellationToken.None);

        Assert.NotNull(result); 
        Assert.True(result.IsSuccess);
        Assert.Equal("test-value", result.Value);
    }

    [Fact]
    public async Task Handle_WhenCachedQueryAndResultIsError_ShouldNotCacheResult()
    {
        var request = new CachedQuery();
        var errorResult = (Result<string>)Error.Validation("code", "message");

        var result = await _sut.Handle(request, _ => Task.FromResult(errorResult), CancellationToken.None);

        Assert.NotNull(result); 
        Assert.True(result.IsError);

        await _cache.DidNotReceiveWithAnyArgs().SetAsync<Result<string>>(default!, default!);
    }

    public class NonCachedQuery;

    public class CachedQuery : ICachedQuery
    {
        public string CacheKey => "test-key";
        public TimeSpan Expiration => TimeSpan.FromMinutes(5);
        public string[] Tags => ["unit-test"];
    }
}