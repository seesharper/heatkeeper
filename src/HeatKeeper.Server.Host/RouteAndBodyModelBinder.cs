using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
namespace HeatKeeper.Server.Host
{
    public class RouteAndBodyModelBinder : IModelBinder
    {
        private readonly IModelBinder bodyModelBinder;

        public RouteAndBodyModelBinder(IModelBinder bodyModelBinder)
        {
            this.bodyModelBinder = bodyModelBinder;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {

            await bodyModelBinder.BindModelAsync(bindingContext);

            var routeValues = bindingContext.HttpContext.Request.RouteValues.Select(rv => rv.Key);

            var routeProperties = bindingContext.ModelMetadata.Properties.Where(p => routeValues.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToArray();

            if (bindingContext.Result.Model == null)
            {
                if (IsRecordType(bindingContext.ModelType))
                {
                    bindingContext.Result = ModelBindingResult.Success(RuntimeHelpers.GetUninitializedObject(bindingContext.ModelType));
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Success(Activator.CreateInstance(bindingContext.ModelType));
                }

            }
            foreach (var routeProperty in routeProperties)
            {
                var routeValue = bindingContext.ValueProvider.GetValue(routeProperty.Name).FirstValue;
                var convertedValue = Convert.ChangeType(routeValue, routeProperty.UnderlyingOrModelType);
                routeProperty.PropertySetter(bindingContext.Result.Model, convertedValue);
            }
        }

        private static bool IsRecordType(Type type)
        {
            return type.GetMethods().Any(m => m.Name == "<Clone>$");
        }
    }


    public class QueryAndRouteModelBinder : IModelBinder
    {


        public QueryAndRouteModelBinder()
        {

        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {



            var routeValues = bindingContext.HttpContext.Request.RouteValues.Select(rv => rv.Key);

            var test = bindingContext.HttpContext.Request.Query;

            var routeProperties = bindingContext.ModelMetadata.Properties.Where(p => routeValues.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToArray();
            var queryProperties = bindingContext.ModelMetadata.Properties.Where(p => test.ContainsKey(p.Name)).ToArray();


            if (bindingContext.Result.Model == null)
            {
                if (IsRecordType(bindingContext.ModelType))
                {
                    bindingContext.Result = ModelBindingResult.Success(RuntimeHelpers.GetUninitializedObject(bindingContext.ModelType));
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Success(Activator.CreateInstance(bindingContext.ModelType));
                }

            }
            foreach (var routeProperty in routeProperties)
            {
                var routeValue = bindingContext.ValueProvider.GetValue(routeProperty.Name).FirstValue;
                var convertedValue = Convert.ChangeType(routeValue, routeProperty.UnderlyingOrModelType);
                routeProperty.PropertySetter(bindingContext.Result.Model, convertedValue);
            }

            foreach (var queryProperty in queryProperties)
            {
                var queryValue = bindingContext.ValueProvider.GetValue(queryProperty.Name).FirstValue;
                if (queryProperty.UnderlyingOrModelType.IsEnum)
                {
                    var enumValue = Enum.Parse(queryProperty.UnderlyingOrModelType, queryValue);
                    queryProperty.PropertySetter(bindingContext.Result.Model, enumValue);
                }
                else
                {
                    var convertedValue = Convert.ChangeType(queryValue, queryProperty.UnderlyingOrModelType);
                    queryProperty.PropertySetter(bindingContext.Result.Model, convertedValue);
                }

            }
        }

        private static bool IsRecordType(Type type)
        {
            return type.GetMethods().Any(m => m.Name == "<Clone>$");
        }
    }




    public class RouteAndBodyBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinderProvider bodyModelBinderProvider;

        public RouteAndBodyBinderProvider(IModelBinderProvider bodyModelBinderProvider)
        {
            this.bodyModelBinderProvider = bodyModelBinderProvider;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            var identity = (ModelMetadataIdentity)typeof(ModelMetadata).GetProperty("Identity", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(context.Metadata);
            if (identity.MetadataKind == ModelMetadataKind.Parameter)
            {
                if (identity.ParameterInfo.IsDefined(typeof(FromBodyAndRouteAttribute)))
                {
                    return new RouteAndBodyModelBinder(bodyModelBinderProvider.GetBinder(context));
                }

                if (identity.ParameterInfo.IsDefined(typeof(FromQueryAndRouteAttribute)))
                {
                    return new QueryAndRouteModelBinder();
                }

            }

            return null;
        }
    }

    public class FromBodyAndRouteAttribute : Attribute
    {

    }

    public class FromQueryAndRouteAttribute : Attribute
    {

    }
}
