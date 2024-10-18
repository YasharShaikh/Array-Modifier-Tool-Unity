using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArrayModifierType
{
	Prefab, ArrayModifier
}
public enum ParentTo
{
	Self, SelfParent
}

public class ArrayModifier : MonoBehaviour {

	public ArrayModifierType type;

	public GameObject prefab;
	public ArrayModifier arrayModifier;

	public int count;

	public static Vector3 defaultAbsoluteOffset;
	public static Vector3 defaultRelativeOffset = new Vector3(0.0f, 0.0f, 0.0f);
    public static Vector3 defaultRelativeRotationOffset = new Vector3(0.0f, 0.0f, 0.0f);

	public Vector3 absoluteOffset = defaultAbsoluteOffset;
	public Vector3 relativeOffset = defaultRelativeOffset;
	public Vector3 relativeRotationOffset = defaultRelativeRotationOffset;
	
	public bool enableSiblingRotation = true;

	public bool enableRelativeOffset = true;

	public Vector3 scale = Vector3.one;
	public Vector3 rotation = Vector3.zero;

	public ParentTo parentTo;

	public bool auto;

	public GameObject _tempGroup;
	public GameObject[] _temp;
}
