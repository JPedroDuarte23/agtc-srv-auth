using System.Diagnostics.CodeAnalysis;

namespace AgtcSrvAuth.Application.Exceptions;

[ExcludeFromCodeCoverage]
public class ConflictException : HttpException
{
    public ConflictException(string message)
     : base(409, "Conflict", message) { }
}
