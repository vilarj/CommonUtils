using System.Security.Claims;

namespace CommonUtils.Auth;

/// <summary>
/// Provides access to the identity of the currently authenticated user.
/// </summary>
public interface ICurrentUserContext
{
    /// <summary>The authenticated user's ID, or <c>null</c> for anonymous requests.</summary>
    string? UserId { get; }

    /// <summary>The authenticated user's email, or <c>null</c> for anonymous requests.</summary>
    string? Email { get; }

    /// <summary>The authenticated user's primary role, or <c>null</c> if no role claim is present.</summary>
    string? Role { get; }

    /// <summary><c>true</c> when the principal is authenticated.</summary>
    bool IsAuthenticated { get; }
}
