using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.WebApi.Tests;

public class TestUserContext : IUserContext
{
    public long Id => 1;

    public string FirstName => "Bobby";

    public string LastName => "DropTables";

    public string Email => "bobby@droptables.com";
    public bool IsAdmin => Role == "admin";
    public string Role => "admin";
}