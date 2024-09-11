using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum WeaponType{Shotgun, Sniper, Smg, TeslaCoil }

public class AllWeaponStats : MonoBehaviour
{
    public WeaponStats[] weaponStats;
    private void Awake()
    {
        InitializeDictionary();
    }
    public WeaponStats GetWeaponStat(WeaponType weaponType)
    {
        return weaponStats[weaponStatsDic[weaponType]];
    }
    private void InitializeDictionary()
    {
        for (int i = 0; i < d_Value.Length; i++)
        {
            weaponStatsDic.Add(d_Key[i], i);
        }
    }
    private void printDic()
    {
        foreach(KeyValuePair<WeaponType, int> kvp in weaponStatsDic)
        {
            print(string.Format("Key = {0}, Value = {1})", kvp.Key, kvp.Value));
        }
    }
    #region Editor Buttons and weapon stats Serialized editing
    [SerializeField] Dictionary<WeaponType, int> weaponStatsDic = new Dictionary<WeaponType, int>();
    [SerializeField] WeaponType weaponTypeToAdd;
    [SerializeField] int[] d_Value = new int[0];
    [SerializeField] WeaponType[] d_Key = new WeaponType[0];
    public void AddWeaponStats()
    {
        if (ContainsWeaponType())
        {
            Debug.LogWarning("Cannot Have multiple of the same weaponStats in this array");
            return;
        }

        AddToFakeDictionary();

        WeaponStats[] x = new WeaponStats[weaponStats.Length + 1];
       
        for(int i = 0; i < weaponStats.Length; i++) 
        {
            x[i] = weaponStats[i];
        }
        x[x.Length - 1] = new WeaponStats();
        x[x.Length - 1].weaponType = weaponTypeToAdd;
        weaponStats = x;
    }
    /// <summary>
    ///  Dictionary arrays seperated to be exposed in the editor
    ///  This Adds to them like a dictionary
    /// </summary>
    void AddToFakeDictionary()
    {
        WeaponType[] x = new WeaponType[d_Key.Length + 1];
        int[] y = new int[d_Value.Length + 1];
        for (int i = 0; i < d_Key.Length; i++)
        {
            x[i] = d_Key[i];
            y[i] = i;
        }
        x[x.Length - 1] = weaponTypeToAdd;
        y[y.Length - 1] = y.Length - 1;
        d_Key = x;
        d_Value = y;
    }
    public void RemoveWeaponStats()
    {
        if (!ContainsWeaponType())
        {
            Debug.LogWarning("Dictionary Does not Contain WeaponType");
            return;
        }

        WeaponStats[] x = new WeaponStats[weaponStats.Length - 1];

        int num = -1;
        for (int i= 0; i < d_Key.Length; i++) // Get The Key of the Value
        {
            if (d_Key[i] == weaponTypeToAdd)
            {
                num = i;
            }
        }
        if(num == -1)
        {
            Debug.LogWarning($"Could not find WeaponType:{weaponTypeToAdd} to remove");
            return;
        }

        int iterator = 0;
        for(int i  = 0; i < weaponStats.Length; i++)
        {
            if (i == num)
                continue;
            x[iterator] = weaponStats[i];
            iterator++;
        }
        weaponStats = x;
        //weaponStatsDic.Remove(weaponTypeToAdd);
        RemoveFromFakeDictionary();
    }
    void RemoveFromFakeDictionary()
    {
        WeaponType[] x = new WeaponType[d_Key.Length - 1];
        int[] y = new int[d_Value.Length - 1];
        int num = -1;
        //Find the Key
        for(int i = 0; i < d_Key.Length; i++)
        {
            if(d_Key[i] == weaponTypeToAdd) 
            {
                num = i;
                break;
            }
        }
        if(num == -1)
        {
            Debug.LogWarning($"Could not find WeaponType:{weaponTypeToAdd} to remove");
            return;
        }

        for (int i = 0, j = 0; i < d_Key.Length; i++)
        {
            if (i == num) continue;
            x[j] = d_Key[i];
            y[j] = j;
            j++;
        }
        d_Key = x;
        d_Value = y;
        

    }
    bool ContainsWeaponType()
    {
        foreach(WeaponType t in d_Key)
        {
            if(t == weaponTypeToAdd)
                return true;
        }
        return false;
    }
    [ContextMenu("ResetWeaponStatsDic")]
    /// <summary>
    /// Just incase the Dictionary and Array get out of sync, Empties the weaponStatsDic.
    /// </summary>
    private void ResetWeaponStatsDic()
    {
        weaponStatsDic = new Dictionary<WeaponType, int>();
        d_Key = new WeaponType[0];
        d_Value = new int[0];

    }
    #endregion
}

