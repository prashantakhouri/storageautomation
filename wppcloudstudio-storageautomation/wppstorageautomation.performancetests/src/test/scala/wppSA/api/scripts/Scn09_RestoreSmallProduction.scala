package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn09_RestoreSmallProduction(shObj: SuperHelper) {
    /* #region Request Parameters */
        var arcSmallProductionStoreId = ""
        var arcSmallProductionId = ""
        val csvFeederProductionDataARC = csv(fileName="productiondataARC.csv").circular
    /* #endregion Request Parameters */

    /* #region HTTP REQUEST */
        def Scn09_RestoreSmallProduction() = 
        {
            scenario("Scn09_RestoreSmallProduction").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn09_RestoreSmallProduction") seconds)
                .feed(csvFeederProductionDataARC)
                .exec(session => { arcSmallProductionStoreId = session("ARCSmallProductionStoreId").as[String]; session })
                .exec(_.set("arcSmallProductionStoreId",arcSmallProductionStoreId))
                .exec(session => { arcSmallProductionId = session("ARCSmallProductionId").as[String]; session })
                .exec(_.set("arcSmallProductionId",arcSmallProductionId))
                .exec(
                (http("Scn09_RestoreSmallProduction")
                    .post(shObj.conObj.restoreSmallProductionAPI))
                    .header("Content-Type", "application/json") 
                    //.body(ElFileBody("RestoreSmallProduction.json"))
                    //.asJson
                    .check(status.is(shObj.conObj.acceptedStatus))
                    .check(substring("statusQueryGetUri").exists)
                    .check(substring("purgeHistoryDeleteUri").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */ 
  
}
