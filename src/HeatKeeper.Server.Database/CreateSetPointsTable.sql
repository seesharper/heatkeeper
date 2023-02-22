CREATE TABLE SetPoints (
    Id         INTEGER PRIMARY KEY AUTOINCREMENT
                       NOT NULL,
    ScheduleId INTEGER CONSTRAINT FK_SETPOINTS_SCHEDULES REFERENCES Schedules (Id) 
                       NOT NULL,
    Value      DOUBLE  NOT NULL,
    Hysteresis DOUBLE  NOT NULL,
    ZoneId     INTEGER CONSTRAINT FK_SETPOINTS_ZONES REFERENCES Zones (Id) 
                       NOT NULL
);

CREATE UNIQUE INDEX IDX_SCHEDULEID_ZONEID ON SetPoints (
    ScheduleId,
    ZoneId
);
