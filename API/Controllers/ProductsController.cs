using LogicService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelService;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsLogic _productsLogic;
        public ProductsController(IProductsLogic productsLogic)
        {
            _productsLogic = productsLogic;
        }

        [HttpGet("dropdown-values")]
        public async Task<IActionResult> GetDropdownValues()
        {
            try
            {
                return Ok(await _productsLogic.GetDropdownValues());
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(PaginationRequest paginationRequest)
        {
            try
            {
                return Ok(await _productsLogic.GetAll(paginationRequest));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByProductId(int id)
        {
            try
            {
                return Ok(await _productsLogic.GetByProductId(id));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> InsertProduct(ProductModel product)
        {
            try
            {
                return Ok(await _productsLogic.InsertProduct(product));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateProduct(ProductModel product)
        {
            try
            {
                return Ok(await _productsLogic.UpdateProduct(product));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                return Ok(await _productsLogic.DeleteProduct(id));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}