package wppSA.api.loadtest

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import wppSA.entities._
import wppSA.api.scripts._

class LoadTest extends Simulation
{
    /* #region Objects */
        val shObj = new SuperHelper()
    /* #endregion Objects */

    /* #region Variables */
        val testDuration = shObj.conObj.testDuration
    /* #endregion Variables */

    //Start Execution with Print Statements
    println("STARTING EXECUTION")
    println("Load Test Run Duration (in seconds): " + shObj.conObj.testDuration)
    //println("Load Test:: Expected Target RPS : " + shObj.conObj.targetRPS)

    //Define HTTP Protocol
    val httpProtocolWPPSA = http.baseUrl(shObj.conObj.envHostnameWPPSA).header("AppAuthToken", "Bearer " + shObj.conObj.bearertoken)

    //HTTP REQUESTS for Load Test Execution 
    setUp(
          new Scn01_ArchiveAllProductionStores(shObj).Scn01_ArchiveAllProductionStores()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn01_ArchiveAllProductionStores) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),
          new Scn02_ArchiveLargeProductionStore(shObj).Scn02_ArchiveLargeProductionStore()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn02_ArchiveLargeProductionStore) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),
          new Scn03_ArchiveMediumProductionStore(shObj).Scn03_ArchiveMediumProductionStore()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn03_ArchiveMediumProductionStore) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),
          new Scn04_ArchiveSmallProductionStore(shObj).Scn04_ArchiveSmallProductionStore()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn04_ArchiveSmallProductionStore) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),
          new Scn05_CreateProduction(shObj).Scn05_CreateProduction()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn05_CreateProduction) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),
          new Scn06_ListProduction(shObj).Scn06_ListProduction()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn06_ListProduction) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),
          new Scn07_RestoreLargeProduction(shObj).Scn07_RestoreLargeProduction()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn07_RestoreLargeProduction) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),
          new Scn08_RestoreMediumProduction(shObj).Scn08_RestoreMediumProduction()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn08_RestoreMediumProduction) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),
          new Scn09_RestoreSmallProduction(shObj).Scn09_RestoreSmallProduction()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn09_RestoreSmallProduction) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),
          new Scn10_ListProductionStores(shObj).Scn10_ListProductionStores()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn10_ListProductionStores) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),
          new Scn11_MakeOfflineLargeProduction(shObj).Scn11_MakeOfflineLargeProduction()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn11_MakeOfflineLargeProduction) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),
          new Scn12_MakeOfflineMediumProduction(shObj).Scn12_MakeOfflineMediumProduction()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn12_MakeOfflineMediumProduction) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),          
          new Scn13_MakeOfflineSmallProduction(shObj).Scn13_MakeOfflineSmallProduction()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn13_MakeOfflineSmallProduction) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),   
          new Scn14_RestoreDeltaProduction(shObj).Scn14_RestoreDeltaProduction()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn14_RestoreDeltaProduction) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA),
          new Scn15_GetStatusQueryAPI(shObj).Scn15_GetStatusQueryAPI()
            .inject(constantConcurrentUsers(shObj.conObj.userloadScn15_GetStatusQueryAPI) during (shObj.conObj.testDuration seconds))
            .protocols(httpProtocolWPPSA)             
          )
    //.throttle(reachRps(shObj.conObj.targetRPS) in (1 milliseconds), holdFor(testDuration seconds))
}