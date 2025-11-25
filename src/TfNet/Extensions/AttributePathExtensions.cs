using Google.Protobuf.Collections;
using Tfplugin6;

namespace TfNet.Extensions;

internal static class AttributePathExtensions
{
    extension(IEnumerable<Models.AttributePath> paths)
    {
        public void AddRange(RepeatedField<AttributePath> target)
        {
            var items = paths
                .Where(attribute => attribute.Path.Length > 0)
                .Select(attribute => attribute.Map());

            target.AddRange(items);
        }
    }

    extension(Models.AttributePath attribute)
    {
        public AttributePath Map()
        {
            var path = new AttributePath();
            path.Steps.AddRange(attribute.Path.Select(step => new AttributePath.Types.Step
            {
                AttributeName = step
            }));

            return path;
        }
    }
}
