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
    private void Start()
    {
        playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        AddOneHundredFloatingTextGameObjects();
    }
    public static void SpawnFloatingText(Vector3 position, float damageValue, bool offset = true, float offesetAmount = 0.25f)
    {
        if(iterator == floatingTextPool.Length)
            iterator = 0;
        if (offset)
        {
            float randFloat = Random.Range(-offesetAmount, offesetAmount);
            position = position + new Vector3(randFloat, randFloat, 0);
        }
        floatingTextPool[iterator].transform.position = position;
        floatingTextPool[iterator].gameObject.SetActive(true);
        floatingTextPool[iterator].ResetText((damageValue * 10).ToString());
        
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
