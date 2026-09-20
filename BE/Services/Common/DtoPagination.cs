using System.Collections.Generic;

namespace Ordbox.Services.Common
{
    public class DtoPagination<T>
    {
        public IEnumerable<T> Data { get; set; }
        public dynamic RawData { get; set; }
        public long TotalCount { get; set; }
        public int PageSize { get; set; }
    }
}
