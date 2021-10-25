package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn02_ArchiveLargeProductionStore(shObj: SuperHelper) {
    /* #region Request Parameters */
        var archivePSLargeProductionStoreId = ""
        val csvFeederProductionDataWIP = csv(fileName="productiondataWIP.csv").circular
    /* #endregion Request Parameters */

    /* #region HTTP REQUEST */
        def Scn02_ArchiveLargeProductionStore() = 
        {
            scenario("Scn02_ArchiveLargeProductionStore").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn02_ArchiveLargeProductionStore") seconds)
                .feed(csvFeederProductionDataWIP)
                .exec(session => { archivePSLargeProductionStoreId = session("ArchivePSLargeProductionStoreId").as[String]; session })
                .exec(_.set("wipLargeProductionStoreId",archivePSLargeProductionStoreId))
                .exec(
                (http("Scn02_ArchiveLargeProductionStore")
                    .post(shObj.conObj.archiveLargeProductionStoreAPI))
                    .header("Content-Type", "application/json") 
                    .check(status.is(shObj.conObj.acceptedStatus))
                    .check(substring("statusQueryGetUri").exists)
                    .check(substring("purgeHistoryDeleteUri").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */ 
  
}
