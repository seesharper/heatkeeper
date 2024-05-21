namespace HeatKeeper.Server;

public static class DbReaderExtensions
{
    public static async Task<ResourceId> ExecuteInsertAsync(this IDbConnection dbConnection, string sql, object args)
    {
        await dbConnection.ExecuteAsync(sql, args);
        return new ResourceId(await dbConnection.ExecuteScalarAsync<long>("SELECT last_insert_rowid();"));
    }

    public static async Task ExecuteInsertAsync(this IDbConnection dbConnection, string sql, CreateCommand args)
    {
        await dbConnection.ExecuteAsync(sql, args);
        args.SetCreatedResult(new ResourceId(await dbConnection.ExecuteScalarAsync<long>("SELECT last_insert_rowid();")));
    }
}