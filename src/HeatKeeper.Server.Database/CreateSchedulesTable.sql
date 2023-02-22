CREATE TABLE Schedules (
    Id             INTEGER PRIMARY KEY AUTOINCREMENT
                           NOT NULL,
    ProgramId      INTEGER CONSTRAINT FK_SCHEDULES_PROGRAMS REFERENCES Programs (Id) 
                           NOT NULL,
    Name           TEXT    NOT NULL,
    CronExpression TEXT    NOT NULL
);
