namespace TfNet.SampleEfProvider.Extensions;

internal static class CollectionExtensions
{
    extension<T>(ICollection<T> collection)
    {
        public void AddRange(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                collection.Add(value);
            }
        }
    }

}
