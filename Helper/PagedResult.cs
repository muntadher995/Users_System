﻿namespace Ai_LibraryApi.Helper
{
     
        public class PagedResult<T>
        {
            public IEnumerable<T> Items { get; set; }
            public int TotalCount { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }

            public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
            {
                Items = items;
                TotalCount = totalCount;
                PageNumber = pageNumber;
                PageSize = pageSize;
            }
        }
    
}
