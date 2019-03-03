select exists(
    select 1
    from locations
    where name = @name and id <> @id)