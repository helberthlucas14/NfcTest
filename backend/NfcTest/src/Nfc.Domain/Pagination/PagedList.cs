namespace Nfc.Application.UseCases.NotaFiscal.Common
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalRecords { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList()
        {
        }

        public PagedList(List<T> items, int totalRecords, int pageNumber, int pageSize)
        {
            TotalRecords = totalRecords;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            AddRange(items);
        }

        public static PagedList<T> ToPagedList(List<T> source, int pageNumber, int pageSize)
        {
            var totalRecords = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToList();

            return new PagedList<T>(items, totalRecords, pageNumber, pageSize);
        }

    }
}
