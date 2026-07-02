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

    private static ItemData tasklistData;
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

        tasklistData = new ItemData();
        tasklistData.name = "Root";

        #region Navigation UI

        // -------------------------------------------------- Control menu
        VisualElement ControlMenu = new VisualElement();
        ControlMenu.name = "ControlMenu";
        ControlMenu.style.flexDirection = FlexDirection.Column;
        ControlMenu.style.width = Length.Percent(100);


        // -------------------------------------------------- Row 1
        VisualElement CM_Row1 = new VisualElement();
        CM_Row1.name = "ControlMenu_Row1";
        CM_Row1.style.flexDirection = FlexDirection.Row;
        CM_Row1.style.width = Length.Percent(100);

        Button button_AddTask = new Button(() =>
        {
            ItemData initVal = new ItemData();
            initVal.completed = false;
            initVal.name = $"New Task {rootVisualElement.childCount}";
            initVal.description = $"New Description {rootVisualElement.childCount}";
            initVal.unfolded = false;
            initVal.parent = tasklistData;
            initVal.type = NodeType.Task;

            // attach to root data tree
            tasklistData.children.Add(initVal);

            // reload UI
            UIHandler.ReloadUI(tasklistData, TaskList);
        });
        button_AddTask.name = "AddTaskButton";
        button_AddTask.text = "Add Task";
        button_AddTask.style.flexGrow = 1;

        Button button_AddFolder = new Button(() =>
        {
            ItemData newFolder = new ItemData();
            newFolder.name = $"New Folder {tasklistData.children.Count}";
            newFolder.parent = tasklistData;
            newFolder.unfolded = false;
            newFolder.type = NodeType.Folder;

            // attach to root data tree
            tasklistData.children.Add(newFolder);


            // reload UI
            UIHandler.ReloadUI(tasklistData, TaskList);
            //Debug.Log("Add Folder");
        });
        button_AddFolder.name = "AddFolderButton";
        button_AddFolder.text = "Add Folder";
        button_AddFolder.style.flexGrow = 1;

        // -------------------------------------------------- Row 2
        VisualElement CM_Row2 = new VisualElement();
        CM_Row2.name = "ControlMenu_Row2";
        CM_Row2.style.flexDirection = FlexDirection.Row;
        CM_Row2.style.width = Length.Percent(100);



        Foldout foldout_Settings = new Foldout();
        foldout_Settings.name = "Settings Foldout";
        foldout_Settings.text = "Settings";
        foldout_Settings.style.width = Length.Percent(100);


        Toggle toggle_EditMode = new Toggle();
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
        toggle_EditMode.name = "ToggleEditMode";
        toggle_EditMode.text = "Edit Mode";
        toggle_EditMode.value = false;
        toggle_EditMode.style.flexGrow = 1;  

        Button debugTree = new Button(() =>
        {
            PrintTree(tasklistData);
        });
        debugTree.text = "Print Folder Tree";
        debugTree.name = "DebugTreeButton";
        debugTree.style.flexGrow = 1;

        Button reloadUI = new Button(() =>
        {
            UIHandler.ReloadUI(tasklistData, TaskList);
        });
        reloadUI.text = "Reload UI";
        reloadUI.name = "ReloadUIButton";
        reloadUI.style.flexGrow = 1;


        VisualElement DataHandling = new VisualElement();
        DataHandling.name = "DataHandling Settings";
        DataHandling.style.width = Length.Percent(100);
        DataHandling.style.flexDirection = FlexDirection.Row;

        Button Save = new Button(() =>
        {
            DataPersistence.SaveData(tasklistData);
        });
        Save.name = "Saving Button";
        Save.text = "Save";
        Save.style.flexGrow = 1;
        Save.style.alignContent = Align.Center;

        Button Load = new Button(() =>
        {
            tasklistData = DataPersistence.LoadData();
            UIHandler.ReloadUI(tasklistData, TaskList);
        });
        Load.name = "Loading Button";
        Load.text = "Load";
        Load.style.flexGrow = 1;
        Load.style.alignContent = Align.Center;


        TaskList = new VisualElement();
        TaskList.name = "TaskList";
        TaskList.style.flexDirection = FlexDirection.Column;
        TaskList.style.width = Length.Percent(100);

        #endregion Navigation UI

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


        ScrollView scroll = new ScrollView();
        scroll.style.alignSelf = Align.FlexStart;
        scroll.style.flexGrow = 1;
        scroll.style.width = Length.Percent(100);
        scroll.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;

        scroll.Add(TaskList);
        root.Add(scroll);
        UIHandler.ReloadUI(tasklistData, TaskList);
    }

    public static void ToDoReloadUI()
    {
        UIHandler.ReloadUI(tasklistData, TaskList);
    }
}
