package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn07_RestoreLargeProduction(shObj: SuperHelper) {
    /* #region Request Parameters */
        var arcLargeProductionStoreId = ""
        var arcLargeProductionId = ""
        val csvFeederProductionDataARC = csv(fileName="productiondataARC.csv").circular
    /* #endregion Request Parameters */

    /* #region HTTP REQUEST */
        def Scn07_RestoreLargeProduction() = 
        {
            scenario("Scn07_RestoreLargeProduction").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn07_RestoreLargeProduction") seconds)
                .feed(csvFeederProductionDataARC)
                .exec(session => { arcLargeProductionStoreId = session("ARCLargeProductionStoreId").as[String]; session })
                .exec(_.set("arcLargeProductionStoreId",arcLargeProductionStoreId))
                .exec(session => { arcLargeProductionId = session("ARCLargeProductionId").as[String]; session })
                .exec(_.set("arcLargeProductionId",arcLargeProductionId))
                .exec(
                (http("Scn07_RestoreLargeProduction")
                    .post(shObj.conObj.restoreLargeProductionAPI))
                    .header("Content-Type", "application/json") 
                    //.header("Ocp-Apim-Subscription-Key",shObj.conObj.apimSubscriptionKey)
                    //.body(ElFileBody("RestoreLargeProduction.json"))
                    //.asJson
                    .check(status.is(shObj.conObj.acceptedStatus))
                    .check(substring("statusQueryGetUri").exists)
                    .check(substring("purgeHistoryDeleteUri").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */ 
  
}
