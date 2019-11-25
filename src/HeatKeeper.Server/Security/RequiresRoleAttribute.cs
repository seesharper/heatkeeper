using System;
using System.Linq;

namespace HeatKeeper.Server.Security
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
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
    }

    public class RequireAdminRoleAttribute : RequireRoleAttribute
    {
        public RequireAdminRoleAttribute() : base(new string[] { Roles.AdminRole })
        {
        }
    }

    public class RequireUserRoleAttribute : RequireRoleAttribute
    {
        public RequireUserRoleAttribute() : base(new string[] { Roles.AdminRole, Roles.UserRole })
        {
        }
    }

    public class RequireReporterRoleAttribute : RequireRoleAttribute
    {
        public RequireReporterRoleAttribute() : base(new string[] { Roles.AdminRole, Roles.UserRole, Roles.ReporterRole })
        {
        }
    }

    public class RequireNoRoleAttribute : RequireRoleAttribute
    {
        public RequireNoRoleAttribute() : base(Array.Empty<string>())
        {
        }

        public override bool IsSatisfiedBy(string role)
        {
            return true;
        }
    }
}
