SELECT
    Id,
    Name,
    Email,
    IsAdmin,
    HashedPassword
FROM
    Users
WHERE
    name = @name