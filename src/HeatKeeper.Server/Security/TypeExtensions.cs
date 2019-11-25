using System;
using System.Collections.Generic;
using System.Linq;

namespace HeatKeeper.Server.Security
{
    public static class TypeExtensions
    {
        public static RequireRoleAttribute GetRoleAttribute(this Type type)
        {
            var roleAttribute = (RequireRoleAttribute)type.GetCustomAttributes(true).SingleOrDefault(c => c is RequireRoleAttribute);
            if (roleAttribute == null)
            {
                if (type.IsArray)
                {
                    return GetRoleAttribute(type.GetElementType());
                }

                throw new InvalidOperationException($"Missing role attribute for type {type}");
            }

            return roleAttribute;
        }
    }
}
