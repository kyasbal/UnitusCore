 ECHO "リモートデータベースの移行を有効化します。対象DBC:BasicDbContext"
$RemoteConnectionString="Data Source=tcp:cv4qa57efn.database.windows.net,1433;Initial Catalog=unitus-core_db;User ID=UnitusCore@cv4qa57efn;Password=wNTdzCtn%d";
$RemoteProviderType="System.Data.SqlClient";
 ECHO $RemoteConnectionString
 Enable-Migrations　-ContextTypeName UnitusCore.Models.BasicDbContext -StartUpProjectName "UnitusCore" -ConnectionString RemoteConnectionString -ConnectionProviderName RemoteConnectionProvider
 ECHO "SUCCESSFUL COMPLETION";