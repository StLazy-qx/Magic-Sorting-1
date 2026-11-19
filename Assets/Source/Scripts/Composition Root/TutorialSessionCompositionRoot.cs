using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Player;
using UnityEngine;

namespace Assets.Source.Scripts.Composition_Root
{
    public class TutorialSessionCompositionRoot : SessionCompositionRootBase
    {
        [SerializeField] private PlatformTutorialGameAdapter _platformSetter;

        protected override GameMode ResolveMode()
            => GameMode.Tutorial;

        protected override void ValidateSessionDependencies()
        {
            Validator.AllNotNull((_platformSetter, nameof(_platformSetter)));
        }

        protected override void InitializeSessionSync()
        {
            _platformSetter.Initialize();
        }
    }
}