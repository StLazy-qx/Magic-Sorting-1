using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastDebugger : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) == false)
            return;

        var data = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        var results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(data, results);
        Debug.Log($"--- Raycast hits: {results.Count}");

        foreach (var objects in results)
        {
            Debug.Log($"{objects.gameObject.name} " +
                $"| {objects.module.GetType().Name} " +
                $"| dist {objects.distance}");
        }
    }
}