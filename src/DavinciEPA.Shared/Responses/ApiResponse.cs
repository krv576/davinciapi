namespace DavinciEPA.Shared.Responses;

/// <summary>Generic envelope for non-FHIR JSON API responses (administrative/status endpoints).</summary>
public sealed record ApiResponse<T>(bool Success, T? Data, string? Message)
{
    public static ApiResponse<T> Ok(T data, string? message = null) => new(true, data, message);

    public static ApiResponse<T> Fail(string message) => new(false, default, message);
}

/// <summary>Generic paginated result envelope for non-FHIR list endpoints.</summary>
public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
