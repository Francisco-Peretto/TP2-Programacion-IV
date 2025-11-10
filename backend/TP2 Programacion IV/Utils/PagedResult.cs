namespace TP2_Programming_IV.Utils;
public record PagedResult<T>(IEnumerable<T> Items, int Page, int PageSize, int Total);
