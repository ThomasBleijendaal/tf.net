using TfNet.Extensions;

namespace TfNet.Api;

internal class MemoryStateStorage : IStateStorage
{
    private readonly Dictionary<string, string> _state = new();

    public async Task DeleteStateAsync(string stateId)
    {
        _state.Remove(stateId);
    }

    public async Task<string?> GetStateAsync(string stateId)
    {
        return _state.TryGetValue(stateId, out var value)
            ? value
            : null;
    }

    public Task<bool> LockAsync(string stateId, string lockId)
    {
        return Task.FromResult(true);
    }

    public Task UnlockAsync(string stateId, string lockId)
    {
        return Task.CompletedTask;
    }

    public Task UpdateStateAsync(string stateId, string lockId, string state)
    {
        _state[stateId] = state;
        return Task.CompletedTask;
    }
}
