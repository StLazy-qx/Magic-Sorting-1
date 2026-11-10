using Assets.Source.Scripts.YG;
using System;
using System.Collections.Generic;
using System.Linq;
using YG;

namespace Assets.Source.Scripts.Save_Policies
{
    public sealed class SaveCoordinator : IDisposable
    {
        private readonly List<BaseSection> _saveSections;

        public SaveCoordinator(IEnumerable<BaseSection> saveSections)
        {
            _saveSections = saveSections.ToList();
        }

        public void RebuildAll()
        {
            foreach (var saveSection in _saveSections)
                saveSection.Rebuild();
        }

        public void FlushAll()
        {
            foreach (var saveSection in _saveSections)
                saveSection.Flush();

            YG2.SaveProgress();
        }

        public void Dispose()
        {
            FlushAll();

            foreach (var disposableSection in _saveSections.OfType<IDisposable>())
                disposableSection.Dispose();
        }
    }
}
