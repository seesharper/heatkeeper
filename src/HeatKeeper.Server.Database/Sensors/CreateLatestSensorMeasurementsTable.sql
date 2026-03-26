CREATE TABLE LatestSensorMeasurements (
    SensorId        INTEGER  NOT NULL REFERENCES Sensors(Id),
    MeasurementType INTEGER  NOT NULL,
    Value           DOUBLE   NOT NULL,
    Updated         DATETIME NOT NULL
);

CREATE UNIQUE INDEX IDX_LatestSensorMeasurements_SensorId_MeasurementType
    ON LatestSensorMeasurements (SensorId, MeasurementType);
