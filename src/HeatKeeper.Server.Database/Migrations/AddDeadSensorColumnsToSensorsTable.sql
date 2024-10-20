alter table Sensors
    add EnableDeadSensorNotification integer default 1 not null;

alter table Sensors
    add MinutesBeforeSensorIsConsideredDead integer default 60;