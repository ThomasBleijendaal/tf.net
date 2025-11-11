using Tfplugin6;

namespace TfNet.Providers.Resource;

internal interface IResourceProviderHost
{
    Task<ApplyResourceChange.Types.Response> ApplyResourceChangeAsync(ApplyResourceChange.Types.Request request);
    Task<ImportResourceState.Types.Response> ImportResourceStateAsync(ImportResourceState.Types.Request request);
    Task<PlanResourceChange.Types.Response> PlanResourceChangeAsync(PlanResourceChange.Types.Request request);
    Task<ReadResource.Types.Response> ReadResourceAsync(ReadResource.Types.Request request);
    Task<UpgradeResourceState.Types.Response> UpgradeResourceStateAsync(UpgradeResourceState.Types.Request request);
}
