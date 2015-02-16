using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace UnitusCore.Storage.Base
{
    public class AzureStorageConnection
    {
        public AzureStorageConnection()
        {
            StorageAccount = new CloudStorageAccount(new StorageCredentials("unituscorestorage", "XCmDomSGAob09h939pf2+f7rlC1lIN1TEUHidqwFMU8vXGYbovZbxAUnmzqi0ItFjn57kG3wzWlGV1q6xcEsPw=="),useHttps:false);
            GenDebugAccount();
        }

        [Conditional("DEBUG")]
        private void GenDebugAccount()
        {
            StorageAccount=CloudStorageAccount.DevelopmentStorageAccount;
        }

        protected CloudStorageAccount StorageAccount { get; set; }
    }
}