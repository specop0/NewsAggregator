using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;

namespace NewsAggregator.OpenApi;

public static class OpenApiExtensions
{
    public static void RemoveServers(this OpenApiOptions options)
    {
        options.AddDocumentTransformer((document, context, cancel) =>
        {
            document.Servers = null;
            return Task.CompletedTask;
        });
    }
}