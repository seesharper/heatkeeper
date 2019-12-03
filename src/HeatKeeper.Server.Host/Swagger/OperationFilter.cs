using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using HeatKeeper.Server.Authorization;
using System.Text.RegularExpressions;

namespace HeatKeeper.Server.Host.Swagger
{
    public class OperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var parameters = context.MethodInfo.GetParameters();
            if (parameters.Length == 0 || parameters.Length > 1)
            {
                throw new ArgumentOutOfRangeException($"Endpoints should have exactly one paramater ({context.MethodInfo.Name})");
            }

            var parameterType = parameters[0].ParameterType;
            var summary = parameterType.GetSummary();
            var roleAttribute = parameters[0].ParameterType.GetRoleAttribute();
            operation.Summary = $"{summary} [AccessLevel: {roleAttribute.DisplayName}]";
        }
    }


    public static class SummaryExtension
    {
        public static string GetSummary(this Type type)
        {
            var assemblyFilename = new Uri(type.Assembly.CodeBase).LocalPath;
            var xmlFile = Path.ChangeExtension(assemblyFilename, ".xml");
            var document = XDocument.Load(xmlFile);
            var memberFullName = $"T:{type.FullName}";
            var memberElement = document.Descendants("member").Where(e => e.Attribute("name").Value == memberFullName).FirstOrDefault();
            return memberElement?.Descendants("summary")?.SingleOrDefault()?.Value?.Trim() ?? string.Empty;
        }
    }
}
