DELETE FROM Measurements
WHERE Exported IS NOT NULL AND Created < @RetentionDate
