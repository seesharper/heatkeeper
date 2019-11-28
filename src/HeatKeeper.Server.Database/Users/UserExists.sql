select exists(
    select 1
    from users
    where email = @email and id <> @id)