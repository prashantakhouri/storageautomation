package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn12_MakeOfflineMediumProduction(shObj: SuperHelper) {
    /* #region Request Parameters */
        var makeOfflineMediumProductionStoreId = ""
        var makeOfflineMediumProductionId = ""
        val csvFeederProductionDataWIP = csv(fileName="productiondataWIP.csv").circular
    /* #endregion Request Parameters */

        /* #region HTTP REQUEST */
        def Scn12_MakeOfflineMediumProduction() = 
        {
            scenario("Scn12_MakeOfflineMediumProduction").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn12_MakeOfflineMediumProduction") seconds)
                .feed(csvFeederProductionDataWIP)
                .exec(session => { makeOfflineMediumProductionStoreId = session("MOPMediumProductionStoreId").as[String]; session })
                .exec(_.set("makeOfflineMediumProductionStoreId",makeOfflineMediumProductionStoreId))
                .exec(session => { makeOfflineMediumProductionId = session("MOPMediumProductionId").as[String]; session })
                .exec(_.set("makeOfflineMediumProductionId",makeOfflineMediumProductionId))
                .exec(
                (http("Scn12_MakeOfflineMediumProduction")
                    .post(shObj.conObj.makeOfflineMediumProductionAPI))
                    .check(status.is(shObj.conObj.acceptedStatus))
                    .check(substring("statusQueryGetUri").exists)
                    .check(substring("purgeHistoryDeleteUri").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */   
    
}
