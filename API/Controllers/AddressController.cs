using LogicService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelService;
using System.Security.Claims;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "JwtAuth")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressLogic _addressLogic;

        public AddressController(IAddressLogic addressLogic)
        {
            _addressLogic = addressLogic;
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

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            try
            {
                var role = GetLoggedInUserRole();
                var loggedInId = GetLoggedInCustomerId();

                if (role != "Admin" && customerId != loggedInId)
                {
                    return Forbid("You are not authorized to view this address.");
                }

                return Ok(await _addressLogic.GetByCustomerId(customerId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertAddress(AddressModel address)
        {
            try
            {
                var loggedInId = GetLoggedInCustomerId();

                if (address.CustomerId != loggedInId)
                {
                    return Forbid("You are not allowed to insert this address.");
                }

                return Ok(await _addressLogic.InsertAddress(address));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAddress(AddressModel address)
        {
            try
            {
                var loggedInId = GetLoggedInCustomerId();

                if (address.CustomerId != loggedInId)
                {
                    return Forbid("You are not allowed to update this address.");
                }

                return Ok(await _addressLogic.UpdateAddress(address));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                var loggedInId = GetLoggedInCustomerId();

                var addressList = await _addressLogic.GetByCustomerId(loggedInId);
                var addressToDelete = addressList.FirstOrDefault(a => a.AddressId == id);

                if (addressToDelete == null || addressToDelete.CustomerId != loggedInId)
                {
                    return Forbid("You are not allowed to delete this address.");
                }

                return Ok(await _addressLogic.DeleteAddress(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}