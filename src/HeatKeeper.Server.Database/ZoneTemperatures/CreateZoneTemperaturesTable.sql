CREATE TABLE ZoneTemperatures (
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    ZoneId INTEGER NOT NULL REFERENCES Zones(Id),
    Temperature REAL NOT NULL,
    Hour DATETIME NOT NULL,
    LastUpdate DATETIME NOT NULL
);

CREATE UNIQUE INDEX idx_zone_temperatures_zone_hour ON ZoneTemperatures (ZoneId, Hour);
