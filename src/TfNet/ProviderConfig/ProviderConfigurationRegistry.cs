using Tfplugin6;

namespace TfNet.ProviderConfig;

internal record ProviderConfigurationRegistry(
    Schema ConfigurationSchema,
    Type ConfigurationType);
