using System.Security.Cryptography.X509Certificates;

namespace TfNet;

public record PluginHostCertificate(X509Certificate2 Certificate);
