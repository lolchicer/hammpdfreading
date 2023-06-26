using Microsoft.EntityFrameworkCore;

namespace Infor.HammPdfReading.WebApi
{
    [PrimaryKey(nameof(PartNo), nameof(Item), nameof(Assembly), nameof(Series))]
    public class WebDetail
    {
        public string? PartNo { get; set; }
        public double Item { get; set; }
        public string? ValidFor { get; set; }
        public double Quantity { get; set; }
        public Unit Unit { get; set; }
        public string? Designation { get; set; }

        // сделать 2 нормальные таблицы уровень: невозмжоно
        public string? Assembly { get; set; }
        public string? Series { get; set; }

        public WebDetail() { }

        public WebDetail(ExtendedDetail detail)
        {
            PartNo = detail.PartNo;
            Item = detail.Item;
            ValidFor = $"{detail.ValidFor.Item1}-{detail.ValidFor.Item2}";
            Quantity = detail.Quantity;
            Unit = detail.Unit;
            Designation = detail.Designation;
            Assembly = detail.Assembly;
            Series = detail.Series;
        }
    }
}
