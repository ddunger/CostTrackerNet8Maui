
using Miljokaz.Models;
using SQLite;

namespace Miljokaz.Data
{
    public class DataRepository
    {
        public string _dbPath;
        private SQLiteConnection conn;

        public DataRepository(string dbPath)
        {
            _dbPath = dbPath;
        }

        public void Init()
        {
            conn = new SQLiteConnection(_dbPath);
            conn.CreateTable<DataModel>();
            conn.CreateTable<TypeModel>();
            conn.CreateTable<ColorsList>();
        }
        public List<ColorsList> GetAvailableColors()
        {
            Init();
            return conn.Table<ColorsList>().Where(c => c.ColorStatus == "none").ToList();
        }

        public List<DataModel> GetAllData() 
        { 
            Init();
            return conn.Table<DataModel>().ToList();
        }
        public List<DataModel> GetDataRange(DateTime startDate, DateTime endDate)
        {
            Init();
            var query = conn.Table<DataModel>().Where(data => data.dateTime >= startDate && data.dateTime <= endDate);
            return query.ToList();
        }
        public List<TypeModel> GetAllTypes()
        {
            Init();
            return conn.Table<TypeModel>().ToList();
        }

        public void AddData(DataModel dataModel) 
        {
            conn = new SQLiteConnection(_dbPath);
            conn.Insert(dataModel);
        }

        public void AddType(TypeModel typeModel)
        {
            conn = new SQLiteConnection(_dbPath);
            conn.Insert(typeModel);
        }

        public void Delete(int itemId) 
        {
            conn = new SQLiteConnection(_dbPath);

            conn.Delete(new { Id = itemId });
        }

        public int ColorCount()
        {
            conn = new SQLiteConnection(_dbPath);
            return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM ColorsList");
        }
        public void AddColors(List<ColorModel> colors)
        {
            conn = new SQLiteConnection(_dbPath);
            foreach (var color in colors)
            {
                var colorsList = new ColorsList
                {
                    ColorName = color.Name,
                    ColorCode = color.HexCode,
                    ColorStatus = color.Status
                };

                conn.Insert(colorsList);
            }
        }
        public void UsedColor(int colorId)
        {
            conn = new SQLiteConnection(_dbPath);

            var colorItem = conn.Table<ColorsList>().FirstOrDefault(c => c.Id == colorId);

            if (colorItem != null)
            {
                colorItem.ColorStatus = "used";

                conn.Update(colorItem);
            }
        }

        public DateTime GetOldestDate()
        {
            conn = new SQLiteConnection(_dbPath);

            var oldestDate = conn.Table<DataModel>().Min(x => x.dateTime);
            return oldestDate;
        }

        public void UpdateItem(int selectedID, DataModel updatedModel)
        {
            DataModel existingModel = conn.Table<DataModel>().FirstOrDefault(model => model.Id == selectedID);

            if (existingModel != null)
            {
                existingModel.Type = updatedModel.Type;
                existingModel.dateTime = updatedModel.dateTime;
                existingModel.Amount = updatedModel.Amount;
                existingModel.dataHexColor = updatedModel.dataHexColor;

                conn.Update(existingModel);
            }
        }
    }
}
