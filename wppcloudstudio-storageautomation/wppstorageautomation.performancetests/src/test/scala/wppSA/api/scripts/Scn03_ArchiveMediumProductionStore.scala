package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn03_ArchiveMediumProductionStore(shObj: SuperHelper) {
    /* #region Request Parameters */
        var archivePSMediumProductionStoreId = ""
        val csvFeederProductionDataWIP = csv(fileName="productiondataWIP.csv").circular
    /* #endregion Request Parameters */

    /* #region HTTP REQUEST */
        def Scn03_ArchiveMediumProductionStore() = 
        {
            scenario("Scn03_ArchiveMediumProductionStore").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn03_ArchiveMediumProductionStore") seconds)
                .feed(csvFeederProductionDataWIP)
                .exec(session => { archivePSMediumProductionStoreId = session("ArchivePSMediumProductionStoreId").as[String]; session })
                .exec(_.set("wipMediumProductionStoreId",archivePSMediumProductionStoreId))
                .exec(
                (http("Scn03_ArchiveMediumProductionStore")
                    .post(shObj.conObj.archiveMediumProductionStoreAPI))
                    .header("Content-Type", "application/json") 
                    .check(status.is(shObj.conObj.acceptedStatus))
                    .check(substring("statusQueryGetUri").exists)
                    .check(substring("purgeHistoryDeleteUri").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */ 
  
}
