package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn13_MakeOfflineSmallProduction(shObj: SuperHelper) {
    /* #region Request Parameters */
        var makeOfflineSmallProductionStoreId = ""
        var makeOfflineSmallProductionId = ""
        val csvFeederProductionDataWIP = csv(fileName="productiondataWIP.csv").circular
    /* #endregion Request Parameters */

    /* #region HTTP REQUEST */
        def Scn13_MakeOfflineSmallProduction() = 
        {
            scenario("Scn13_MakeOfflineSmallProduction").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn13_MakeOfflineSmallProduction") seconds)
                .feed(csvFeederProductionDataWIP)
                .exec(session => { makeOfflineSmallProductionStoreId = session("MOPSmallProductionStoreId").as[String]; session })
                .exec(_.set("makeOfflineSmallProductionStoreId",makeOfflineSmallProductionStoreId))
                .exec(session => { makeOfflineSmallProductionId = session("MOPSmallProductionId").as[String]; session })
                .exec(_.set("makeOfflineSmallProductionId",makeOfflineSmallProductionId))
                .exec(
                (http("Scn13_MakeOfflineSmallProduction")
                    .post(shObj.conObj.makeOfflineSmallProductionAPI))
                    .check(status.is(shObj.conObj.acceptedStatus))
                    .check(substring("statusQueryGetUri").exists)
                    .check(substring("purgeHistoryDeleteUri").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */   
}
