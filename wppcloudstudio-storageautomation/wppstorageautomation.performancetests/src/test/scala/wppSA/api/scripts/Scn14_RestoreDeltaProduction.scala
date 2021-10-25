package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn14_RestoreDeltaProduction(shObj: SuperHelper) {
    /* #region Request Parameters */
        var arcDeltaProductionStoreId = ""
        var arcDeltaProductionId = ""
        val csvFeederProductionDataARC = csv(fileName="productiondataARCDelta.csv").circular
    /* #endregion Request Parameters */

    /* #region HTTP REQUEST */
        def Scn14_RestoreDeltaProduction() = 
        {
            scenario("Scn14_RestoreDeltaProduction").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn14_RestoreDeltaProduction") seconds)
                .feed(csvFeederProductionDataARC)
                .exec(session => { arcDeltaProductionStoreId = session("ARCDeltaProductionStoreId").as[String]; session })
                .exec(_.set("arcDeltaProductionStoreId",arcDeltaProductionStoreId))
                .exec(session => { arcDeltaProductionId = session("ARCDeltaProductionId").as[String]; session })
                .exec(_.set("arcDeltaProductionId",arcDeltaProductionId))
                .exec(
                (http("Scn14_RestoreDeltaProduction")
                    .post(shObj.conObj.restoreDeltaProductionAPI))
                    .header("Content-Type", "application/json")
                    .check(status.is(shObj.conObj.acceptedStatus))
                    .check(substring("statusQueryGetUri").exists)
                    .check(substring("purgeHistoryDeleteUri").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */ 
  
}
