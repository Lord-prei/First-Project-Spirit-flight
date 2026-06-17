using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

using Handling.UI;
using Handling.Data;

public class ToDo : EditorWindow
{
    public static bool editMode = true;
    private ToDoFolderData rootData;

    [MenuItem("Window/ToDo")] // Add menu item to open the To-Do window

    // Initialize the To-Do window
    public static void Init()
    {
        ToDo wnd = GetWindow<ToDo>();               // Get an instance of the To-Do window
        wnd.titleContent = new GUIContent("To-Do"); // Set the title of the window
    }



    private void PrintTree(ToDoFolderData root)
    {
        var sb = new System.Text.StringBuilder();

        BuildTree(root, 0, sb);

        Debug.Log(sb.ToString());
    }

    private void BuildTree(ToDoFolderData folder, int depth, System.Text.StringBuilder sb)
    {
        string indent = new string(' ', depth * 2);

        sb.AppendLine($"{indent}[Folder] {folder.name}");

        foreach(var child in folder.children)
        {
            if (child is ToDoItemData)
            {
                sb.AppendLine($"{indent}  [Task] {((ToDoItemData)child).name}");
            }
            if (child is ToDoFolderData)
            {
                BuildTree((ToDoFolderData)child, depth + 1, sb);
            }
        }
    }

    
    
    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        root.name = "Root";

        rootData = new ToDoFolderData();
        rootData.name = "Root";

        VisualElement ControlMenu = new VisualElement();
        ControlMenu.name = "ControlMenu";
        ControlMenu.style.flexDirection = FlexDirection.Row;
        ControlMenu.style.width = Length.Percent(100);

        VisualElement TaskList = new VisualElement();
        TaskList.name = "TaskList";
        TaskList.style.flexDirection = FlexDirection.Column;
        TaskList.style.width = Length.Percent(100);



        //Button button_AddTask = new Button(() =>
        //{
        //    ToDoItemData initVal = new ToDoItemData();
        //    initVal.done = false;
        //    initVal.name = $"New Task {rootVisualElement.childCount}";
        //    initVal.description = $"New Description {rootVisualElement.childCount}";
        //    initVal.desFolded = true;
        //    initVal.parent = rootData;

        //    rootData.children.Add(initVal);

        //    TaskList.Add(CreateToDoItemElement(initVal));
        //});
        //button_AddTask.name = "AddTaskButton";
        //button_AddTask.text = "Add Task";
        //button_AddTask.style.width = Length.Percent(25);


        //Button button_AddFolder = new Button(() =>
        //{
        //    ToDoFolderData newFolder = new ToDoFolderData();
        //    newFolder.name = $"New Folder {rootData.children.Count}";
        //    newFolder.parent = rootData;

        //    // attach to root data tree
        //    rootData.children.Add(newFolder);

        //    // create UI
        //    VisualElement folderUI = CreateFolder(newFolder);

        //    TaskList.Add(folderUI);
        //    //Debug.Log("Add Folder");
        //});

        //button_AddFolder.name = "AddFolderButton";
        //button_AddFolder.text = "Add Folder";
        //button_AddFolder.style.width = Length.Percent(25);

        Toggle toggle_EditMode = new Toggle();
        toggle_EditMode.name = "ToggleEditMode";
        toggle_EditMode.text = "Edit Mode";
        toggle_EditMode.style.width = Length.Percent(25);
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
        debugTree.style.width = Length.Percent(25);
        Button reloadUI = new Button(() =>
        {
            UIHandler.ReloadUI(rootData, root);
        });


        rootData.children.Add(new ToDoItemData { name = "Task 3" });

        rootData.children.Add(new ToDoItemData { name = "Task 1" });
        ToDoFolderData folder1 = new ToDoFolderData { name = "Folder 1" };
        rootData.children.Add(folder1);
        folder1.children.Add(new ToDoItemData { name = "Task 2" });

        ToDoFolderData folder2 = new ToDoFolderData { name = "Folder 2" };
        folder1.children.Add(folder2);
        folder2.children.Add(new ToDoItemData { name = "Task 4" });

        rootData.children.Add(new ToDoItemData { name = "Task 5" });

        root.Add(ControlMenu);

        //ControlMenu.Add(button_AddTask);
        //ControlMenu.Add(button_AddFolder);
        ControlMenu.Add(toggle_EditMode);
        ControlMenu.Add(debugTree);


        root.Add(TaskList);
        UIHandler.ReloadUI(rootData, TaskList);

    }
}
