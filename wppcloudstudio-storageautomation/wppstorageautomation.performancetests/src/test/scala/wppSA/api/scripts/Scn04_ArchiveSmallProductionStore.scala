package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn04_ArchiveSmallProductionStore(shObj: SuperHelper) {
    /* #region Request Parameters */
        var archivePSSmallProductionStoreId = ""
        val csvFeederProductionDataWIP = csv(fileName="productiondataWIP.csv").circular
    /* #endregion Request Parameters */

    /* #region HTTP REQUEST */
        def Scn04_ArchiveSmallProductionStore() = 
        {
            scenario("Scn04_ArchiveSmallProductionStore").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn04_ArchiveSmallProductionStore") seconds)
                .feed(csvFeederProductionDataWIP)
                .exec(session => { archivePSSmallProductionStoreId = session("ArchivePSSmallProductionStoreId").as[String]; session })
                .exec(_.set("wipSmallProductionStoreId",archivePSSmallProductionStoreId))
                .exec(
                (http("Scn04_ArchiveSmallProductionStore")
                    .post(shObj.conObj.archiveSmallProductionStoreAPI))
                    .header("Content-Type", "application/json")
                    .check(status.is(shObj.conObj.acceptedStatus))
                    .check(substring("statusQueryGetUri").exists)
                    .check(substring("purgeHistoryDeleteUri").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */ 
  
}
