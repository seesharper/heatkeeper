namespace HeatKeeper.Server.Database
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString();
    }
}