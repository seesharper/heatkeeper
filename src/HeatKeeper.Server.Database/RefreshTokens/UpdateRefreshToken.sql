UPDATE RefreshTokens
SET    
    Token = @Token,
    Created = @Created,
    ExpiresInDays = @ExpiresInDays
WHERE
    Id = @Id