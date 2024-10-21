#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;


public class EditorTreeLooker : MonoBehaviour
{
    static bool render = true;
    private void OnDrawGizmos()
    {
        if (!render)
            return;
        Camera cam = SceneView.lastActiveSceneView.camera;
        
        transform.LookAt(cam.transform);
        Vector3 rot = transform.eulerAngles;
        rot.x = 0;
        transform.eulerAngles = rot;
    }

}
#endif