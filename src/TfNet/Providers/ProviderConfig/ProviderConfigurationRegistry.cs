using TfNet.Schemas;

namespace TfNet.Providers.ProviderConfig;

internal record ProviderConfigurationRegistry(
    ISchemaProvider SchemaProvider,
    Type ConfigurationType);
