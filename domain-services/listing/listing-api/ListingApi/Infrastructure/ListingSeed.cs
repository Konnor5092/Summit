using ListingApi.Models;

namespace ListingApi.Infrastructure;

public class ListingSeed
{

    private IEnumerable<Listing> GetListingsFromFile(string contentRootPath, ILogger<ListingSeed> logger)
    {
        string csvListings = Path.Combine(contentRootPath, "Infrastructure", "SeedFiles", "Listings.csv");

        string[] csvHeaders;

        try
        {
            string[] headers = { "Id", "Name", "Description", "Name", "Price", "Type" };
            csvHeaders = GetHeaders(csvListings, headers);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SEED ERROR: {Message}", ex.Message);
            // Get preconfiguredlistings
        }

        //return File.ReadAllLines(csvListings).Skip(1).
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
            Price = int.Parse(column[Array.IndexOf(headers, "Price")].Trim('"').Trim()),
            Type = (ListingType)Enum.Parse(typeof(ListingType), column[Array.IndexOf(headers, "Price")].Trim('"').Trim())
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
            if (!csvheaders.Contains(requiredHeader))
            {
                throw new Exception($"does not contain required header '{requiredHeader}'");
            }
        }

        return csvheaders;
    }
}
