using Assets.Source.Scripts.Vessels;
using System;
using System.Collections.Generic;

namespace Assets.Source.Scripts.Pool
{
    class VesselPool : Pool<Vessel>
    {
        //public void DeactivateAll()
        //{
        //    for (int i = 0; i < _objects.Count; i++)
        //    {
        //        if (_objects[i].gameObject.activeSelf)
        //            Deactivate(_objects[i]);
        //    }
        //}

        //public IReadOnlyList<Vessel> GetInactiveObjects()
        //{
        //    List<Vessel> inactive = new List<Vessel>();

        //    for (int i = 0; i < _objects.Count; i++)
        //    {
        //        if (_objects[i].gameObject.activeSelf == false)
        //            inactive.Add(_objects[i]);
        //    }

        //    return inactive;
        //}

        //public void Activate(Vessel vessel)
        //{
        //    if (vessel == null)
        //        throw new ArgumentNullException(nameof(vessel));

        //    if (_objects.Contains(vessel) == false)
        //        throw new InvalidOperationException("Vessel is not in the pool");

        //    if (vessel.gameObject.activeSelf)
        //        return;

        //    vessel.gameObject.SetActive(true);
        //    OnActivated(vessel);
        //}
    }
}
