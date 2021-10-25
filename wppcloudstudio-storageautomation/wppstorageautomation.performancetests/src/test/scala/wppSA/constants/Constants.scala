package wppSA.constants

import com.typesafe.config.ConfigFactory
import java.time.LocalDate

class Constants
{
    /* #region Variables */ 
        //Read config values from app.conf
        val defaultStatus: Int = ConfigFactory.load().getInt("app.status.http200")
        val acceptedStatus: Int = ConfigFactory.load().getInt("app.status.http202")
        val testDuration = ConfigFactory.load().getString("app.runSettings.loadTestDuration").toInt
        val isPacingEnabled =ConfigFactory.load().getString("app.runSettings.pacingEnabled").toBoolean
        val envHostnameWPPSA = ConfigFactory.load().getString("app.environment.hostnameWPPSA")
        val bearertoken = ConfigFactory.load().getString("app.secrets.bearertoken")
        val apimSubscriptionKey = ConfigFactory.load().getString("app.secrets.apimSubscriptionKey")
        //val pauseDuration = ConfigFactory.load().getString("app.runSettings.pauseDuration").toInt
        //val targetRPS = ConfigFactory.load().getString("app.runSettings.targetRPS").toInt
        //Read API Endpoints from config
        val archiveAllAPI = ConfigFactory.load().getString("app.endpoint.archiveAllAPI")
        val archiveLargeProductionStoreAPI = ConfigFactory.load().getString("app.endpoint.archiveLargeProductionStoreAPI")
        val archiveMediumProductionStoreAPI = ConfigFactory.load().getString("app.endpoint.archiveMediumProductionStoreAPI")
        val archiveSmallProductionStoreAPI = ConfigFactory.load().getString("app.endpoint.archiveSmallProductionStoreAPI")
        val createProductionAPI = ConfigFactory.load().getString("app.endpoint.createProductionAPI")
        val listProductionAPI = ConfigFactory.load().getString("app.endpoint.listProductionAPI")
        val restoreLargeProductionAPI = ConfigFactory.load().getString("app.endpoint.restoreLargeProductionAPI")
        val restoreMediumProductionAPI = ConfigFactory.load().getString("app.endpoint.restoreMediumProductionAPI")
        val restoreSmallProductionAPI = ConfigFactory.load().getString("app.endpoint.restoreSmallProductionAPI")
        val listProductionStoreAPI = ConfigFactory.load().getString("app.endpoint.listProductionStoreAPI")
        val restoreDeltaProductionAPI = ConfigFactory.load().getString("app.endpoint.restoreDeltaProductionAPI")
        val getStatusQueryAPI = ConfigFactory.load().getString("app.endpoint.getStatusQueryAPI")
        val makeOfflineLargeProductionAPI = ConfigFactory.load().getString("app.endpoint.makeOfflineLargeProductionAPI")
        val makeOfflineMediumProductionAPI = ConfigFactory.load().getString("app.endpoint.makeOfflineMediumProductionAPI")
        val makeOfflineSmallProductionAPI = ConfigFactory.load().getString("app.endpoint.makeOfflineSmallProductionAPI")        
        //Script Variables
        //val productionStoreSmall: String = ConfigFactory.load().getString("app.vars.productionStoreSmall")
        //val productionStoreMedium: String = ConfigFactory.load().getString("app.vars.productionStoreMedium")
        //val productionStoreLarge: String = ConfigFactory.load().getString("app.vars.productionStoreLarge")
        //val productionStoreLargeContainer: String = ConfigFactory.load().getString("app.vars.productionStoreLargeContainer")
        //val productionStoreMediumContainer: String = ConfigFactory.load().getString("app.vars.productionStoreMediumContainer")
        //val productionStoreSmallContainer: String = ConfigFactory.load().getString("app.vars.productionStoreSmallContainer")
        //val productionLargeGUID: String = ConfigFactory.load().getString("app.vars.productionLargeGUID")
        //val productionMediumGUID: String = ConfigFactory.load().getString("app.vars.productionMediumGUID")
        //val productionSmallGUID: String = ConfigFactory.load().getString("app.vars.productionSmallGUID")
        //val productionStoreName: String = "perftestfileshare"
        //Read UserLoad Concurrency from app.conf
        val userloadScn01_ArchiveAllProductionStores = ConfigFactory.load().getString("app.scenarioConcurrency.Scn01_ArchiveAllProductionStores").toInt
        val userloadScn02_ArchiveLargeProductionStore = ConfigFactory.load().getString("app.scenarioConcurrency.Scn02_ArchiveLargeProductionStore").toInt
        val userloadScn03_ArchiveMediumProductionStore = ConfigFactory.load().getString("app.scenarioConcurrency.Scn03_ArchiveMediumProductionStore").toInt
        val userloadScn04_ArchiveSmallProductionStore = ConfigFactory.load().getString("app.scenarioConcurrency.Scn04_ArchiveSmallProductionStore").toInt
        val userloadScn05_CreateProduction = ConfigFactory.load().getString("app.scenarioConcurrency.Scn05_CreateProduction").toInt
        val userloadScn06_ListProduction = ConfigFactory.load().getString("app.scenarioConcurrency.Scn06_ListProduction").toInt
        val userloadScn07_RestoreLargeProduction = ConfigFactory.load().getString("app.scenarioConcurrency.Scn07_RestoreLargeProduction").toInt
        val userloadScn08_RestoreMediumProduction = ConfigFactory.load().getString("app.scenarioConcurrency.Scn08_RestoreMediumProduction").toInt
        val userloadScn09_RestoreSmallProduction = ConfigFactory.load().getString("app.scenarioConcurrency.Scn09_RestoreSmallProduction").toInt
        val userloadScn10_ListProductionStores = ConfigFactory.load().getString("app.scenarioConcurrency.Scn10_ListProductionStores").toInt
        val userloadScn11_MakeOfflineLargeProduction = ConfigFactory.load().getString("app.scenarioConcurrency.Scn11_MakeOfflineLargeProduction").toInt
        val userloadScn12_MakeOfflineMediumProduction = ConfigFactory.load().getString("app.scenarioConcurrency.Scn12_MakeOfflineMediumProduction").toInt
        val userloadScn13_MakeOfflineSmallProduction = ConfigFactory.load().getString("app.scenarioConcurrency.Scn13_MakeOfflineSmallProduction").toInt
        val userloadScn14_RestoreDeltaProduction = ConfigFactory.load().getString("app.scenarioConcurrency.Scn14_RestoreDeltaProduction").toInt        
        val userloadScn15_GetStatusQueryAPI = ConfigFactory.load().getString("app.scenarioConcurrency.Scn15_GetStatusQueryAPI").toInt
    /* #endregion Variables */
}