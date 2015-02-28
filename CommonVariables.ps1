$global:RemoteConnectionString="Data Source=tcp:cv4qa57efn.database.windows.net,1433;Initial Catalog=unitus-core_db;User ID=UnitusCore@cv4qa57efn;Password=wNTdzCtn%d";
$global:RemoteProviderName="System.Data.SqlClient";
$global:RemoteProjectName="UnitusCore"
$global:BasicDbContext="UnitusCore.Models.BasicDbContext";
$global:ApplicationDbContext="UnitusCore.Models.ApplicationDbContext";
function global:Update-RemoteDatabase()
{
	Update-Database -Verbose -ConnectionString $global:RemoteConnectionString -ConnectionProviderName $global:RemoteProviderName
}
function global:Add-RemoteMigration([String]$mn)
{
	Add-Migration $mn -ProjectName $global:RemoteProjectName -ConnectionString $global:RemoteConnectionString -ConnectionProviderName $global:RemoteProviderName 
}
function global:Enable-RemoteMigrations([String] $tn)
{
	Enable-Migrations -Force  -ConnectionString $global:RemoteConnectionString -ConnectionProviderName $global:RemoteProviderName -ContextTypeName $tn -ProjectName $global:RemoteProjectName 
}
function global:Reload-CommonFunction()
{
	.\CommonVariables.ps1
}
ECHO "Common variables was loaded!"