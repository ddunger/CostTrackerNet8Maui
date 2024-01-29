using SQLite;


namespace Miljokaz.Models
{
    public class ColorsList
    {
        [PrimaryKey, AutoIncrement, Column("Id")]
        public int Id { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public string ColorStatus { get; set; }
     
    }
}
