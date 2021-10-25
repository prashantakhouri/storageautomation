using System.Collections.Generic;
using System.Threading.Tasks;
using Wpp.StorageAutomation.Entities.Models;
using ProductionEntity = Wpp.StorageAutomation.Entities.Models.Production;

namespace Wpp.StorageAutomation.ProductionControl.Repository
{
    /// <summary>
    /// Production repository.
    /// </summary>
    public interface IProductionRepository
    {
        /// <summary>
        /// Gets all productions.
        /// </summary>
        /// <returns>A list of all productions.</returns>
        Task<IEnumerable<ProductionEntity>> GetAllProductions();

        /// <summary>
        /// Adds the production.
        /// </summary>
        /// <param name="production">The production.</param>
        /// <returns>The task.</returns>
        Task AddProduction(ProductionEntity production);

        /// <summary>
        /// Deletes the production.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task.</returns>
        Task DeleteProduction(string id);

        /// <summary>
        /// Updates the production.
        /// </summary>
        /// <param name="production">The production.</param>
        /// <returns>The task.</returns>
        Task UpdateProduction(ProductionEntity production);

        /// <summary>
        /// Productions the exists.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="productionName">Name of the production.</param>
        /// <returns>A Production with specified Production Store Id and name.</returns>
        Task<ProductionEntity> GetProductionByName(string productionStoreId, string productionName);

        /// <summary>
        /// Gets the production by production store identifier.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <returns>List of Productions belonging to a Production Store Id.</returns>
        Task<IEnumerable<ProductionEntity>> GetProductionByProductionStoreId(string productionStoreId);
    }
}