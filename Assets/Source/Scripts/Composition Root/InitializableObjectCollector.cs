using Assets.Source.Scripts.EntryPoint;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Composition_Root
{
    public class InitializableObjectCollector
    {
        public void Collect(
            MonoBehaviour[] source,
            List<IObjectInitilizable> target)
        {
            if (source == null || 
                source.Length == 0 || 
                target == null)
                return;

            foreach (var mono in source)
            {
                if (mono == null)
                {
                    throw new ArgumentNullException(
                        nameof(source),
                        "Null entry inside object initialization array.");
                }


                if (mono is IObjectInitilizable init && 
                    target.Contains(init) == false)
                    target.Add(init);
            }
        }
    }
}
