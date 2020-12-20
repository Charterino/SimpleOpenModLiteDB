using System;
using System.IO;
using LiteDB;

namespace SimpleOpenModLiteDB
{
    public class Database<T>
    {
        public string Path;
        public ILiteCollection<T> Collection;

        private LiteDatabase _database;
        
        public Database()
        {
            var databaseFolderPath = string.Concat(new string[]
            {
                Directory.GetCurrentDirectory(),
                System.IO.Path.DirectorySeparatorChar.ToString(),
                "Databases",
                System.IO.Path.DirectorySeparatorChar.ToString()
            });
            if (!Directory.Exists(databaseFolderPath))
            {
                Directory.CreateDirectory(databaseFolderPath);
            }
            Path = databaseFolderPath + GetType().Name;
            _database = new LiteDatabase(Path);
            Collection = _database.GetCollection<T>(GetType().Name);
        }
        
        ~Database()
        {
            _database.Dispose();
        }
    }
}