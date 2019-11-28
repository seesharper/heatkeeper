SELECT
    Id,
    Email,
    FirstName,
    LastName,
    IsAdmin,
    HashedPassword
FROM
    Users
WHERE
    email = @email