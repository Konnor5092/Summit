using ListingApi.Infrastructure;
using ListingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ListingApi.Controllers
{
    [Route("api/[controller]")]
    public class ListingController : Controller
    {
        public ListingContext _listingContext { get; }

        public ListingController(ListingContext listingContext)
        {
            _listingContext = listingContext;
        }

        // GET api/listing/listings?pageSize=3&pageIndex=1
        [HttpGet]
        [Route("listings")]
        [ProducesResponseType(typeof(PaginatedItems<Listing>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ListingsAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var totalItems = await _listingContext.Listings
                .LongCountAsync();

            if (totalItems <= 0)
            {
                return NotFound();
            }

            var itemsOnPage = await _listingContext.Listings
                .OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            var model = new PaginatedItems<Listing>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        // GET api/listing/listing/{id:guid}
        [HttpGet]
        [Route("listing/{id:guid}")]
        [ProducesResponseType(typeof(Listing), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Listing>> ListingByIdAsync(Guid id)
        {
            var item = await _listingContext.Listings.SingleOrDefaultAsync(ci => ci.Id == id);

            if (item != null)
            {
                return item;
            }

            return NotFound();
        }

        // POST api/listing/listing
        [HttpPost]
        [Route("listing")]        
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddListingAsync([FromBody] ListingDto listing)
        {
            var item = new Listing()
            {
                Id = Guid.NewGuid(),
                Name = listing.Name,
                Description = listing.Description,
                Price = listing.Price,
                Type = (ListingType)Enum.Parse(typeof(ListingType), listing.Type)
            };

            _listingContext.Listings.Add(item);

            await _listingContext.SaveChangesAsync();

            return CreatedAtAction(nameof(ListingByIdAsync), new { id = item.Id }, null);
        }
    }
}
