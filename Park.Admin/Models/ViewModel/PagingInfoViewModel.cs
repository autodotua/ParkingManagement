using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Park.Admin.Models
{
    public class PagingInfoViewModel
    {
        public int RecordCount { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string SortField { get; set; }

        public string SortDirection { get; set; }

    }
}