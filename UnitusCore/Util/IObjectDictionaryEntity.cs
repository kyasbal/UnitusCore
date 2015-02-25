using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitusCore.Util
{
    public interface IObjectDictionaryEntity
    {
        string Name { get; }
    }

    public class ObjectDictionary<T> : Dictionary<string, T> where T : IObjectDictionaryEntity
    {
        public void Add(T entity)
        {
            if (this.ContainsKey(entity.Name))
            {
                this[entity.Name] = entity;
            }
            else
            {
                Add(entity.Name,entity);
            }
        }
    }

    public abstract class CalculatableObjectDictionary<T> : Dictionary<string, T> where T : IObjectDictionaryEntity
    {
        public void Add(T entity)
        {
            if (this.ContainsKey(entity.Name))
            {
                this[entity.Name] = Update(this[entity.Name], entity);
            }
            else
            {
                Add(entity.Name, entity);
            }
        }

        protected abstract T Update(T oldEntity, T newEntity);
    }
}