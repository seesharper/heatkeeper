select exists(
    select 1
    from zones
    where name = @name and locationId = @locationId and id <> @id)