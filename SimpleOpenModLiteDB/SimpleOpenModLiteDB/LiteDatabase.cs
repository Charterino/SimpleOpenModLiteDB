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
using ILogger = Serilog.ILogger;

namespace SimpleOpenModLiteDB
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class LiteDatabase<T> : ILiteDatabase<T>, IDisposable
    {
        public string Path;
        public ILiteCollection<T> Collection;

        private LiteDatabase _database;

        private readonly ILogger<LiteDatabase> m_Logger;
        
        public LiteDatabase(IOpenModHost host, ILogger<LiteDatabase> logger)
        {
            m_Logger = logger;
            var databaseFolderPath = System.IO.Path.Combine(host.WorkingDirectory, "databases");
            if (!Directory.Exists(databaseFolderPath))
            {
                Directory.CreateDirectory(databaseFolderPath);
            }
            Path = System.IO.Path.Combine(databaseFolderPath, typeof(T).Name + ".db");
            _database = new LiteDatabase(BuildSharedConnectionString(Path));
            
            Collection = _database.GetCollection<T>(typeof(T).Name);
            _database.Commit();
        }

        //this is a temporal solution. Instead of making the service singleton (which I couldnt do) we allow shared connections.
        public static string BuildSharedConnectionString(string path)
        {
            return $"Filename={path};Connection=Shared";
        }
        
        public void Dispose()
        {
            m_Logger.LogInformation("DISPOSING");
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
            m_Logger.LogInformation($"UPDATING {item}");
            m_Logger.LogInformation(Collection.Update(item).ToString());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}