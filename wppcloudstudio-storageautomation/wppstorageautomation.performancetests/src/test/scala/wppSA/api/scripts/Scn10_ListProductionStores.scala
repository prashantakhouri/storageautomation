package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn10_ListProductionStores(shObj: SuperHelper) {

    /* #region HTTP REQUEST */
        def Scn10_ListProductionStores() = 
        {
            scenario("Scn10_ListProductionStores").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn10_ListProductionStores") seconds)
                .exec(
                (http("Scn10_ListProductionStores")
                    .get(shObj.conObj.listProductionStoreAPI))
                    .check(status.is(shObj.conObj.defaultStatus))
                    .check(substring("productionStoreList").exists)
                    .check(substring("success").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */ 
  
}

