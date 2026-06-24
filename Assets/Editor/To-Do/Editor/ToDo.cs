using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer;
using Handling.Data;
using Handling.UI;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;


public class ToDo : EditorWindow
{
    public static bool editMode = true;

    private static ItemData rootData;
    private static VisualElement TaskList;

    [MenuItem("Window/ToDo")] // Add menu item to open the To-Do window

    // Initialize the To-Do window
    public static void Init()
    {
        ToDo wnd = GetWindow<ToDo>();               // Get an instance of the To-Do window
        wnd.titleContent = new GUIContent("To-Do"); // Set the title of the window
    }

    

    private void PrintTree(ItemData root)
    {
        var sb = new System.Text.StringBuilder();

        BuildTree(root, 0, sb);

        Debug.Log(sb.ToString());
    }

    private void BuildTree(ItemData folder, int depth, System.Text.StringBuilder sb)
    {
        string indent = new string(' ', depth * 2);

        sb.AppendLine($"{indent}[Folder] {folder.name}");

        foreach(var child in folder.children)
        {
            if (child.type == NodeType.Task)
            {
                sb.AppendLine($"{indent}  [Task] {((ItemData)child).name}");
            }
            if (child.type == NodeType.Folder)
            {
                BuildTree((ItemData)child, depth + 1, sb);
            }
        }
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        root.name = "Root";

        rootData = new ItemData();
        rootData.name = "Root";

        VisualElement ControlMenu = new VisualElement();
        ControlMenu.name = "ControlMenu";
        ControlMenu.style.flexDirection = FlexDirection.Column;
        ControlMenu.style.width = Length.Percent(100);

        VisualElement CM_Row1 = new VisualElement();
        CM_Row1.name = "ControlMenu_Row1";
        CM_Row1.style.flexDirection = FlexDirection.Row;
        CM_Row1.style.width = Length.Percent(100);

        VisualElement CM_Row2 = new VisualElement();
        CM_Row2.name = "ControlMenu_Row2";
        CM_Row2.style.flexDirection = FlexDirection.Row;
        CM_Row2.style.width = Length.Percent(100);

        TaskList = new VisualElement();
        TaskList.name = "TaskList";
        TaskList.style.flexDirection = FlexDirection.Column;
        TaskList.style.width = Length.Percent(100);



        Button button_AddTask = new Button(() =>
        {
            ItemData initVal = new ItemData();
            initVal.completed = false;
            initVal.name = $"New Task {rootVisualElement.childCount}";
            initVal.description = $"New Description {rootVisualElement.childCount}";
            initVal.folded = true;
            initVal.parent = rootData;
            initVal.type = NodeType.Task;

            // attach to root data tree
            rootData.children.Add(initVal);

            // reload UI
            UIHandler.ReloadUI(rootData, TaskList);
        });
        button_AddTask.name = "AddTaskButton";
        button_AddTask.text = "Add Task";
        button_AddTask.style.width = Length.Percent(50);


        Button button_AddFolder = new Button(() =>
        {
            ItemData newFolder = new ItemData();
            newFolder.name = $"New Folder {rootData.children.Count}";
            newFolder.parent = rootData;
            newFolder.type = NodeType.Folder;

            // attach to root data tree
            rootData.children.Add(newFolder);


            // reload UI
            UIHandler.ReloadUI(rootData, TaskList);
            //Debug.Log("Add Folder");
        });

        button_AddFolder.name = "AddFolderButton";
        button_AddFolder.text = "Add Folder";
        button_AddFolder.style.width = Length.Percent(50);

        Foldout foldout_Settings = new Foldout();
        foldout_Settings.name = "Settings Foldout";
        foldout_Settings.text = "Settings";
        foldout_Settings.style.width = Length.Percent(100);

        Toggle toggle_EditMode = new Toggle();
        toggle_EditMode.name = "ToggleEditMode";
        toggle_EditMode.text = "Edit Mode";
        toggle_EditMode.value = false;
        toggle_EditMode.style.width = Length.Percent(100);
        editMode = false;
        toggle_EditMode.RegisterValueChangedCallback(evt =>
        {
            editMode = evt.newValue;

            foreach (var editButtons in rootVisualElement.Query<VisualElement>("EditButtons").ToList())
            {
                editButtons.style.display =
                    editMode ? DisplayStyle.Flex : DisplayStyle.None;
            }
            foreach (var deleteButton in rootVisualElement.Query<VisualElement>("deleteButton").ToList())
            {
                deleteButton.style.display =
                    editMode ? DisplayStyle.None : DisplayStyle.Flex;
            }
            foreach (var foldoutButton in rootVisualElement.Query<VisualElement>("foldoutButton").ToList())
            {
                foldoutButton.style.backgroundColor =
                    editMode ? Color.darkGray : StyleKeyword.Null;
            }
        });


        Button debugTree = new Button(() =>
        {
            PrintTree(rootData);
        });
        debugTree.text = "Print Folder Tree";
        debugTree.name = "DebugTreeButton";
        debugTree.style.width = Length.Percent(100);

        Button reloadUI = new Button(() =>
        {
            UIHandler.ReloadUI(rootData, TaskList);
        });
        reloadUI.text = "Reload UI";
        reloadUI.name = "ReloadUIButton";
        reloadUI.style.width = Length.Percent(100);

        VisualElement DataHandling = new VisualElement();
        DataHandling.name = "DataHandling Settings";
        DataHandling.style.width = Length.Percent(100);
        DataHandling.style.flexDirection = FlexDirection.Row;

        Button Save = new Button(() =>
        {
            DataPersistence.SaveData(rootData);
        });
        Save.name = "Saving Button";
        Save.text = "Save";
        Save.style.width = Length.Percent(50);
        Save.style.alignContent = Align.Center;

        Button Load = new Button(() =>
        {
            rootData = DataPersistence.LoadData();
            UIHandler.ReloadUI(rootData, TaskList);
        });
        Load.name = "Loading Button";
        Load.text = "Load";
        Load.style.width = Length.Percent(50);
        Load.style.alignContent = Align.Center;


        //rootData.children.Add(new ToDoItemData { name = "Task 3" });

        //rootData.children.Add(new ToDoItemData { name = "Task 1" });
        //ToDoFolderData folder1 = new ToDoFolderData { name = "Folder 1" };
        //rootData.children.Add(folder1);
        //folder1.children.Add(new ToDoItemData { name = "Task 2" });

        //ToDoFolderData folder2 = new ToDoFolderData { name = "Folder 2" };
        //folder1.children.Add(folder2);
        //folder2.children.Add(new ToDoItemData { name = "Task 4" });

        //rootData.children.Add(new ToDoItemData { name = "Task 5" });

        root.Add(ControlMenu);

        ControlMenu.Add(CM_Row1);
        CM_Row1.Add(button_AddTask);
        CM_Row1.Add(button_AddFolder);

        ControlMenu.Add(CM_Row2);
        CM_Row2.Add(foldout_Settings);
        foldout_Settings.Add(toggle_EditMode);
        foldout_Settings.Add(debugTree);
        foldout_Settings.Add(reloadUI);
        foldout_Settings.Add(DataHandling);
        DataHandling.Add(Save);
        DataHandling.Add(Load);
        DataHandling.Add(Load);


        root.Add(TaskList);
        UIHandler.ReloadUI(rootData, TaskList);
    }

    public static void ToDoReloadUI()
    {
        UIHandler.ReloadUI(rootData, TaskList);
    }
}
