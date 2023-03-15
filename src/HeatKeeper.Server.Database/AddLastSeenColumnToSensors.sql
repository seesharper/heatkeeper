PRAGMA foreign_keys = 0;

CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                          FROM Sensors;

DROP TABLE Sensors;

CREATE TABLE Sensors (
    Id          INTEGER  NOT NULL
                         PRIMARY KEY AUTOINCREMENT,
    ZoneId      INTEGER  REFERENCES Zones (Id),
    ExternalId  TEXT     NOT NULL,
    Name        TEXT     NOT NULL,
    Description TEXT,
    LastSeen    DATETIME NOT NULL
                         DEFAULT (CURRENT_TIMESTAMP) 
);

INSERT INTO Sensors (
                        Id,
                        ZoneId,
                        ExternalId,
                        Name,
                        Description
                    )
                    SELECT Id,
                           ZoneId,
                           ExternalId,
                           Name,
                           Description
                      FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

CREATE UNIQUE INDEX IX_Sensors_ExternalId ON Sensors (
    "ExternalId" ASC
);

CREATE UNIQUE INDEX idx_sensors_name ON Sensors (
    "Name" ASC
);

PRAGMA foreign_keys = 1;
