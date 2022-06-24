﻿namespace ListingApi.Models
{
    public class Listing
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public ListingType Type { get; set; }
    }
}
