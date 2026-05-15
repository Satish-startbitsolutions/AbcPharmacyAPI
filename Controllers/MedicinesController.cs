using AbcPharmacy.DTOs;
using AbcPharmacy.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbcPharmacy.Controllers;

/// <summary>
/// Manages medicine records — CRUD + search.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MedicinesController : ControllerBase
{
    private readonly IMedicineService _medicineService;

    public MedicinesController(IMedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    // ────────────────────────────────────────────────────────────
    // GET  /api/medicines
    // Returns paged + searchable list for the grid (no Notes field)
    // ────────────────────────────────────────────────────────────
    /// <summary>
    /// Get all medicines. Supports search by name/brand, sorting, and pagination.
    /// </summary>
    /// <param name="search">Filter by medicine name or brand (optional).</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100).</param>
    /// <param name="sortBy">Sort field: name | expiry | quantity | price | brand.</param>
    /// <param name="ascending">Sort direction (default: true = ascending).</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MedicineListItem>>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search   = null,
        [FromQuery] int    page      = 1,
        [FromQuery] int    pageSize  = 10,
        [FromQuery] string? sortBy   = "name",
        [FromQuery] bool   ascending = true,
        [FromQuery] string? filter   = null)
    {
        // Clamp page size
        pageSize = Math.Clamp(pageSize, 1, 100);
        page     = Math.Max(page, 1);

        var result = await _medicineService.GetAllAsync(search, page, pageSize, sortBy, ascending, filter);
        return Ok(ApiResponse<PagedResult<MedicineListItem>>.Ok(result,
            $"{result.TotalCount} medicine(s) found."));
    }

    // ────────────────────────────────────────────────────────────
    // GET  /api/medicines/{id}
    // Returns full details including Notes
    // ────────────────────────────────────────────────────────────
    /// <summary>Get a single medicine by ID (includes Notes field).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MedicineResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<MedicineResponse>), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        var medicine = await _medicineService.GetByIdAsync(id);
        if (medicine == null)
            return NotFound(ApiResponse<MedicineResponse>.NotFound($"Medicine with ID {id} not found."));

        return Ok(ApiResponse<MedicineResponse>.Ok(medicine));
    }

    // ────────────────────────────────────────────────────────────
    // POST  /api/medicines
    // ────────────────────────────────────────────────────────────
    /// <summary>Add a new medicine to the system.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MedicineResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<MedicineResponse>), 400)]
    public async Task<IActionResult> Create([FromBody] CreateMedicineRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<MedicineResponse>.Fail("Validation failed.", errors));
        }

        var medicine = await _medicineService.CreateAsync(request);
        return CreatedAtAction(
            nameof(GetById),
            new { id = medicine.Id },
            ApiResponse<MedicineResponse>.Ok(medicine, "Medicine added successfully."));
    }

    // ────────────────────────────────────────────────────────────
    // PUT  /api/medicines/{id}
    // Partial update — only provided fields are updated
    // ────────────────────────────────────────────────────────────
    /// <summary>Update an existing medicine (partial update — send only fields you want to change).</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MedicineResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<MedicineResponse>), 404)]
    [ProducesResponseType(typeof(ApiResponse<MedicineResponse>), 400)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicineRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<MedicineResponse>.Fail("Validation failed.", errors));
        }

        var medicine = await _medicineService.UpdateAsync(id, request);
        if (medicine == null)
            return NotFound(ApiResponse<MedicineResponse>.NotFound($"Medicine with ID {id} not found."));

        return Ok(ApiResponse<MedicineResponse>.Ok(medicine, "Medicine updated successfully."));
    }

    // ────────────────────────────────────────────────────────────
    // DELETE  /api/medicines/{id}
    // ────────────────────────────────────────────────────────────
    /// <summary>Delete a medicine by ID.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _medicineService.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse.Fail($"Medicine with ID {id} not found."));

        return Ok(ApiResponse.Ok("Medicine deleted successfully."));
    }
}
