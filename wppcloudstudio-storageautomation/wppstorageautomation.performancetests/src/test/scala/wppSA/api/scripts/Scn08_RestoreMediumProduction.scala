package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn08_RestoreMediumProduction(shObj: SuperHelper) {
    /* #region Request Parameters */
        var arcMediumProductionStoreId = ""
        var arcMediumProductionId = ""
        val csvFeederProductionDataARC = csv(fileName="productiondataARC.csv").circular
    /* #endregion Request Parameters */

    /* #region HTTP REQUEST */
        def Scn08_RestoreMediumProduction() = 
        {
            scenario("Scn08_RestoreMediumProduction").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn08_RestoreMediumProduction") seconds)
                .feed(csvFeederProductionDataARC)
                .exec(session => { arcMediumProductionStoreId = session("ARCMediumProductionStoreId").as[String]; session })
                .exec(_.set("arcMediumProductionStoreId",arcMediumProductionStoreId))
                .exec(session => { arcMediumProductionId = session("ARCMediumProductionId").as[String]; session })
                .exec(_.set("arcMediumProductionId",arcMediumProductionId))
                .exec(
                (http("Scn08_RestoreMediumProduction")
                    .post(shObj.conObj.restoreMediumProductionAPI))
                    .header("Content-Type", "application/json") 
                    //.header("Ocp-Apim-Subscription-Key",shObj.conObj.apimSubscriptionKey)
                    //.body(ElFileBody("RestoreMediumProduction.json"))
                    //.asJson
                    .check(status.is(shObj.conObj.acceptedStatus))
                    .check(substring("statusQueryGetUri").exists)
                    .check(substring("purgeHistoryDeleteUri").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */ 
  
}
