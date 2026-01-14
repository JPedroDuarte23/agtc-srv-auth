
using System.Diagnostics.CodeAnalysis;

namespace AgtcSrvAuth.Infrastructure.Configuration;

[ExcludeFromCodeCoverage]
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
