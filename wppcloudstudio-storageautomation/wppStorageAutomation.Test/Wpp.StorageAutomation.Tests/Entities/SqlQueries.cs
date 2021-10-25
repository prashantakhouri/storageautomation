// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlQueries.cs" company="Microsoft">
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//    OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//    ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//    OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Wpp.StorageAutomation.Tests.Entities
{
    using Bdd.Core;

    public static class SqlQueries
    {
        // SQL query
        public static readonly string InsertProductionStore = $"INSERT INTO {DbCatalog}.[dbo].[ProductionStore] (Id, Name, Region, ManagerRoleGroupNames, UserRoleGroupNames, WIPKeyName, ArchiveKeyName) VALUES (@psId, @productionStoreName, @region, @mGroup, @uGroup, @wipKey, @arcKey)";

        public static readonly string GetTopOneProductionDetails = $"SELECT TOP 1 * FROM {DbCatalog}.[dbo].[Production]";
        public static readonly string GetSpecificProductionDetails = $"SELECT TOP 1 * FROM {DbCatalog}.[dbo].[Production] WHERE Name = @name ORDER BY CreatedDateTime DESC";
        public static readonly string GetProductionDetailsByProductionStore = $"SELECT * FROM {DbCatalog}.[dbo].[Production] WHERE ProductionStoreId = @productionStoreId";
        public static readonly string GetSpecificProductionDetailsInProductionStore = $"SELECT TOP 1 * FROM {DbCatalog}.[dbo].[Production] WHERE Name = @name and ProductionStoreId = @productionStoreId ORDER BY CreatedDateTime DESC";
        public static readonly string GetAllProductionDetails = $"SELECT * FROM {DbCatalog}.[dbo].[Production]";
        public static readonly string GetAllProductionStoresDetails = $"SELECT * FROM {DbCatalog}.[dbo].[ProductionStore]";
        public static readonly string GetGuidOfPS = $"SELECT id FROM {DbCatalog}.[dbo].[ProductionStore] WHERE Name = @name";
        public static readonly string GetGuidOfProd = $"SELECT id FROM {DbCatalog}.[dbo].[Production] WHERE Name = @name";
        public static readonly string GetNameOfProdUsingId = $"SELECT Name FROM {DbCatalog}.[dbo].[Production] WHERE id = @id";
        public static readonly string GetProductionsNameInProductionStore = $"SELECT Name, CreatedDateTime, Status FROM {DbCatalog}.[dbo].[Production] WHERE ProductionStoreId = @productionStoreId";
        public static readonly string GetDistinctRegions = $"SELECT distinct(Region) from[dbo].[ProductionStore]";
        public static readonly string GetFirstPSOnUI = $"SELECT top (1) Name from [dbo].[ProductionStore] where Region in (select top (1) Region from [dbo].[ProductionStore] order by Region) order by Name";
        public static readonly string GetPSForARegion = $"Select * from [dbo].[ProductionStore] where Region = @Region";
        public static readonly string GetProductionStoreDetails = $"SELECT * FROM {DbCatalog}.[dbo].ProductionStore WHERE Name = @name";
        public static readonly string GetDistinctWIPKeyNames = $"Select DISTINCT WIPKeyName from {DbCatalog}.[dbo].[ProductionStore]";
        public static readonly string GetDistinctArchiveKeyNames = $"Select DISTINCT ArchiveKeyName from {DbCatalog}.[dbo].[ProductionStore]";
        public static readonly string GetPSForAGroup = $"Select * from [dbo].[ProductionStore] where UserRoleGroupNames like @groupName or ManagerRoleGroupNames like @groupName";

        public static readonly string DeleteAllProductions = $"DELETE FROM {DbCatalog}.[dbo].Production";
        public static readonly string DeleteProductionFromProductionStore = $"DELETE FROM {DbCatalog}.[dbo].Production Where Name = @name and ProductionStoreId = @productionStoreId";
        public static readonly string DeleteAllProductionStores = $"DELETE FROM {DbCatalog}.[dbo].ProductionStore";
        public static readonly string DeleteAProductionStore = $"DELETE FROM {DbCatalog}.[dbo].ProductionStore WHERE Name = @name";

        // public static readonly string DeleteAllProductions = $"DELETE FROM {DbCatalog}.[dbo].Production WHERE productionstoreid NOT IN ('productionstore-a')";
        // public static readonly string DeleteAllProductionStores = $"DELETE FROM {DbCatalog}.[dbo].ProductionStore WHERE id NOT IN ('productionstore-a')";
        public static readonly string UpdateProductionStatus = $"UPDATE {DbCatalog}.[dbo].[Production] SET Status = @status WHERE Name = @productionName";
        public static readonly string UpdateProductionStatusById = $"UPDATE {DbCatalog}.[dbo].[Production] SET Status = @status WHERE Id = @id";
        public static readonly string UpdateProductionStoreManagerGroup = $"UPDATE {DbCatalog}.[dbo].[ProductionStore] SET ManagerRoleGroupNames = @managerRoleGroupNames WHERE Id = @id";
        public static readonly string UpdateProductionStoreUserGroup = $"UPDATE {DbCatalog}.[dbo].[ProductionStore] SET UserRoleGroupNames = @userRoleGroupNames WHERE Id = @id";
        public static readonly string UpdateProductionSizeByProductionName = $"UPDATE {DbCatalog}.[dbo].[Production] SET SizeInBytes = @numberOfBytes where Name = @name";

        // Write SQL Queries before this line
        private static string DbCatalog => ConfigManager.AppSettings[nameof(DbCatalog)];
    }
}
