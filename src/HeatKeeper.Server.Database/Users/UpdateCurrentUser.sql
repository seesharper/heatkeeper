UPDATE Users
SET
    EMail = @eMail,
    FirstName = @firstName,
    LastName = @lastName
WHERE
    id = @id