#region WeaponStats Class
[System.Serializable]
public class WeaponStats
{
    //[HideInInspector]
    public WeaponType weaponType;
    //[HideInInspector]
    public float damage;
    //[HideInInspector]
    public float attackSpeed;
    //[HideInInspector]
    public float coolDown;
    //[HideInInspector]
    public float range;
    //[HideInInspector]
    public float chargeUpTime;
    //[HideInInspector]
    public float bulletSpread;
    //[HideInInspector]
    public int numberOfBullets = 1;
    //
    public float lockOnDistance;
    //
    public int bounces;
    public float bounceRange;
}
#endregion

#region EditorScript

#if UNITY_EDITOR
[CustomEditor(typeof(AllWeaponStats))]
public class WeapopnStatsEditor : Editor
{
    private SerializedProperty scriptProperty;
    private SerializedProperty WeaponStatsArray;
    private SerializedProperty weaponTypeToAdd;
    private SerializedProperty dictionaryKey;
    private SerializedProperty dictionaryValue;

    bool removeWeaponToggle;
    WeaponType _weaponType = WeaponType.Sniper;
    bool showAllWeapons = false;

    private void OnEnable()
    {
        WeaponStatsArray = serializedObject.FindProperty("weaponStats");
        
        scriptProperty = serializedObject.FindProperty("m_Script"); //Script reference
        weaponTypeToAdd = serializedObject.FindProperty("weaponTypeToAdd");

        dictionaryValue = serializedObject.FindProperty("d_Value");
        dictionaryKey = serializedObject.FindProperty("d_Key");
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true); //Makes the following Read only
        EditorGUILayout.PropertyField(scriptProperty, new GUIContent("Script")); //Expose the script var like how it is in all scripts
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(dictionaryKey, new GUIContent("dictionaryKey")); //Using this to save the values VVV
        EditorGUILayout.PropertyField(dictionaryValue, new GUIContent("dictionaryValue"));
        EditorGUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup(); //stops the read only
        serializedObject.Update();
        
        AllWeaponStats myTarget = (AllWeaponStats)target;
        // ADD WEAPON BUTTON
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 90;

        EditorGUILayout.PropertyField(weaponTypeToAdd, new GUIContent("Weapon To Add To Stats"), true, GUILayout.Width(300));
        if (GUILayout.Button("Add Weapon", GUILayout.Width(125))) //BUTTON FOR ADDING WEAPON_STATS
            myTarget.AddWeaponStats();
        
        EditorGUILayout.EndHorizontal();
        //END
        //REMOVE WEAPON BUTTON
        EditorGUILayout.BeginHorizontal();

        removeWeaponToggle = EditorGUILayout.Toggle("RemoveWeaponButton",removeWeaponToggle); //Guard toggle for accidental removal
        if(removeWeaponToggle)
            if (GUILayout.Button("Remove Weapon", GUILayout.Width(125))) //BUTTON FOR ADDING WEAPON_STATS
                myTarget.RemoveWeaponStats();

        EditorGUILayout.EndHorizontal();
        //END
        EditorGUIUtility.labelWidth = 150;

        //EditorGUILayout.PropertyField(WeaponStatsArray, new GUIContent("WeaponStats"), true); //SERIALIZED ARRAY EXPOSE
        EditorGUILayout.Space(); //ADD SPACE

        EditorGUILayout.BeginHorizontal();

        EditorGUIUtility.labelWidth = 90;

        _weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon To Edit", _weaponType);//Weapon TYPE ENUM POPUP
        showAllWeapons = EditorGUILayout.Toggle("Show All Weapon", showAllWeapons); //BOOL EXPOSE

        EditorGUILayout.Space(); //ADD SPACE
        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = 150;

