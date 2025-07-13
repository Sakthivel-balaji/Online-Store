using System.Security.Claims;
using LogicService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelService;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsLogic _reviewsLogic;
        public ReviewsController(IReviewsLogic reviewsLogic)
        {
            _reviewsLogic = reviewsLogic;
        }

        private int GetLoggedInCustomerId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        private string GetLoggedInUserRole()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        }
        
        [HttpPost]
        [Authorize(Policy = "JwtAuth")]
        public async Task<IActionResult> InsertReview(ReviewInsertModel review)
        {
            try
            {
                var role = GetLoggedInUserRole();
                var loggedInId = GetLoggedInCustomerId();

                if (review.CustomerId != loggedInId)
                {
                    return Forbid("You are not allowed to insert this review.");
                }

                return Ok(await _reviewsLogic.InsertReview(review));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetByProductId([FromQuery] int productId, [FromQuery] PaginationRequest page)
        {
            try
            {
                return Ok(await _reviewsLogic.GetByProductId(page, productId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "JwtAuth")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                return Ok(await _reviewsLogic.DeleteReview(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}