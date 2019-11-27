UPDATE Users
SET
    Name = @name,
    EMail = @eMail,
    IsAdmin = @isAdmin
WHERE
    id = @id