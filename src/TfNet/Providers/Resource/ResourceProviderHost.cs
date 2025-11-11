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
        var upgradedSerialized = SerializeDynamicValue(upgraded);

        return new UpgradeResourceState.Types.Response
        {
            UpgradedState = upgradedSerialized,
        };
    }

    public async Task<ReadResource.Types.Response> ReadResourceAsync(ReadResource.Types.Request request)
    {
        var current = DeserializeDynamicValue(request.CurrentState);

        var read = await _resourceProvider.ReadAsync(current);
        var readSerialized = SerializeDynamicValue(read);

        return new ReadResource.Types.Response
        {
            NewState = readSerialized
        };
    }

    public async Task<PlanResourceChange.Types.Response> PlanResourceChangeAsync(PlanResourceChange.Types.Request request)
    {
        var prior = DeserializeDynamicValue(request.PriorState);
        var proposed = DeserializeDynamicValue(request.ProposedNewState);

        var planned = await _resourceProvider.PlanAsync(prior, proposed);
        var plannedSerialized = SerializeDynamicValue(planned.Value);
        var requiresReplace = planned.RequiresReplace
            .Where(attribute => attribute.Path.Length > 0).
            Select(attribute =>
            {
                var path = new Tfplugin6.AttributePath();
                path.Steps.AddRange(attribute.Path.Select(step => new Tfplugin6.AttributePath.Types.Step
                {
                    AttributeName = step
                }));

                return path;
            });

        var res = new PlanResourceChange.Types.Response
        {
            PlannedState = plannedSerialized
        };

        res.RequiresReplace.AddRange(requiresReplace);

        return res;
    }

    public async Task<ApplyResourceChange.Types.Response> ApplyResourceChangeAsync(ApplyResourceChange.Types.Request request)
    {
        var prior = DeserializeDynamicValue(request.PriorState);
        var planned = DeserializeDynamicValue(request.PlannedState);

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
            var createdSerialized = SerializeDynamicValue(created);
            return new ApplyResourceChange.Types.Response
            {
                NewState = createdSerialized,
            };
        }
        else
        {
            // Update
            var updated = await _resourceProvider.UpdateAsync(prior, planned);
            var updatedSerialized = SerializeDynamicValue(updated);
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
                State = SerializeDynamicValue(resource),
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

    private T DeserializeDynamicValue(DynamicValue value)
    {
        if (!value.Msgpack.IsEmpty)
        {
            return _serializer.DeserializeMsgPack<T>(value.Msgpack.Memory);
        }

        if (!value.Json.IsEmpty)
        {
            return _serializer.DeserializeJson<T>(value.Json.Memory);
        }

        throw new ArgumentException("Either MessagePack or Json must be non-empty.", nameof(value));
    }

    private DynamicValue SerializeDynamicValue(T value)
    {
        return new DynamicValue { Msgpack = Google.Protobuf.ByteString.CopyFrom(_serializer.SerializeMsgPack(value)) };
    }
}
