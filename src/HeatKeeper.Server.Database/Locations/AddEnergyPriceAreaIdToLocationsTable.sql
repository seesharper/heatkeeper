ALTER TABLE Locations ADD COLUMN EnergyPriceAreaId INTEGER REFERENCES EnergyPriceAreas(Id);
