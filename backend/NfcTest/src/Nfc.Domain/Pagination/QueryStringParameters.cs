namespace Nfc.Application.UseCases.NotaFiscal.Common
{
    public class QueryStringParameters
    {
        const int maxPageSize = 100;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 50;

        public int PageSize
        {
            get { return _pageSize; }
            set {
                if (value < 1)
                {
                    _pageSize = 1;
                }
                else
                {
                    _pageSize = (value > maxPageSize) ? maxPageSize : value;
                }
            }
        }
    }
}
