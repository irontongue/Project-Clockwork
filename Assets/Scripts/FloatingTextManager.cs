using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    [SerializeField]static FloatingText[] floatingTextPool = new FloatingText[100];
    [SerializeField]GameObject floatingTextPrefab;
    public static Transform playerTransform;
    static int iterator;
    public static Color colorCrit;
    public static Color colorBody;
    private void Start()
    {
        playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        AddOneHundredFloatingTextGameObjects();
    }
    /// <summary>
    /// Spawns floating text at position which moves up over time
    /// </summary>
    /// <param name="position">spawn pos</param>
    /// <param name="damageValue">number to display</param>
    /// <param name="offset">true = apply random offset</param>
    /// <param name="offesetAmount">amount to randomly offset the x and y position by on spawn</param>
    /// <param name="color">txt color, def is white</param>
    public static void SpawnFloatingText(Vector3 position, float damageValue, bool offset = true, float offesetAmount = 0.25f, Color color = default)
    {
        SpawnText(position, damageValue, offset, offesetAmount, color);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position">spawn pos</param>
    /// <param name="damageValue">number to display</param>
    /// <param name="color">txt color, def is white</param>
    public static void SpawnFloatingText(Vector3 position, float damageValue, Color color = default)
    {
        SpawnText(position, damageValue, true, 0.25f, color);
    }
    static void SpawnText(Vector3 position, float damageValue, bool offset = true, float offesetAmount = 0.25f, Color color = default)
    {
        if (color == default)
        {
            color = Color.white;
        }
        if (iterator == floatingTextPool.Length)
            iterator = 0;
        if (offset)
        {
            float randFloat = Random.Range(-offesetAmount, offesetAmount);
            position = position + new Vector3(randFloat, randFloat, 0);
        }
        FloatingText txt = floatingTextPool[iterator];
        txt.transform.position = position;
        txt.gameObject.SetActive(true);
        txt.ResetText((damageValue * 10).ToString());
        txt.textMeshPro.color = color;

        iterator++;
    }
    void AddOneHundredFloatingTextGameObjects()
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogWarning("FloatingTextManager Button: Assign floatingTextPrefab");
            return;
        }
        FloatingText ft;
        for (int i = 0; i < floatingTextPool.Length; i++) 
        {
            if (floatingTextPool[i] != null)
            {
                Destroy(floatingTextPool[i].gameObject);
            }
            ft = Instantiate(floatingTextPrefab, transform).GetComponent<FloatingText>();
            ft.gameObject.SetActive(false);
            floatingTextPool[i] = ft;
        }
    }
}
