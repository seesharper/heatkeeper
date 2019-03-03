select exists(
    select 1
    from users
    where name = @name and id <> @id)