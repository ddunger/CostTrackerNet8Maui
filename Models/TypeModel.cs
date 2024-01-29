using SkiaSharp;
using SQLite;

namespace Miljokaz.Models
{
    public class TypeModel
    {
        [PrimaryKey, AutoIncrement, Column ("Id")]
        public int Id { get; set; }
        public string Type { get; set; }
        public string typeHexColor { get; set; }
    
    }
}
