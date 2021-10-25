package wppSA.api.scripts

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import scala.concurrent.duration._
import wppSA.entities._


class Scn15_GetStatusQueryAPI(shObj: SuperHelper) {
    /* #region Request Parameters */
        var apiURI = ""
        val csvFeederGetStatusDetails = csv(fileName="GetStatusDetails.csv").queue
    /* #endregion Request Parameters */

    /* #region HTTP REQUEST */
        def Scn15_GetStatusQueryAPI() = 
        {
            scenario("Scn15_GetStatusQueryAPI").during(shObj.conObj.testDuration seconds)
            {
                pace(shObj.implObj.definePacing("app.scenarioIterations.Scn15_GetStatusQueryAPI") seconds)
                .feed(csvFeederGetStatusDetails)
                .exec(session => { apiURI = session("APIURI").as[String]; session })
                .exec(_.set("statusQueryGetUri",apiURI))
                .exec(
                (http("Scn15_GetStatusQueryAPI")
                    .get(shObj.conObj.getStatusQueryAPI))                 
                    .check(status.is(shObj.conObj.defaultStatus))
                    .check(substring("createdTime").exists)
                    .check(substring("lastUpdatedTime").exists)
                )
            }
        }
    /* #endregion HTTP REQUEST */ 
  
}
