package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn11_MakeOfflineLargeProduction(shObj: SuperHelper) {
    /* #region Request Parameters */
        var makeOfflineLargeProductionStoreId = ""
        var makeOfflineLargeProductionId = ""
        val csvFeederProductionDataWIP = csv(fileName="productiondataWIP.csv").circular
    /* #endregion Request Parameters */

    /* #region HTTP REQUEST */
        def Scn11_MakeOfflineLargeProduction() = 
        {
            scenario("Scn11_MakeOfflineLargeProduction").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn11_MakeOfflineLargeProduction") seconds)
                .feed(csvFeederProductionDataWIP)
                .exec(session => { makeOfflineLargeProductionStoreId = session("MOPLargeProductionStoreId").as[String]; session })
                .exec(_.set("makeOfflineLargeProductionStoreId",makeOfflineLargeProductionStoreId))
                .exec(session => { makeOfflineLargeProductionId = session("MOPLargeProductionId").as[String]; session })
                .exec(_.set("makeOfflineLargeProductionId",makeOfflineLargeProductionId))
                .exec(
                (http("Scn11_MakeOfflineLargeProduction")
                    .post(shObj.conObj.makeOfflineLargeProductionAPI))
                    .check(status.is(shObj.conObj.acceptedStatus))
                    .check(substring("statusQueryGetUri").exists)
                    .check(substring("purgeHistoryDeleteUri").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */   
}
