using System.Net.Http.Json;
using BuildingBlocks.Application;
using Shouldly;

namespace Shared.IntegrationTests.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<HttpResponseMessage> ShouldBeErrorAsync<TError>(
        this HttpResponseMessage response, string? id = null, string? message = null)
        where TError : Error
    {
        var error = await response.Content.ReadFromJsonAsync<TError>();
        error.ShouldNotBeNull();

        if (id is not null) error.Id.ShouldBe(id);
        if (message is not null) error.Message.ShouldBe(message);

        return response;
    }
}