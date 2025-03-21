using Microsoft.AspNetCore.Mvc;
using SimpleAPI.DTO;
using System.Text.Json;

namespace SimpleAPI.Controllers
{
  public partial class CustomerController
  {
    const int maxCitiesPageSize = 5;

    [HttpGet("GetCustomersWithPagination/")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 5)
    {
      if (pageSize > maxCitiesPageSize)
      {
        pageSize = maxCitiesPageSize;
      }

      var (customerEntities, paginationMetadata) = await _customerService.GetCustomersAsync(name, searchQuery, pageNumber, pageSize);

      Response.Headers.Add("X-Pagination",
          JsonSerializer.Serialize(paginationMetadata));

      return Ok(_mapper.Map<IEnumerable<CustomerDto>>(customerEntities));
    }
  }
}
