using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Assets.Source.Scripts.Items;
using Assets.Source.Scripts.Storage;
using System;

    namespace Assets.Source.Scripts.UI.StoreView
    {
        public class StoreScrollViewFiller : MonoBehaviour
        {
            [SerializeField] private Store _store;
            [SerializeField] private ScrollRect _scrollRect;

            private Coroutine _startRoutine;
            private WaitForEndOfFrame _waitForEndOfFrame;
            private WaitUntil _waitStoreInitialized;

            private void Awake()
            {
                ValidateInitializeArguments();

                _waitForEndOfFrame = new WaitForEndOfFrame();
                _waitStoreInitialized = new WaitUntil(() => _store.IsInitialized);
            }

            private void OnEnable()
            {
                _startRoutine = StartCoroutine(StartRoutine());
            }

            private void OnDisable()
            {
                if (_startRoutine != null)
                    StopCoroutine(_startRoutine);
            }

            private IEnumerator StartRoutine()
            {
                yield return _waitStoreInitialized;
                yield return _waitForEndOfFrame;

                IReadOnlyList<ItemSO> items = _store.GetItemsSO();

                foreach (ItemSO storeItem in items)
                {
                    if (storeItem == null)
                        continue;

                    ItemView itemView = Instantiate(
                        storeItem.ItemView, _scrollRect.content);

                    if (itemView == null)
                    {
                        throw new NullReferenceException(
                            "Instantiated ItemView is null in StoreScrollViewFiller.");
                    }

                    //itemView.Initialize(storeItem);
                }

                Canvas.ForceUpdateCanvases();

                _scrollRect.verticalNormalizedPosition = 1;
            }

        private void ValidateInitializeArguments()
        {
            if (_store == null)
            {
                throw new NullReferenceException(
                    "Store reference missing in StoreScrollViewFiller.");
            }

            if (_scrollRect == null)
            {
                throw new NullReferenceException(
                    "ScrollRect reference missing in StoreScrollViewFiller.");
            }
        }
    }
}