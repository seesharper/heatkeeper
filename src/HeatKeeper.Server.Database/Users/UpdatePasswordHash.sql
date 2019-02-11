UPDATE Users
SET HashedPassword = @HashedPassword
WHERE Id = @UserId 