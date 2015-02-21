using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels;

namespace UnitusCore.Storage
{
    public class IdPairContainerStorage
    {
        private readonly TableStorageConnection _storageConnection;

        private readonly CloudTable _idPairTable;

        public IdPairContainerStorage(TableStorageConnection storageConnection)
        {
            _storageConnection = storageConnection;
            _idPairTable = _storageConnection.TableClient.GetTableReference("IdPairContainer");
            _idPairTable.CreateIfNotExists();
        }
        /// <summary>
        /// 指定したIDのペアを作成します
        /// </summary>
        /// <param name="firstId"></param>
        /// <param name="secoundId"></param>
        /// <param name="idType1"></param>
        /// <param name="idType2"></param>
        /// <returns></returns>
        public async Task MakePair(string firstId,string secoundId,string idType1,string idType2)
        {
            await InsertPair(firstId, secoundId, idType1, idType2);
            await InsertPair(secoundId, firstId, idType2, idType1);
        }

        private async Task InsertPair(string firstId, string secoundId, string idType1, string idType2)
        {
            if (await IsStored(firstId, idType1, idType2)) return;
            string partitionKey = getPartitionKey(idType1, idType2);
            string rowKey = firstId;
            await _idPairTable.ExecuteAsync(TableOperation.InsertOrReplace(new IdPairContainer(partitionKey,rowKey,secoundId)));
        }

        public async Task<bool> IsStored(string firstId, string idType1, string idType2)
        {
            var result = await RetrieveIdResult(firstId, idType1, idType2);
            return result.Result != null;
        }

        private async Task<TableResult> RetrieveIdResult(string firstId, string idType1, string idType2)
        {
            var result = await
                _idPairTable.ExecuteAsync(TableOperation.Retrieve<IdPairContainer>(getPartitionKey(idType1, idType2),
                    firstId));
            return result;
        }

        private string getPartitionKey(string idType1, string idType2)
        {
            return idType1 + "-" + idType2;
        }

        /// <summary>
        /// 指定したIDとタイプに関連付けされたIDを取得します
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idType1"></param>
        /// <param name="idType2"></param>
        /// <returns></returns>
        public async Task<string> RetrieveId(string id, string idType1, string idType2)
        {
            var result = await RetrieveIdResult(id, idType1, idType2);
            var resultContainer = ((IdPairContainer) result.Result);
            return resultContainer != null ? resultContainer.TargetId : null;
        }
    }
}