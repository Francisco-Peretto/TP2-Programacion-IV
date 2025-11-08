namespace TP2_Programacion_IV.Utils;
public record PagedResult<T>(IEnumerable<T> Items, int Page, int PageSize, int Total);
