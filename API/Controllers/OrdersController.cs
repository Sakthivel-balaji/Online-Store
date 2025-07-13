using System.Security.Claims;
using LogicService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelService;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersLogic _ordersLogic;
        public OrdersController(IOrdersLogic ordersLogic)
        {
            _ordersLogic = ordersLogic;
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

        [HttpGet("dropdown-values")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetDropdownValues()
        {
            try
            {
                return Ok(await _ordersLogic.GetDropdownValues());
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("GetOrders")]
        [Authorize(Policy = "JwtAuth")]
        public async Task<IActionResult> GetOrders(PaginationRequest paginationRequest)
        {
            try
            {
                return Ok(await _ordersLogic.GetOrders(paginationRequest));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "JwtAuth")]
        public async Task<IActionResult> GetByOrderId(int id)
        {
            try
            {
                var role = GetLoggedInUserRole();
                var loggedInId = GetLoggedInCustomerId();
                var orderList = await _ordersLogic.GetByOrderId(id);

                if (role != "Admin" && orderList.CustomerId != loggedInId)
                {
                    return Forbid("You are not allowed to view this order.");
                }

                return Ok(await _ordersLogic.GetByOrderId(id));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "JwtAuth")]
        public async Task<IActionResult> InsertOrder(OrderInsertModel order)
        {
            try
            {
                var role = GetLoggedInUserRole();
                var loggedInId = GetLoggedInCustomerId();

                if (role != "Admin" && order.CustomerId != loggedInId)
                {
                    return Forbid("You are not authorized to insert this order.");
                }

                return Ok(await _ordersLogic.InsertOrder(order));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateOrder(OrderUpdateModel order)
        {
            try
            {
                return Ok(await _ordersLogic.UpdateOrder(order));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "JwtAuth")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var role = GetLoggedInUserRole();
                var loggedInId = GetLoggedInCustomerId();
                var orderList = await _ordersLogic.GetByOrderId(id);

                if (role != "Admin" && orderList.CustomerId != loggedInId)
                {
                    return Forbid("You are not allowed to delete this order.");
                }

                if (role != "Admin" && orderList.Status != "Placed")
                {
                    return Forbid("Order cannot be cancelled contact admin.");
                }
                
                return Ok(await _ordersLogic.DeleteOrder(id));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}