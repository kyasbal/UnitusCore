using System;

namespace UnitusCore.Storage.Base
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BlobStorageAttribute:Attribute
    {
        public string ContainerName { get; private set; }

        public BlobStorageAttribute(string containerName)
        {
            ContainerName = containerName;
        }
    }
}