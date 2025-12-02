* Create latest tag for watchtower
* Enable notification (mail) for watchtower
* Fix basepath for grafana so that we can access it through heatkeeper.no
* Disable exposed http port for Grafana
* Notifications should be replaced by triggers 
* Triggers need to carry the domain event down to the action 
* New concept of jobs. A job could be CheckForDeadSensors. If a dead sensor is detected we should send a DomainEvent 
  DeadSensorDetectedEvent. Action here could be to send a notification or turn the heater off 
  Jobs are scheduled using cron expressions 

  How we avoid that checking for dead sensors every 10 minutes causes a notification to be sent every 10 minutes?
  TriggerDefinitions are store in the EventTriggers table. 
  The EventTriggers table should have lasttriggered column and a HoursToSnoooze 
  Probably a bad idea. Ex we need to ensure that the heater gets disabled, but we don't want to spam the user with notifications. 




  What about user defined jobs. Like running a query in the database. Jobs are probably preconfigured or stored in the database. They are scheduled in Janitor.



  


