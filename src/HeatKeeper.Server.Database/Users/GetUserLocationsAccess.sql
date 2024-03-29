SELECT
    l.id AS LocationId,
    l.Name AS LocationName,
    iif(ul.UserId IS NULL, 0, 1) AS HasAccess
FROM
    Locations l
    LEFT OUTER JOIN UserLocations ul ON l.Id = ul.LocationId
    AND ul.UserId = @UserId