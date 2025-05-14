--
-- File generated with SQLiteStudio v3.2.1 on Fri Apr 24 13:20:01 2020
--
-- Text encoding used: UTF-8
--
PRAGMA foreign_keys = off;
--BEGIN TRANSACTION;

-- Table: LatestZoneMeasurements
DROP TABLE IF EXISTS LatestZoneMeasurements;
CREATE TABLE LatestZoneMeasurements (ZoneId INTEGER REFERENCES Zones (Id), MeasurementType INTEGER, Value DOUBLE, Updated DATETIME);

-- Index: IDX_ZoneId_MeasurementType
DROP INDEX IF EXISTS IDX_ZoneId_MeasurementType;
CREATE UNIQUE INDEX IDX_ZoneId_MeasurementType ON LatestZoneMeasurements (ZoneId, MeasurementType);

--COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
