namespace TfNet.Api;

public interface IStateStorage
{
    Task<string?> GetStateAsync(string stateId);

    Task UpdateStateAsync(string stateId, string lockId, string state);

    Task DeleteStateAsync(string stateId);

    Task<bool> LockAsync(string stateId, string lockId);

    Task UnlockAsync(string stateId, string lockId);
}
