PRAGMA foreign_keys = 0;

CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                          FROM Measurements;

DROP TABLE Measurements;

CREATE TABLE Measurements (
    Id              INTEGER  NOT NULL
                             PRIMARY KEY AUTOINCREMENT,
    SensorId        INTEGER  NOT NULL
                             REFERENCES Sensors (Id),
    MeasurementType INTEGER  NOT NULL,
    Value           DOUBLE   NOT NULL,
    Exported        DATETIME,
    Created         DATETIME NOT NULL
                             DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO Measurements (
                             Id,
                             SensorId,
                             MeasurementType,
                             Value,
                             Exported,
                             Created
                         )
                         SELECT Id,
                                SensorId,
                                MeasurementType,
                                Value,
                                Exported,
                                Created
                           FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

PRAGMA foreign_keys = 1;
