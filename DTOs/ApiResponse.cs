namespace AbcPharmacy.DTOs;

/// <summary>
/// Standard API response envelope used by all endpoints.
/// Provides consistent shape: { success, message, data, errors }
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    // ── Factory helpers ─────────────────────────────────────────

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors ?? new() };

    public static ApiResponse<T> NotFound(string message = "Record not found.") =>
        new() { Success = false, Message = message };
}

/// <summary>Non-generic version for responses with no data payload.</summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static ApiResponse Ok(string message = "Success") =>
        new() { Success = true, Message = message };

    public static ApiResponse Fail(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors ?? new() };
}

/// <summary>Paged result wrapper for list endpoints.</summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int ExpiringSoonCount { get; set; }
    public int LowStockCount { get; set; }
    public int ExpiredCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNext => Page < TotalPages;
    public bool HasPrev => Page > 1;
}
