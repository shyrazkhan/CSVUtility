using System.Collections.Generic;
using CSVUtility.Core.Model;

namespace CSVUtility.Core
{
    public interface IUtilityEngine
    {
        bool SampleProperty { get; set; }

        string CsvFilePath { get; set; }
        void Action();
        int TotalOrders(IEnumerable<OrderDetail> orderDetailsLookup);
    }
}
