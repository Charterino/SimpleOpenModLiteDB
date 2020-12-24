using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.Runtime;

namespace SimpleOpenModLiteDB
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class LiteDatabase<T> : ILiteDatabase<T>, IDisposable
    {
        public string Path;
        public ILiteCollection<T> Collection;

        private LiteDatabase _database;
        
        public LiteDatabase(IRuntime runtime)
        {
            var databaseFolderPath = System.IO.Path.Combine(runtime.WorkingDirectory, "databases");
            if (!Directory.Exists(databaseFolderPath))
            {
                Directory.CreateDirectory(databaseFolderPath);
            }
            Path = System.IO.Path.Combine(databaseFolderPath, typeof(T).Name + ".db");
            _database = new LiteDatabase(BuildSharedConnectionString(Path));
            Collection = _database.GetCollection<T>(typeof(T).Name);
        }

        //this is a temporal solution. Instead of making the service singleton (which I couldnt do) we allow shared connections.
        public static string BuildSharedConnectionString(string path)
        {
            return $"Filename={path};Connection=shared";
        }
        
        public void Dispose()
        {
            _database.Dispose();
        }

        public void Add(T item)
        {
            Collection.Insert(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Collection.FindAll().GetEnumerator();
        }

        public void Update(T item)
        {
            Collection.Update(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}