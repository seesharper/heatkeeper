using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Locations
{
    public class AddUserLocationRequest
    {
        public AddUserLocationRequest(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; }
    }

    public class AddUserLocationRequest2
    {

        public long UserId { get; set; }

        //[FromRoute]
        public long LocationId { get; set; }
    }
}
