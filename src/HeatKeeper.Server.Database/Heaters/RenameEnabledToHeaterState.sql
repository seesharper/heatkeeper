ALTER TABLE Heaters RENAME COLUMN Enabled TO HeaterState;
-- Convert old values: Enabled=1 -> Idle(0), Enabled=0 -> Disabled(3)
UPDATE Heaters SET HeaterState = CASE WHEN HeaterState = 1 THEN 0 ELSE 3 END;
