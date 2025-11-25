namespace TfNet.Registry;

internal record struct Registration<T>(string Key, T Value);
