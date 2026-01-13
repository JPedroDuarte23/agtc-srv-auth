using System.Diagnostics.CodeAnalysis;
using AgtcSrvAuth.Infrastructure.Mappings;

namespace FiapSrvGames.Infrastructure.Mappings;

[ExcludeFromCodeCoverage]
public static class MongoMappings
{
    public static void ConfigureMappings() 
    {
        FarmerMapping.Configure();
        SensorMapping.Configure();
    }
}