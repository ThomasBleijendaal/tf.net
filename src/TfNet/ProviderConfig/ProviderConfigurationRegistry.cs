using TfNet.Schemas;

namespace TfNet.ProviderConfig;

internal record ProviderConfigurationRegistry(
    ISchemaProvider SchemaProvider,
    Type ConfigurationType);
