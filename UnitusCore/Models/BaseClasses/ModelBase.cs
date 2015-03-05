using System;
using System.ComponentModel.DataAnnotations;

namespace UnitusCore.Models.BaseClasses
{
    public abstract class ModelBase
    {
        public static T CreateNewEntity<T>()where T:ModelBase,new()
        {
            var t = new T();
            t.GenerateId();
            return t;
        }

        public void GenerateId()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
    }
}