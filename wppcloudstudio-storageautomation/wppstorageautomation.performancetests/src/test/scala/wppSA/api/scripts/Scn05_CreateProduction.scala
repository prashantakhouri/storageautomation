package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn05_CreateProduction(shObj: SuperHelper) {
    /* #region Request Parameters */
        var archivePSLargeProductionStoreId = ""
        val csvFeederProductionDataWIP = csv(fileName="productiondataWIP.csv").random
    /* #endregion Request Parameters */
  
  /* #region HTTP REQUEST */
    def Scn05_CreateProduction() = 
    {
      scenario("Scn05_CreateProduction").during(shObj.conObj.testDuration seconds)
      {
        pace(shObj.implObj.definePacing("app.scenarioIterations.Scn05_CreateProduction") seconds)
        .feed(csvFeederProductionDataWIP)
        .exec(session => { archivePSLargeProductionStoreId = session("ArchivePSLargeProductionStoreId").as[String]; session })
        .exec(_.set("wipLargeProductionStoreId",archivePSLargeProductionStoreId))
        .exec(_.set("ProductionId",DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH-mm-ss-SSS'Z'").format(LocalDateTime.now()) + "_" + System.nanoTime))
        .exec(
          (http("Scn05_CreateProduction")
            .post(shObj.conObj.createProductionAPI))
            .header("Content-Type", "application/json") 
            .body(ElFileBody("CreateProduction.json"))
            .asJson
            .check(status.is(shObj.conObj.defaultStatus))
            .check(substring("status").exists)
            .check(substring("createdDateTime").exists)
          )
      }
    }
  /* #endregion HTTP REQUEST */  

}
