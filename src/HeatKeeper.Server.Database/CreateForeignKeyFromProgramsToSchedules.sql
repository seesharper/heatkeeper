PRAGMA foreign_keys = 0;

CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                          FROM Programs;

DROP TABLE Programs;

CREATE TABLE Programs (
    Id               INTEGER PRIMARY KEY AUTOINCREMENT
                             NOT NULL,
    LocationId       INTEGER CONSTRAINT FK_PROGRAMS_LOCATIONS REFERENCES Locations (Id) 
                             NOT NULL,
    ActiveScheduleId INTEGER CONSTRAINT FK_PROGRAMS_SCHEDULES REFERENCES Schedules (Id),
    Name             TEXT    NOT NULL
);

INSERT INTO Programs (
                         Id,
                         LocationId,
                         ActiveScheduleId,
                         Name
                     )
                     SELECT Id,
                            LocationId,
                            ActiveScheduleId,
                            Name
                       FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

PRAGMA foreign_keys = 1;
