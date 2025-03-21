using Microsoft.AspNetCore.Mvc;
using SimpleAPI.DTO;
using System.Text.Json;

namespace SimpleAPI.Controllers
{
  public partial class CustomerController : ControllerBase
  {
    const int maxCitiesPageSize = 5;

    [HttpGet("GetCustomersWithPagination")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers(
        string? name,
        string? searchQuery,
        int pageNumber = 1,
        int pageSize = 5)
    {
      // Validation for pageSize and pageNumber
      if (pageSize <= 0)
      {
        return BadRequest("Page size must be greater than zero.");
      }

      if (pageNumber <= 0)
      {
        return BadRequest("Page number must be greater than zero.");
      }

      // Apply maxCitiesPageSize limit
      if (pageSize > maxCitiesPageSize)
      {
        pageSize = maxCitiesPageSize;
      }

      // Fetch customers with pagination
      var (customerEntities, paginationMetadata) = await _customerService.GetCustomersAsync(name, searchQuery, pageNumber, pageSize);

      // Handle case where no customers are found
      if (customerEntities == null || !customerEntities.Any())
      {
        return NotFound("No customers found matching the criteria.");
      }

      // Add pagination metadata to response header
      Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

      // Return the customers in DTO format
      var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customerEntities);
      return Ok(customerDtos);
    }
  }
}