# UI

* * * * *
In order to manage all UI panels based on Unity uGUI system including open, close, show, hide panels dynamically, UI folder contains GUIManager, UIBase and UIRootHandler script which will work together to complete those tasks.

### Rules
1. All the UIPrefab needs to be placed under UIprefabs folder.
2. The name of UIPrefab needs to be kept the same as its class.
3. All the root node of UIPrefab is full screen.
4. All button click events must be encapsulated.
5. All node finding methods must be encapsulated.

All UIPrefab must have a corresponding scirpt. For example, a UI_Produce prefab must have a script named UI_Produce.cs.
