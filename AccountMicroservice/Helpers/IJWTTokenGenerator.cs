using System;

namespace AccountMicroservice.Helpers
{
    public interface IJWTTokenGenerator
    {
        string GenerateJWT(Guid id);
    }
}