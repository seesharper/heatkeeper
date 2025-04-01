SELECT
    n.Id,
    n.Name,
    n.Enabled
FROM Notifications n WHERE UserId = @UserId;