using System.ComponentModel;
using System.Reflection;

namespace HeatKeeper.Server.Notifications.Api;
public class GetNotificationTypes(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetNotificationTypesQuery, NotificationTypeInfo[]>
{
    public async Task<NotificationTypeInfo[]> HandleAsync(GetNotificationTypesQuery query, CancellationToken cancellationToken = default)
    {
        return Enum.GetValues(typeof(NotificationType))
           .Cast<NotificationType>()
           .Select(nt => new NotificationTypeInfo(
               Id: (int)nt,
               Name: GetEnumDescription(nt)
           ))
           .ToArray();

    }

    private static string GetEnumDescription(Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}

[RequireUserRole]
[Get("/api/notification-types")]
public record GetNotificationTypesQuery : IQuery<NotificationTypeInfo[]>;

public record NotificationTypeInfo(int Id, string Name);

