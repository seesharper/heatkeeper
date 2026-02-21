CREATE TABLE EnergyCosts (
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    SensorId INTEGER NOT NULL REFERENCES Sensors(Id),
    PowerImport REAL NOT NULL,
    CostInLocalCurrency REAL NOT NULL,
    CostInLocalCurrencyAfterSubsidy REAL NOT NULL,
    CostInLocalCurrencyWithFixedPrice REAL NOT NULL,
    Hour DATETIME NOT NULL
);

CREATE UNIQUE INDEX idx_energy_costs_sensor_hour ON EnergyCosts (SensorId, Hour);
