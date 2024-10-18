using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ArrayModifier))]
public class ArrayModifierEditor : Editor
{
    ArrayModifier main;

    GameObject _tempGroup;
    GameObject[] _temp;

    void OnEnable()
    {
        main = (ArrayModifier)target;

        _temp = main._temp;
        _tempGroup = main._tempGroup;
    }

    void OnDisable()
    {
        main._temp = _temp;
        main._tempGroup = _tempGroup;
    }

    public override void OnInspectorGUI()
    {
        /// base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));

        if (main.type == ArrayModifierType.Prefab)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("prefab"));

            if (GUILayout.Button("This", GUILayout.Width(60)))
            {
                UnityEngine.Object prefabObject = PrefabUtility.GetPrefabParent(main.gameObject);

                string prefabPath = AssetDatabase.GetAssetPath(prefabObject);

                if (!string.IsNullOrEmpty(prefabPath))
                {
                    main.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                }
                else
                {
                    Debug.LogError("The selected object is not a prefab!!");
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("arrayModifier"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("count"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("parentTo"));

        if (!serializedObject.FindProperty("enableRelativeOffset").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("absoluteOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotation"));
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("relativeOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("relativeRotationOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableSiblingRotation"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("enableRelativeOffset"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("scale"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("auto"));

        EditorGUILayout.BeginHorizontal();

        PreventNegative();

        if (main.auto)
        {
            if (main.prefab != null && main.type == ArrayModifierType.Prefab)
            {
                CreateTempGroup();

                SpawnPrefabs();
            }
        }

        if (GUILayout.Button("Apply"))
        {
            if (main.prefab != null && main.type == ArrayModifierType.Prefab)
            {
                if (main.auto == true)
                {
                    main.auto = false;
                }
                else
                {
                    CreateTempGroup();
                    SpawnPrefabs();
                }

                ReplaceParent(main.gameObject.transform);
                DestroyImmediate(_tempGroup);
                _temp = null;

                DestroyImmediate(main);
            }
        }
        if (GUILayout.Button("Clear"))
        {
            main.auto = false;
            DestroyImmediate(_tempGroup);
            _tempGroup = null;
            _temp = null;
        }

        EditorGUILayout.EndHorizontal();

        if(serializedObject != null)
            serializedObject.ApplyModifiedProperties();
    }

    private void PreventNegative()
    {
        if (main.count < 0)
            main.count = 0;
    }

    private void ReplaceParent(Transform transform)
    {
        for (int i = 0; i < _temp.Length; i++)
        {
            if (main.parentTo == ParentTo.SelfParent && transform.parent != null)
                _temp[i].transform.SetParent(transform.parent);

            else 
                _temp[i].transform.SetParent(transform);

        }
    }

    private void CreateTempGroup()
    {
        _tempGroup = GameObject.Find("_arrayModifierTemp_");

        if (_tempGroup == null)
            _tempGroup = new GameObject("_arrayModifierTemp_");
    }

    void SpawnPrefabs()
    {
        ClearTemp();

        _temp = new GameObject[main.count];

        for (int i = 0; i < main.count; i++)
        {
            GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(main.prefab);
            _temp[i] = prefab;

            _temp[i].transform.SetParent(_tempGroup.transform);

            if (!main.enableRelativeOffset)
            {
                if (i == 0)
                {
                    _temp[i].transform.position = main.transform.position + main.absoluteOffset;
                    _temp[i].transform.localScale = main.scale;
                    _temp[i].transform.localRotation = Quaternion.Euler(main.rotation);
                }
                else
                {
                    _temp[i].transform.position = _temp[i - 1].transform.position + main.absoluteOffset;
                    _temp[i].transform.localScale = main.scale;
                    _temp[i].transform.localRotation = Quaternion.Euler(main.rotation);
                }
            }
            else
            {
                if (i == 0)
                {
                    _temp[i].transform.position = main.transform.position + main.transform.TransformDirection(main.relativeOffset);
                    _temp[i].transform.localScale = main.scale;

                    Vector3 rot = main.transform.eulerAngles;

                    _temp[i].transform.rotation = Quaternion.Euler(new Vector3(
                        rot.x + main.relativeRotationOffset.x,
                        rot.y + main.relativeRotationOffset.y,
                        rot.z + main.relativeRotationOffset.z));
                }
                else
                {
                    if (main.enableSiblingRotation)
                    {
                        _temp[i].transform.position = _temp[i - 1].transform.position + _temp[i - 1].transform.TransformDirection(main.relativeOffset);

                        Vector3 rot = Vector3.zero;

                        _temp[i].transform.rotation = Quaternion.Euler(
                                new Vector3(
                                    _temp[i - 1].transform.eulerAngles.x + rot.x + main.relativeRotationOffset.x,
                                    _temp[i - 1].transform.eulerAngles.y + rot.y + main.relativeRotationOffset.y,
                                    _temp[i - 1].transform.eulerAngles.z + rot.z + main.relativeRotationOffset.z
                                )
                            );
                    }
                    else
                    {
                        _temp[i].transform.position = _temp[i - 1].transform.position + main.transform.TransformDirection(main.relativeOffset);

                        Vector3 rot = main.transform.eulerAngles;

                        _temp[i].transform.rotation = Quaternion.Euler(new Vector3(
                            rot.x + main.relativeRotationOffset.x,
                            rot.y + main.relativeRotationOffset.y,
                            rot.z + main.relativeRotationOffset.z));

                    }

                    _temp[i].transform.localScale = main.scale;
                }
            }
        }
    }

    void ClearTemp()
    {
        if (_temp != null)
        {
            for (int i = 0; i < _temp.Length; i++)
            {
                DestroyImmediate(_temp[i]);
            }
        }
    }
}
