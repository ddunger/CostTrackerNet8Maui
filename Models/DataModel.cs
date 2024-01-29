using SQLite;

namespace Miljokaz.Models
{
    public class DataModel
    {
        [PrimaryKey, AutoIncrement, Column ("Id")]
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime dateTime { get; set; }
        public float Amount { get; set; }
        public string dataHexColor { get; set; }
    }
}
