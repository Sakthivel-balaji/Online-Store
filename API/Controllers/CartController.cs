using System.Security.Claims;
using LogicService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelService;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "JwtAuth")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartLogic _cartLogic;
        public CartController(ICartLogic cartLogic)
        {
            _cartLogic = cartLogic;
        }   

        private int GetLoggedInCustomerId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        [HttpPost]
        public async Task<IActionResult> InsertItem(CartInsertModel cart)
        {
            try
            {
                var loggedInId = GetLoggedInCustomerId();

                if (cart.CustomerId != loggedInId)
                {
                    return Forbid("You are not authorized to insert item in cart.");
                }

                return Ok(await _cartLogic.InsertItem(cart));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            try
            {
                var loggedInId = GetLoggedInCustomerId();

                if (customerId != loggedInId)
                {
                    return Forbid("You are not authorized to get the cart items.");
                }

                return Ok(await _cartLogic.GetByCustomerId(customerId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateItem(CartUpdateModel cart)
        {
            try
            {
                var loggedInId = GetLoggedInCustomerId();
                var cartList = await _cartLogic.GetByCustomerId(loggedInId);

                if (cartList == null || cartList.CustomerId != loggedInId)
                {
                    return Forbid("You are not authorized to update this cart item.");
                }

                return Ok(await _cartLogic.UpdateItem(cart));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var loggedInId = GetLoggedInCustomerId();
                var cartList = await _cartLogic.GetByCustomerId(loggedInId);

                if (cartList == null || cartList.CustomerId != loggedInId)
                {
                    return Forbid("You are not authorized to delete this cart item.");
                }

                return Ok(await _cartLogic.DeleteItem(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}