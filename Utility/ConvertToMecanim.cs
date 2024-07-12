using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class ConvertToMecanim : MonoBehaviour
{

    [MenuItem("Tools/Convert Legacy Animation to Mecanim")]
    static void ConvertLegacyAnimationClip()
    {
        foreach (Object obj in Selection.objects)
        {
            if (obj is AnimationClip)
            {
                AnimationClip clip = obj as AnimationClip;
                SerializedObject serializedClip = new SerializedObject(clip);
                SerializedProperty legacyProp = serializedClip.FindProperty("m_Legacy");

                if (legacyProp != null)
                {
                    legacyProp.boolValue = false;
                    serializedClip.ApplyModifiedProperties();
                    Debug.Log("Converted " + clip.name + " to Mecanim.");
                }
                else
                {
                    Debug.LogWarning("Could not find the Legacy property for " + clip.name);
                }
            }
            else
            {
                Debug.LogWarning(obj.name + " is not an AnimationClip");
            }
        }
    }
}
#endif
