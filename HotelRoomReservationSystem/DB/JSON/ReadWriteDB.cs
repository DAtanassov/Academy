
using System.Text.Json;

namespace HotelRoomReservationSystem.DB.JSON
{
    public class ReadWriteDB<T>
    {
        protected readonly string _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data") + Path.DirectorySeparatorChar;

        private void CreateDataBase(string path)
        {
            string dbPath = _dbPath + path;

            if (!Directory.Exists(_dbPath))
            {
                try
                { Directory.CreateDirectory(_dbPath); }
                catch (Exception)
                { throw new ApplicationException("Can't create directory database."); }
            }

            if (!File.Exists(dbPath))
            {
                try
                {
                    FileStream file = File.Create(dbPath);
                    file.Close();
                }
                catch (Exception)
                { throw new ApplicationException("Can't write to database"); }
            }

        }

        private string GetFileContent(string path)
        {
            string dbPath = _dbPath + path;

            CreateDataBase(path);

            string fileContent = "";
            try
            {
                fileContent = File.ReadAllText(dbPath);
            }
            catch (Exception e)
            { throw new ApplicationException($"Can't read database.\n{e.Message}"); }

            return fileContent;
        }

        public List<T> GetAllItemsFromFile(string path)
        {
            string fileContent = GetFileContent(path);
            List<T>? items = new List<T>();

            if (!string.IsNullOrEmpty(fileContent))
                try
                { items = JsonSerializer.Deserialize<List<T>>(fileContent); }
                catch (Exception e)
                { throw new ApplicationException($"Can't read database.\n{e.Message}"); }


            if (items == null)
                return new List<T>();

            return items;
        }

        public virtual void WriteToFile(List<T> items, string path)
        {
            string fileContent = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });

            string dbPath = _dbPath + path;

            CreateDataBase(path);

            try
            { File.WriteAllText(dbPath, fileContent); }
            catch (Exception e)
            { throw new ApplicationException($"Can't write to database.\n{e.Message}"); }
        }
    }
}
