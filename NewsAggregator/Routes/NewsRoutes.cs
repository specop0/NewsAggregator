using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NewsAggregator.Model;
using NewsAggregator.UseCases;

namespace NewsAggregator.Routes;

public static class NewsRoutes
{
    public static void MapNewsRoutes(this IEndpointRouteBuilder app)
    {
        var newsGroups = app.MapGroup("api")
            .WithTags("News")
            .RequireAuthorization();
        newsGroups.MapPost("News", Get);
    }

    [EndpointName("getNews")]
    [EndpointSummary("Gets the news.")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(GetNewsResponse))]
    private static async Task<IResult> Get(
        [FromBody] GetNewsRequest? request,
        [FromServices] GetNewsHandler handler)
    {
        var isLatest = request?.IsLatest ?? false;
        var news = await handler.Invoke(isLatest);
        var response = new GetNewsResponse
        {
            Items = news,
        };
        return Results.Ok(response);
    }


}
