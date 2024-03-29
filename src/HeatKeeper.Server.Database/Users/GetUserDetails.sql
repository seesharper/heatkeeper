SELECT
    Id,
    Email,
    FirstName,
    LastName,
    IsAdmin
FROM
    Users
WHERE
    Id = @UserId