using Assets.Source.Scripts.Extensions;
using UnityEngine;

namespace Assets.Source.Scripts.Composition_Root
{
    public class CompositionValidator
    {
        public void AllNotNull(params (object Value, string Name)[] items)
        {
            if (items == null)
                return;

            foreach (var (value, name) in items)
            {
                if (IsNullOrDestroyed(value))
                    Guard.NotNull((object)null, name);
            }
        }

        private bool IsNullOrDestroyed(object value)
        {
            if (value == null)
                return true;

            return value is Object unityObject && unityObject == null;
        }
    }
}