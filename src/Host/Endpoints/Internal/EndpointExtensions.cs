using System.Reflection;

namespace Host.Endpoints.Internal;

public static class EndpointExtensions
{
    public static void UseEndpoints<TMarker>(this IApplicationBuilder app)
        => UseEndpoints(app, typeof(TMarker));
    public static void UseEndpoints(this IApplicationBuilder app, Type typeMarker)
    {
        IEnumerable<TypeInfo> endpointsTypes = GetEndpointTypesFromAssemblyContaining(typeMarker);

        foreach (var endpointsType in endpointsTypes)
        {
            endpointsType.GetMethod(nameof(IEndpoints.DefineEndpoints))!
                .Invoke(null, new object[] { app });
        }
    }
    
    private static IEnumerable<TypeInfo> GetEndpointTypesFromAssemblyContaining(Type typeMarker)
    {
        var endpointsTypes = typeMarker.Assembly.DefinedTypes
            .Where(x => !x.IsAbstract)
            .Where(x => !x.IsInterface)
            .Where(x => typeof(IEndpoints).IsAssignableFrom(x));
        return endpointsTypes;
    }
}