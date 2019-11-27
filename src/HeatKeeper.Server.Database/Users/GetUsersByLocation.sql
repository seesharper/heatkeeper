SELECT
    u.Id,
    Name,
    Email,
    IsAdmin
FROM
    Users u
INNER JOIN
    UserLocations ul
ON
    u.Id = ul.UserId AND
    ul.LocationId = @LocationId