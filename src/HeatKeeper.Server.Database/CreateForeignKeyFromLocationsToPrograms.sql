PRAGMA foreign_keys = 0;

CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                          FROM Locations;

DROP TABLE Locations;

CREATE TABLE Locations (
    Id                   INTEGER NOT NULL
                                 PRIMARY KEY AUTOINCREMENT,
    SensorId             INTEGER,
    ActiveProgramId      INTEGER CONSTRAINT FK_LOCATIONS_PROGRAMS REFERENCES Programs (Id),
    DefaultOutsideZoneId INTEGER REFERENCES Zones (Id),
    DefaultInsideZoneId  INTEGER REFERENCES Zones (Id),
    Name                 TEXT    NOT NULL,
    Description          TEXT
);

INSERT INTO Locations (
                          Id,
                          SensorId,
                          DefaultOutsideZoneId,
                          DefaultInsideZoneId,
                          Name,
                          Description
                      )
                      SELECT Id,
                             SensorId,
                             DefaultOutsideZoneId,
                             DefaultInsideZoneId,
                             Name,
                             Description
                        FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

CREATE UNIQUE INDEX idx_locations_name ON Locations (
    "Name" ASC
);

PRAGMA foreign_keys = 1;
