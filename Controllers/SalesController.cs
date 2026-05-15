using AbcPharmacy.DTOs;
using AbcPharmacy.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbcPharmacy.Controllers;

/// <summary>
/// Manages medicine sale records — record sales, view history, and get summary stats.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    // ────────────────────────────────────────────────────────────
    // GET  /api/sales
    // Returns paged list of all sale transactions
    // ────────────────────────────────────────────────────────────
    /// <summary>Get all sale records. Optionally filter by medicine ID.</summary>
    /// <param name="medicineId">Filter sales for a specific medicine (optional).</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100).</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<SaleResponse>>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? medicineId = null,
        [FromQuery] int  page       = 1,
        [FromQuery] int  pageSize   = 10, 
        [FromQuery] string? sortBy  = null,
        [FromQuery] bool ascending  = true)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        page     = Math.Max(page, 1);

        var result = await _saleService.GetAllAsync(medicineId, page, pageSize, sortBy, ascending);
        return Ok(ApiResponse<PagedResult<SaleResponse>>.Ok(result,
            $"{result.TotalCount} sale record(s) found."));
    }

    // ────────────────────────────────────────────────────────────
    // GET  /api/sales/{id}
    // ────────────────────────────────────────────────────────────
    /// <summary>Get a single sale record by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SaleResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<SaleResponse>), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        var sale = await _saleService.GetByIdAsync(id);
        if (sale == null)
            return NotFound(ApiResponse<SaleResponse>.NotFound($"Sale record with ID {id} not found."));

        return Ok(ApiResponse<SaleResponse>.Ok(sale));
    }

    // ────────────────────────────────────────────────────────────
    // POST  /api/sales
    // Records a new sale + deducts stock automatically
    // ────────────────────────────────────────────────────────────
    /// <summary>
    /// Record a new medicine sale. Automatically deducts sold quantity from stock.
    /// Validates: medicine exists, not expired, sufficient stock.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SaleResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<SaleResponse>), 400)]
    [ProducesResponseType(typeof(ApiResponse<SaleResponse>), 404)]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<SaleResponse>.Fail("Validation failed.", errors));
        }

        var (sale, error) = await _saleService.CreateAsync(request);

        if (error != null)
            return BadRequest(ApiResponse<SaleResponse>.Fail(error));

        return CreatedAtAction(
            nameof(GetById),
            new { id = sale!.Id },
            ApiResponse<SaleResponse>.Ok(sale, "Sale recorded successfully. Stock updated."));
    }

    // ────────────────────────────────────────────────────────────
    // GET  /api/sales/summary
    // Dashboard stats: total revenue, transactions, units sold
    // ────────────────────────────────────────────────────────────
    /// <summary>Get aggregated sales statistics: total revenue, transactions, and units sold.</summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<SalesSummary>), 200)]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _saleService.GetSummaryAsync();
        return Ok(ApiResponse<SalesSummary>.Ok(summary, "Sales summary retrieved."));
    }
}
