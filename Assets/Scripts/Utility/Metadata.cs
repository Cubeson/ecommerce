using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    internal sealed class ProductMetadata
    {
        public ProductMetadata() { }
        public ProductMetadata(DateTime date, int fullDownload) { Date = date; FullDownload = fullDownload; }
        public DateTime Date { get; set; }
        public int FullDownload { get; set; }
    }
}
