using System;
using System.ComponentModel.DataAnnotations;

namespace UnitusCore.Models.BaseClasses
{
    public abstract class ModelBase
    {
        public void GenerateId()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
    }
}