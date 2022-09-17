using System.Security.Claims;
using a;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace VecoBackend.Authentication;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(
        new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Name, "Anonymous"),
            new Claim(ClaimTypes.NameIdentifier, ""),
            new Claim(ClaimTypes.Role, "Anonymous")
        }, "CustomAuth"));

    public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
            var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
            if (userSession == null)
                return await Task.FromResult(new AuthenticationState(_anonymous));
            var claimPrincipal =
                new ClaimsPrincipal(
                    new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userSession.UserName),
                        new Claim(ClaimTypes.NameIdentifier, userSession.Token),
                        new Claim(ClaimTypes.Role, userSession.Role)
                    }, "CustomAuth"));
            return await Task.FromResult(new AuthenticationState(claimPrincipal));
        }
        catch
        {
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
    }


    public async Task UpdateAutheticationSession(UserSession userSession)
    {
        ClaimsPrincipal claimPrincipal;
        if (userSession == null)
        {
            await _sessionStorage.DeleteAsync("UserSession");
            claimPrincipal = _anonymous;
        }
        else
        {
            await _sessionStorage.SetAsync("UserSession", userSession);
            claimPrincipal =
                new ClaimsPrincipal(
                    new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userSession.UserName),
                        new Claim(ClaimTypes.NameIdentifier, userSession.Token),
                        new Claim(ClaimTypes.Role, userSession.Role)
                    }));
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimPrincipal)));
    }
}