using ListingApi.Extensions;
using ListingApi.Models;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using System.Text.RegularExpressions;

namespace ListingApi.Infrastructure;

public class ListingSeed
{
    public async Task SeedAsync(ListingContext context, IWebHostEnvironment env, ILogger<ListingSeed> logger)
    {
        var policy = CreatePolicy(logger, nameof(ListingSeed));

        await policy.ExecuteAsync(async () =>
        {
            if (!context.Listings.Any())
            {
                await context.Listings.AddRangeAsync(GetListingsFromFile(env.ContentRootPath, logger));
                await context.SaveChangesAsync();
            }
        });
    }

    private IEnumerable<Listing> GetListingsFromFile(string contentRootPath, ILogger<ListingSeed> logger)
    {
        string csvListings = Path.Combine(contentRootPath, "Infrastructure", "SeedFiles", "Listings.csv");

        string[] csvHeaders = new string[] { };

        try
        {
            string[] headers = { "Id", "Name", "Description", "Price", "Type" };
            csvHeaders = GetHeaders(csvListings, headers);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SEED ERROR: {Message}", ex.Message);
            // Get preconfiguredlistings
        }

        return File.ReadAllLines(csvListings)
                        .Skip(1) // skip header row
                        .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                        .SelectTry<string[], Listing>(column => CreateListing(column, csvHeaders))
                        .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                        .Where(x => x != null);
    }

    private Listing CreateListing(string[] column, string[] headers)
    {
        if (column.Count() != headers.Count())
        {
            throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
        }

        return new Listing()
        {
            Id = Guid.Parse(column[Array.IndexOf(headers, "id")].Trim('"').Trim()),
            Name = column[Array.IndexOf(headers, "name")].Trim('"').Trim(),
            Description = column[Array.IndexOf(headers, "description")].Trim('"').Trim(),
            Price = int.Parse(column[Array.IndexOf(headers, "price")].Trim('"').Trim()),
            Type = (ListingType)Enum.Parse(typeof(ListingType), column[Array.IndexOf(headers, "type")].Trim('"').Trim())
        };
    }

    private string[] GetHeaders(string csvfile, string[] requiredHeaders, string[] optionalHeaders = null)
    {
        string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

        if (csvheaders.Count() < requiredHeaders.Count())
        {
            throw new Exception($"requiredHeader count '{requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");
        }

        if (optionalHeaders != null)
        {
            if (csvheaders.Count() > (requiredHeaders.Count() + optionalHeaders.Count()))
            {
                throw new Exception($"csv header count '{csvheaders.Count()}'  is larger then required '{requiredHeaders.Count()}' and optional '{optionalHeaders.Count()}' headers count");
            }
        }

        foreach (var requiredHeader in requiredHeaders)
        {
            if (!csvheaders.Contains(requiredHeader.ToLower()))
            {
                throw new Exception($"does not contain required header '{requiredHeader}'");
            }
        }

        return csvheaders;
    }

    private AsyncRetryPolicy CreatePolicy(ILogger<ListingSeed> logger, string prefix, int retries = 3)
    {
        return Policy.Handle<SqlException>().
            WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                }
            );
    }
}
