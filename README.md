# Array Modifier for Unity

A scene authoring tool that repeats a prefab with configurable spacing, rotation, scale, and parenting. Use it to lay out repeated props without placing each one by hand.

https://github.com/user-attachments/assets/60c53039-b313-45b1-a6f0-b157d5d6a2fa

## Use the tool

1. Copy [ArrayModifier.cs](ArrayModifier.cs) and the [Editor](Editor) folder under `Assets` in your project. Keep the custom inspector inside an `Editor` folder.
2. Add **Array Modifier** to a GameObject and set **Type** to **Prefab**. Assign a prefab from the Project window.
3. Set the count and choose absolute or relative offsets. Use **Auto** to preview the array and **Clear** to remove the preview.
4. Click **Apply** to keep the generated objects. The inspector reparents them and removes the Array Modifier component from the source object.

The custom inspector uses the legacy `PrefabUtility.GetPrefabParent` API for its **This** shortcut. Check API compatibility before importing the tool into a newer Unity project.
