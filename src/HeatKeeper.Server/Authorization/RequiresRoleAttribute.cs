using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace HeatKeeper.Server.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class RequireRoleAttribute : Attribute
    {
        private readonly string[] satisfiedByRoles;

        public RequireRoleAttribute(string[] satisfiedByRoles)
        {
            this.satisfiedByRoles = satisfiedByRoles;
        }

        public virtual bool IsSatisfiedBy(string role)
        {
            return satisfiedByRoles.Contains(role);
        }

        public string DisplayName => Regex.Match(this.GetType().Name, "^Require(.*)Attribute$").Groups[1].Value;
    }

    public class RequireAdminRoleAttribute : RequireRoleAttribute
    {
        public RequireAdminRoleAttribute() : base([Roles.AdminRole])
        {
        }
    }

    public class RequireUserRoleAttribute : RequireRoleAttribute
    {
        public RequireUserRoleAttribute() : base([Roles.AdminRole, Roles.UserRole])
        {
        }
    }

    public class RequireReporterRoleAttribute : RequireRoleAttribute
    {
        public RequireReporterRoleAttribute() : base([Roles.AdminRole, Roles.UserRole, Roles.ReporterRole])
        {
        }
    }

    public class RequireBackgroundRole : RequireRoleAttribute
    {
        public RequireBackgroundRole() : base([Roles.AdminRole, Roles.UserRole, Roles.BackgroundUserRole])
        {
        }
    }

    public class RequireAnonymousRoleAttribute : RequireRoleAttribute
    {
        public RequireAnonymousRoleAttribute() : base([])
        {
        }

        public override bool IsSatisfiedBy(string role)
        {
            return true;
        }
    }
}
