select exists(
    select 1
    from LatestZoneMeasurements
    where MeasurementType = @MeasurementType and ZoneId=@ZoneId)