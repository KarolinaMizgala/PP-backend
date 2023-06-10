using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using WizardShopAPI.Models;

namespace WizardShopAPI.ResponseDto
{
    public class ReviewResponseDto
    {
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Rating { get; set; }
        public string? Username { get; set; }
        public List<string>? ImageUris { get; set; }

        public ReviewResponseDto(Review review, string username,List<String>uris)
        {
            ReviewId = review.ReviewId;
            ProductId = review.ProductId;
            Title = review.Title;
            Description = review.Description;
            Rating = review.Rating;
            Username = username;
            ImageUris = uris; 
        }
    }
}
