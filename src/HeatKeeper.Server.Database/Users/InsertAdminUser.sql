INSERT INTO Users (Email, FirstName, LastName, IsAdmin, HashedPassword)
SELECT @Email,@Firstname, @Lastname, 1, @HashedPassword
WHERE NOT EXISTS (SELECT 1 FROM Users)