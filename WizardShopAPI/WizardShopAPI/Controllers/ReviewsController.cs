using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WizardShopAPI.DTOs;
using WizardShopAPI.Models;
using WizardShopAPI.ResponseDto;
using WizardShopAPI.Services;

namespace WizardShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly WizardShopDbContext _context;
        private readonly IAzureReviewStorage _storage;
        public ReviewsController(WizardShopDbContext context, IAzureReviewStorage storage)
        {
            _context = context;
            _storage = storage;
        }

        // GET: api/5/ProductReviews
        [HttpGet("{productId}/ProductReviews")]
        public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetReviewsForProduct(int productId)
        {
            if (_context.Reviews == null)
            {
                return NotFound();
            }
            if (!_context.Products.Any(p => p.Id == productId)) return BadRequest();

            var responses=new List<ReviewResponseDto>();
            
            var reviews = await _context.Reviews.Where(r => r.ProductId == productId).ToListAsync();
            foreach (var review in reviews)
            {
                string username = "Anonymous";
                List<string> images = await _storage.ListAllUrisForReviewAsync(review.ReviewId);
                if (review.UserId.HasValue)
                {
                    User user = _context.Users.Where(u => u.UserId == review.UserId!).FirstOrDefault();
                    if (user == null) return BadRequest("this user doesn't exist");
                    username = user.Username;
                }

                ReviewResponseDto response = new ReviewResponseDto(review, username, images);
                responses.Add(response);
            }
            return Ok(responses);
        }

        // GET: api/Reviews/5
        [HttpGet("{reviewId}")]
        public async Task<ActionResult<ReviewResponseDto>> GetReview(int reviewId)
        {
            if (_context.Reviews == null)
            {
                return NotFound();
            }
            var review = await _context.Reviews.FindAsync(reviewId);

            if (review == null)
            {
                return NotFound();
            }

            List<string> images = await _storage.ListAllUrisForReviewAsync(reviewId);

            string username = "Anonymous";
            if (review.UserId.HasValue)
            {
                User user = _context.Users.Where(u => u.UserId == review.UserId!).FirstOrDefault();
                if (user == null) return BadRequest("this user doesn't exist");
                username = user.Username;

            }

            ReviewResponseDto response = new ReviewResponseDto(review, username, images);

            return response;
        }

        // POST: api/Reviews
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReviewResponseDto>> PostReview(ReviewDto reviewDto)
        {
            if (_context.Reviews == null)
            {
                return Problem("Entity set 'WizardShopDbContext.Reviews' is null.");
            }
            if (!ModelState.IsValid) return BadRequest();

            if(!_context.Products.Any(p=>p.Id==reviewDto.ProductId))return BadRequest();

            Review newReview = new Review()
            {
                ReviewId = this.GetNewReviewId(),
                ProductId = reviewDto.ProductId,
                Title = reviewDto.Title,
                Description = reviewDto.Description,
                Rating = reviewDto.Rating,
                UserId = reviewDto.UserId
            };
            
            _context.Reviews.Add(newReview);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ReviewExists(newReview.ReviewId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            string username = "Anonymous";
            if (newReview.UserId.HasValue)
            {
                User user = _context.Users.Where(u => u.UserId == newReview.UserId!).FirstOrDefault();
                if (user == null) return BadRequest("this user doesn't exist");
                username = user.Username;

            }

            ReviewResponseDto response = new ReviewResponseDto(newReview, username,new List<string>());

            return Ok(response);
        }

        // DELETE: api/Reviews/5
        [Authorize]
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            if (_context.Reviews == null)
            {
                return NotFound();
            }
            var review = await _context.Reviews.FindAsync(reviewId);

            if (review == null) return NotFound();

            

            ImageResponseDto rdto=await _storage.DeleteAllsFromReviewImageAsync(reviewId);

            if (rdto.Error) return BadRequest();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReviewExists(int id)
        {
            return (_context.Reviews?.Any(e => e.ReviewId == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Calculates new, review product id
        /// </summary>
        /// <returns>review id</returns>
        private int GetNewReviewId()
        {
            if (!_context.Reviews.Any()) return 1;

            return _context.Reviews.Max(x => x.ReviewId) + 1;
        }
    }
}
