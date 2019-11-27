select exists(
    select 1
    from UserLocations
    where LocationId = @LocationId and UserId = @UserId)