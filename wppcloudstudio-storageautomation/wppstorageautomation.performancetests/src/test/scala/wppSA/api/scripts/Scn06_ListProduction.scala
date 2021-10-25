package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn06_ListProduction(shObj: SuperHelper) {
    /* #region Request Parameters */
        var archivePSLargeProductionStoreId = ""
        val csvFeederProductionDataWIP = csv(fileName="productiondataWIP.csv").random
    /* #endregion Request Parameters */

    /* #region HTTP REQUEST */
        def Scn06_ListProduction() = 
        {
            scenario("Scn06_ListProduction").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn06_ListProduction") seconds)
                .feed(csvFeederProductionDataWIP)
                .exec(session => { archivePSLargeProductionStoreId = session("ArchivePSLargeProductionStoreId").as[String]; session })
                .exec(_.set("wipLargeProductionStoreId",archivePSLargeProductionStoreId))
                .exec(
                (http("Scn06_ListProduction")
                    .get(shObj.conObj.listProductionAPI))
                    .check(status.is(shObj.conObj.defaultStatus))
                    .check(substring("productionList").exists)
                    .check(substring("productionStore").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */ 
  
}