        for (int i = 0; i < WeaponStatsArray.arraySize; i++)
        {
            SerializedProperty weapon = WeaponStatsArray.GetArrayElementAtIndex(i);
            SerializedProperty weaponType = weapon.FindPropertyRelative("weaponType");
            SerializedProperty damage = weapon.FindPropertyRelative("damage");
            SerializedProperty attackSpeed = weapon.FindPropertyRelative("attackSpeed");
            SerializedProperty coolDown = weapon.FindPropertyRelative("coolDown");
            SerializedProperty range = weapon.FindPropertyRelative("range");
            SerializedProperty chargeUpTime = weapon.FindPropertyRelative("chargeUpTime");
            SerializedProperty bulletSpread = weapon.FindPropertyRelative("bulletSpread");
            SerializedProperty numberOfBullets = weapon.FindPropertyRelative("numberOfBullets");
            SerializedProperty lockOnDistance = weapon.FindPropertyRelative("lockOnDistance");
            SerializedProperty bounces = weapon.FindPropertyRelative("bounces");
            SerializedProperty bounceRange = weapon.FindPropertyRelative("bounceRange");


            if (!showAllWeapons)
            {
                if (weaponType.intValue != (int)_weaponType)
                    continue;
            }
            // Draw the weapon type dropdown
            if (i < myTarget.weaponStats.Length)
                EditorGUILayout.LabelField(myTarget.weaponStats[i].weaponType.ToString(), EditorStyles.boldLabel);

            //EditorGUILayout.PropertyField(weaponType);

            EditorGUILayout.PropertyField(damage);
            EditorGUILayout.PropertyField(attackSpeed);
            EditorGUILayout.PropertyField(coolDown);
            // Draw properties based on the selected weapon type
            switch ((WeaponType)weaponType.enumValueIndex)
            {
                case WeaponType.Shotgun:
                    EditorGUILayout.PropertyField(bulletSpread);
                    EditorGUILayout.PropertyField(range);
                    EditorGUILayout.IntSlider(numberOfBullets, 1, 1000);
                    // Add other properties specific to Shotgun
                    break;

                case WeaponType.Sniper:
                    EditorGUILayout.PropertyField(chargeUpTime);
                                       
                    // Add other properties specific to Sniper
                    break;

                case WeaponType.Smg:
                    EditorGUILayout.PropertyField(lockOnDistance);
                    // Add other properties specific to SMG
                    break;
                case WeaponType.TeslaCoil:

                    EditorGUILayout.PropertyField(range);
                    EditorGUILayout.PropertyField(bounces);
                    EditorGUILayout.PropertyField(bounceRange);

                    break;
            }

            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
// --- LOOP OVER GUNSTATS ARRAY AND EXPOSE VARIABLES ---
//for (int i = 0; i < WeaponStatsArray.arraySize; i++)
//{
//    SerializedProperty weapon = WeaponStatsArray.GetArrayElementAtIndex(i);
//    SerializedProperty weaponType = weapon.FindPropertyRelative("weaponType");
//    SerializedProperty damage = weapon.FindPropertyRelative("damage");
//    SerializedProperty attackSpeed = weapon.FindPropertyRelative("attackSpeed");
//    SerializedProperty chargeUpTime = weapon.FindPropertyRelative("chargeUpTime");
//    SerializedProperty bulletSpread = weapon.FindPropertyRelative("bulletSpread");
//    SerializedProperty lockOnDistance = weapon.FindPropertyRelative("lockOnDistance");

//    // Draw the weapon type dropdown
//    if(i < myTarget.weaponStats.Length)
//    EditorGUILayout.LabelField(myTarget.weaponStats[i].weaponType.ToString(), EditorStyles.boldLabel);

//    EditorGUILayout.PropertyField(weaponType);

//    EditorGUILayout.PropertyField(damage);
//    EditorGUILayout.PropertyField(attackSpeed);
//    // Draw properties based on the selected weapon type
//    switch ((WeaponType)weaponType.enumValueIndex)
//    {
//        case WeaponType.Shotgun:
//            EditorGUILayout.PropertyField(bulletSpread);
//            // Add other properties specific to Shotgun
//            break;

//        case WeaponType.Sniper:
//            EditorGUILayout.PropertyField(chargeUpTime);
//            // Add other properties specific to Sniper
//            break;

//        case WeaponType.Smg:
//            EditorGUILayout.PropertyField(lockOnDistance);
//            // Add other properties specific to SMG
//            break;
//    }

//    EditorGUILayout.Space();
//}
// public class GunBaseCustomEditor : Editor
// {

//     public override void OnInspectorGUI()
//     {
//         // base.OnInspectorGUI();
//EditorGUILayout.PropertyField(serializedObject.FindProperty("vectorTest"));

//         // WeaponStats myTarget = (WeaponStats)target;

//         // myTarget.weaponType = (WeaponType)EditorGUILayout.EnumPopup("Gun Type", myTarget.weaponType); //Expose Enum
//         // myTarget.damage = EditorGUILayout.FloatField("Damage", myTarget.damage);
//         // myTarget.attackSpeed = EditorGUILayout.FloatField("Attack Speed", myTarget.damage);


//         // switch (myTarget.weaponType)
//         // {
//         //     case WeaponType.Shotgun:
//         //         myTarget.bulletSpread = EditorGUILayout.FloatField("Bullet Spread", myTarget.bulletSpread);
//         //     break;

//         //     case WeaponType.Sniper:
//         //         myTarget.chargeUpTime = EditorGUILayout.FloatField("Charge Up Time", myTarget.chargeUpTime);
//         //     break;

//         //     case WeaponType.Smg:
//         //         myTarget.lockOnDistance = EditorGUILayout.FloatField("LockOnDistance", myTarget.lockOnDistance);

//         //     break;

//         //     default:
//         //     break;
//         // }
//     }
// }
#endif
//BUTTON:
// if(GUILayout.Button("Print", GUILayout.Width(900f)))
// {
//     myTarget.PrintHello();
// }
#endregion