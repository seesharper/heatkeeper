--
-- File generated with SQLiteStudio v3.2.1 on Sun Dec 22 13:27:33 2019
--
-- Text encoding used: UTF-8
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Locations
DROP TABLE IF EXISTS Locations;
CREATE TABLE Locations (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, SensorId INTEGER, DefaultOutsideZoneId INTEGER REFERENCES Zones (Id), DefaultInsideZoneId INTEGER REFERENCES Zones (Id), Name TEXT NOT NULL, Description TEXT);

-- Table: Measurements
DROP TABLE IF EXISTS Measurements;
CREATE TABLE "Measurements" ("Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, "SensorId" INTEGER NOT NULL, "MeasurementType" INTEGER NOT NULL, "Value" NUMERIC NOT NULL, "Exported" DATETIME, "Created" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP);

-- Table: Sensors
DROP TABLE IF EXISTS Sensors;
CREATE TABLE Sensors (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ZoneId INTEGER REFERENCES Zones (Id), ExternalId TEXT NOT NULL, Name TEXT NOT NULL, Description TEXT);

-- Table: UserLocations
DROP TABLE IF EXISTS UserLocations;
CREATE TABLE UserLocations (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, LocationId INTEGER NOT NULL REFERENCES Locations (Id), UserId INTEGER NOT NULL REFERENCES Users (Id));

-- Table: Users
DROP TABLE IF EXISTS Users;
CREATE TABLE "Users" ("Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, "Email" TEXT NOT NULL, "FirstName" TEXT NOT NULL, "LastName" TEXT NOT NULL, "IsAdmin" INTEGER NOT NULL, "HashedPassword" TEXT NOT NULL);

-- Table: VersionInfo
DROP TABLE IF EXISTS VersionInfo;
CREATE TABLE "VersionInfo" ("Version" INTEGER NOT NULL, "AppliedOn" DATETIME, "Description" TEXT);

-- Table: Zones
DROP TABLE IF EXISTS Zones;
CREATE TABLE Zones (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, LocationId INTEGER NOT NULL REFERENCES Locations (Id), Name TEXT NOT NULL, Description TEXT);

-- Index: idx_locations_name
DROP INDEX IF EXISTS idx_locations_name;
CREATE UNIQUE INDEX idx_locations_name ON Locations ("Name" ASC);

-- Index: idx_sensors_name
DROP INDEX IF EXISTS idx_sensors_name;
CREATE UNIQUE INDEX idx_sensors_name ON Sensors ("Name" ASC);

-- Index: idx_user_locations
DROP INDEX IF EXISTS idx_user_locations;
CREATE UNIQUE INDEX idx_user_locations ON UserLocations ("LocationId" ASC, "UserId" ASC);

-- Index: idx_users_email
DROP INDEX IF EXISTS idx_users_email;
CREATE UNIQUE INDEX "idx_users_email" ON "Users" ("Email" ASC);

-- Index: idx_zones
DROP INDEX IF EXISTS idx_zones;
CREATE UNIQUE INDEX idx_zones ON Zones ("LocationId" ASC, "Name" ASC);

-- Index: IX_Sensors_ExternalId
DROP INDEX IF EXISTS IX_Sensors_ExternalId;
CREATE UNIQUE INDEX IX_Sensors_ExternalId ON Sensors ("ExternalId" ASC);

-- Index: UC_Version
DROP INDEX IF EXISTS UC_Version;
CREATE UNIQUE INDEX "UC_Version" ON "VersionInfo" ("Version" ASC);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
