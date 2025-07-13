using System.Security.Claims;
using LogicService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelService;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersLogic _customersLogic;
        public CustomersController(ICustomersLogic customersLogic)
        {
            _customersLogic = customersLogic;
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
                return Ok(await _customersLogic.GetDropdownValues());
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("GetAll")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAll(PaginationRequest paginationRequest)
        {
            try
            {
                return Ok(await _customersLogic.GetAll(paginationRequest));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<IActionResult> GetByCustomerId(int id)
        {
            try
            {
                var role = GetLoggedInUserRole();
                var loggedInId = GetLoggedInCustomerId();

                if (role != "Admin" && id != loggedInId)
                {
                    return Forbid("You are not authorized to view this customer.");
                }

                return Ok(await _customersLogic.GetByCustomerId(id));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<IActionResult> UpdateCustomer(CustomerUpdateModel customer)
        {
            try
            {
                var role = GetLoggedInUserRole();
                var loggedInId = GetLoggedInCustomerId();

                if (role != "Admin" && customer.CustomerId != loggedInId)
                {
                    return Forbid("You are not authorized to update this customer.");
                }

                return Ok(await _customersLogic.UpdateCustomer(customer));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                return Ok(await _customersLogic.DeleteCustomer(id));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}