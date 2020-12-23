using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OpenMod.API.Ioc;

namespace SimpleOpenModLiteDB
{
    [Service]
    public interface ILiteDatabase<T> : IEnumerable<T>
    {
        void Add(T item);
    }
}