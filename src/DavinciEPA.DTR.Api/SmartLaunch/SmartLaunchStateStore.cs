using System.Collections.Concurrent;

namespace DavinciEPA.DTR.Api.SmartLaunch;

/// <summary>Correlates the SMART App Launch <c>state</c> parameter with its PKCE code verifier and launch context between the authorize redirect and the callback.</summary>
public sealed record SmartLaunchState(string CodeVerifier, string Iss, string Launch);

/// <summary>
/// In-memory store for in-flight SMART App Launch attempts. Suitable for a single-instance deployment;
/// a distributed cache should back this in a multi-instance production deployment.
/// </summary>
public sealed class SmartLaunchStateStore
{
    private readonly ConcurrentDictionary<string, SmartLaunchState> _states = new();

    public void Add(string state, SmartLaunchState value) => _states[state] = value;

    public bool TryTake(string state, out SmartLaunchState? value) => _states.TryRemove(state, out value);
}
