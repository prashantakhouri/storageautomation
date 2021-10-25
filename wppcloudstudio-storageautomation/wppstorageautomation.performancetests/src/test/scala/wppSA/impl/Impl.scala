package wppSA.impl

import io.gatling.core.Predef._
import io.gatling.http.Predef._
import com.typesafe.config.ConfigFactory
import scala.concurrent.duration._
import scala.util.Random
import scalaj.http.{Http, HttpOptions}
import net.liftweb.json._
import scala.util.Random
import java.io

class Impl
{
    /* <Description>define pacing. Supply conf key. Returns Int pacing</Description> */
    def definePacing(key: String): Int = {
        val isPacingEnabled: Boolean = ConfigFactory.load().getString("app.runSettings.pacingEnabled").toBoolean
        if (!isPacingEnabled)
            return 0
        val Pacing = ConfigFactory.load().getString("app.runSettings.loadTestDuration").toInt / ConfigFactory.load().getString(key).toInt
        return Pacing
    }
}