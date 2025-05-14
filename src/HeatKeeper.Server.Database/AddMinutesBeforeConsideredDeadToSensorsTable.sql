alter table Sensors
    add MinutesBeforeConsideredDead integer default 60 not null;