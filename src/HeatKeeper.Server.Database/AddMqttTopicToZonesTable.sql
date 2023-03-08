PRAGMA foreign_keys = 0;

CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                          FROM Zones;

DROP TABLE Zones;

CREATE TABLE Zones (
    Id          INTEGER NOT NULL
                        PRIMARY KEY AUTOINCREMENT,
    LocationId  INTEGER NOT NULL
                        REFERENCES Locations (Id),
    Name        TEXT    NOT NULL,
    Description TEXT,
    MqttTopic   TEXT
);

INSERT INTO Zones (
                      Id,
                      LocationId,
                      Name,
                      Description
                  )
                  SELECT Id,
                         LocationId,
                         Name,
                         Description
                    FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

CREATE UNIQUE INDEX idx_zones ON Zones (
    "LocationId" ASC,
    "Name" ASC
);

PRAGMA foreign_keys = 1;
