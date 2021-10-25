import Dependencies._

enablePlugins(GatlingPlugin)

scalaVersion := "2.13.6"

scalacOptions := Seq(
  "-encoding", "UTF-8", "-target:jvm-1.8", "-deprecation",
  "-feature", "-unchecked", "-language:implicitConversions", "-language:postfixOps")

libraryDependencies += "io.gatling.highcharts" % "gatling-charts-highcharts" % "3.6.0" % "test,it"
libraryDependencies += "io.gatling"            % "gatling-test-framework"    % "3.6.0" % "test,it"
libraryDependencies +=  "org.scalaj" %% "scalaj-http" % "2.4.2"
libraryDependencies +=  "net.liftweb" %% "lift-json" % "3.4.3"

val akkaStreamVersion = "2.6.13"
val akkaHttpVersion = "10.1.12"
libraryDependencies ++= Seq(
  // akka streams
  "com.typesafe.akka" %% "akka-stream" % akkaStreamVersion,
  "com.typesafe.akka" %% "akka-protobuf-v3" % akkaStreamVersion,
  "com.typesafe.akka" %% "akka-actor" % akkaStreamVersion,
  "com.typesafe.akka" %% "akka-slf4j" % akkaStreamVersion,
  // akka http
  "com.typesafe.akka" %% "akka-http" % akkaHttpVersion,
  "com.typesafe.akka" %% "akka-http-spray-json" % akkaHttpVersion
)

dependencyOverrides += "com.typesafe.akka" %% "akka-actor" % akkaStreamVersion
dependencyOverrides += "com.typesafe.akka" %% "akka-stream" % akkaStreamVersion
dependencyOverrides += "com.typesafe.akka" %% "akka-protobuf-v3" % akkaStreamVersion
dependencyOverrides += "com.typesafe.akka" %% "akka-slf4j" % akkaStreamVersion