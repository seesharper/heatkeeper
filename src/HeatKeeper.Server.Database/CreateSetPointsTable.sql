CREATE TABLE SetPoints (
    Id         INTEGER PRIMARY KEY AUTOINCREMENT
                       NOT NULL,
    ScheduleId INTEGER CONSTRAINT FK_SETPOINTS_SCHEDULES REFERENCES Schedules (Id) 
                       NOT NULL,
    Value      DOUBLE  NOT NULL,
    Hysteresis DOUBLE  NOT NULL
);
