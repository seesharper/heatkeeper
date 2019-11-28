SELECT
    u.Id,
    Email,
    FirstName,
    LastName,
    IsAdmin
FROM
    Users u
INNER JOIN
    UserLocations ul
ON
    u.Id = ul.UserId AND
    ul.LocationId = @LocationId