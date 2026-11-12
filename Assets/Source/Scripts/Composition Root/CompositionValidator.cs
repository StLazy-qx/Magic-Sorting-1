using Assets.Source.Scripts.Extensions;

namespace Assets.Source.Scripts.Composition_Root
{
    public class CompositionValidator
    {
        public void AllNotNull(params (object Value, string Name)[] items)
        {
            if (items == null)
                return;

            foreach (var (value, name) in items)
                Guard.NotNull(value, name);
        }
    }
}
