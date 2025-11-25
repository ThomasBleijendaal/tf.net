using TfNet.Extensions;
using TfNet.Providers.ResourceUpgrade;
using TfNet.Serialization;
using Tfplugin6;

namespace TfNet.Providers.Resource;

internal class ResourceProviderHost<T> : IResourceProviderHost
{
    private readonly IResourceProvider<T> _resourceProvider;
    private readonly IResourceUpgrader<T> _resourceUpgrader;
    private readonly IDynamicValueSerializer _serializer;

    public ResourceProviderHost(
        IResourceProvider<T> resourceProvider,
        IResourceUpgrader<T> resourceUpgrader,
        IDynamicValueSerializer serializer)
    {
        _resourceProvider = resourceProvider;
        _resourceUpgrader = resourceUpgrader;
        _serializer = serializer;
    }

    public async Task<UpgradeResourceState.Types.Response> UpgradeResourceStateAsync(UpgradeResourceState.Types.Request request)
    {
        var upgraded = await _resourceUpgrader.UpgradeResourceStateAsync(request.Version, request.RawState.Json.Memory);
        var upgradedSerialized = _serializer.SerializeDynamicValue(upgraded);

        return new UpgradeResourceState.Types.Response
        {
            UpgradedState = upgradedSerialized,
        };
    }

    public async Task<ReadResource.Types.Response> ReadResourceAsync(ReadResource.Types.Request request)
    {
        var current = _serializer.DeserializeDynamicValue<T>(request.CurrentState);

        var read = await _resourceProvider.ReadAsync(current);
        var readSerialized = _serializer.SerializeDynamicValue(read);

        return new ReadResource.Types.Response
        {
            NewState = readSerialized
        };
    }

    public async Task<PlanResourceChange.Types.Response> PlanResourceChangeAsync(PlanResourceChange.Types.Request request)
    {
        var prior = _serializer.DeserializeDynamicValue<T>(request.PriorState);
        var proposed = _serializer.DeserializeDynamicValue<T>(request.ProposedNewState);

        var planned = await _resourceProvider.PlanAsync(prior, proposed);
        var plannedSerialized = _serializer.SerializeDynamicValue(planned.Value);

        var res = new PlanResourceChange.Types.Response
        {
            PlannedState = plannedSerialized
        };

        planned.RequiresReplace.AddRange(res.RequiresReplace);

        return res;
    }

    public async Task<ApplyResourceChange.Types.Response> ApplyResourceChangeAsync(ApplyResourceChange.Types.Request request)
    {
        var prior = _serializer.DeserializeDynamicValue<T>(request.PriorState);
        var planned = _serializer.DeserializeDynamicValue<T>(request.PlannedState);

        if (planned == null)
        {
            // Delete
            await _resourceProvider.DeleteAsync(prior);
            return new ApplyResourceChange.Types.Response();
        }
        else if (prior == null)
        {
            // Create
            var created = await _resourceProvider.CreateAsync(planned);
            var createdSerialized = _serializer.SerializeDynamicValue(created);

            return new ApplyResourceChange.Types.Response
            {
                NewState = createdSerialized
            };
        }
        else
        {
            // Update
            var updated = await _resourceProvider.UpdateAsync(prior, planned);
            var updatedSerialized = _serializer.SerializeDynamicValue(updated);
            return new ApplyResourceChange.Types.Response
            {
                NewState = updatedSerialized,
            };
        }
    }

    public async Task<ImportResourceState.Types.Response> ImportResourceStateAsync(ImportResourceState.Types.Request request)
    {
        var response = new ImportResourceState.Types.Response();

        try
        {
            var imported = await _resourceProvider.ImportAsync(request.Id);

            response.ImportedResources.AddRange(imported.Select(resource => new ImportResourceState.Types.ImportedResource
            {
                TypeName = request.TypeName,
                State = _serializer.SerializeDynamicValue(resource),
            }));

        }
        catch (Exception ex)
        {
            response.Diagnostics.Add(new Diagnostic
            {
                Summary = ex.Message,
                Severity = Diagnostic.Types.Severity.Error
            });
        }

        return response;
    }
}
