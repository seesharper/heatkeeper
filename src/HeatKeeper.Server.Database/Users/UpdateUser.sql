UPDATE Users
SET
    EMail = @eMail,
    FirstName = @firstName,
    LastName = @lastName
    IsAdmin = @isAdmin
WHERE
    id = @id