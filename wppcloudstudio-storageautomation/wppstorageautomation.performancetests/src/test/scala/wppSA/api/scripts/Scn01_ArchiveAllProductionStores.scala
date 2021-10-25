package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import java.time.format.DateTimeFormatter
import java.time.LocalDateTime
import wppSA.entities._

class Scn01_ArchiveAllProductionStores(shObj: SuperHelper) {

    /* #region HTTP REQUEST */
        def Scn01_ArchiveAllProductionStores() = 
        {
            scenario("Scn01_ArchiveAllProductionStores").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn01_ArchiveAllProductionStores") seconds)
                .exec(
                (http("Scn01_ArchiveAllProductionStores")
                    .post(shObj.conObj.archiveAllAPI))
                    //.header("Ocp-Apim-Subscription-Key",shObj.conObj.apimSubscriptionKey)
                    .check(status.is(shObj.conObj.acceptedStatus))
                    .check(substring("statusQueryGetUri").exists)
                    .check(substring("purgeHistoryDeleteUri").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */ 
  
}
