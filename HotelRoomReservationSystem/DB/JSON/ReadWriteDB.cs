
namespace HotelRoomReservationSystem.DB.JSON
{
    public class ReadWriteDB
    {
        private void CreateDataBase(string databasePath, string path)
        {
            string dbPath = databasePath + path;

            if (!Directory.Exists(databasePath))
            {
                try
                { Directory.CreateDirectory(databasePath); }
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

        public virtual string GetFileContent(string databasePath, string path)
        {
            string dbPath = databasePath + path;

            CreateDataBase(databasePath, path);

            string fileContent = "";
            try
            {
                fileContent = File.ReadAllText(dbPath);
            }
            catch (Exception)
            { throw new ApplicationException("Can't read database"); }

            return fileContent;
        }

        public virtual void WriteToFile(string content, string databasePath, string path)
        {
            string dbPath = databasePath + path;

            CreateDataBase(databasePath, path);

            try
            { File.WriteAllText(dbPath, content); }
            catch (Exception)
            { throw new ApplicationException("Can't write to database"); }
        }
    }
}
