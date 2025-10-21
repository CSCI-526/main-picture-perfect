using UnityEngine;
using UnityEditor;

public class AddTriggerArea : MonoBehaviour
{
    [MenuItem("Tools/Add TriggerArea to Selected Platforms")]
    static void AddTriggerToSelected()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            BoxCollider originalCollider = obj.GetComponent<BoxCollider>();
            if (originalCollider == null)
            {
                Debug.LogWarning($"{obj.name} has no BoxCollider.");
                continue;
            }

            // create trigger area
            GameObject triggerArea = new GameObject("TriggerArea");
            triggerArea.transform.parent = obj.transform;
            triggerArea.transform.localPosition = Vector3.zero;
            triggerArea.transform.localRotation = Quaternion.identity;
            triggerArea.transform.localScale = Vector3.one;

            BoxCollider triggerCollider = triggerArea.AddComponent<BoxCollider>();
            triggerCollider.size = originalCollider.size;
            triggerCollider.center = originalCollider.center;
            triggerCollider.isTrigger = true;

            // add SetRespawnPointOnTouch script
            triggerArea.AddComponent<SetRespawnPointOnTouch>();

            Debug.Log($"Added TriggerArea to {obj.name}");
        }
    }
}
