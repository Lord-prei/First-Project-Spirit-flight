using JetBrains.Annotations;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class ToDo : EditorWindow
{
    [MenuItem("Window/UI Toolkit/ToDo")]
    public static void Init()
    {
        ToDo wnd = GetWindow<ToDo>();
        wnd.titleContent = new GUIContent("To-Do");
    }

    class ToDoItemData
    {
        public bool done;
        public string title;
        public string description;
        public bool desFolded;
        public string path = "root";

        public override string ToString()
        {
            return $"TodoItemData\ndone =\t{done}\ntitle =\t\t{title}\ndescr =\t{description}\ndesFolded =\t{desFolded}\npath =\t{path}";
        }
    }

    private VisualElement CreateHorizontalFoldout(ToDoItemData data, TextField parentTitle)
    {
        // Create a new VisualElement to represent the horizontal foldout
        VisualElement root = new VisualElement();
        root.name = "foldoutRoot";
        root.style.flexDirection = FlexDirection.Row;
        root.style.flexGrow = 0;

        // Content of the foldout
        VisualElement content = new VisualElement();
        content.name = "foldoutContent";
        content.style.flexDirection = FlexDirection.Row;
        content.style.flexGrow = 1;
        content.style.flexBasis = Length.Percent(100);

        // Description field for the task
        TextField description = new TextField();
        description.name = "Description";
        description.value = data.description;
        description.style.flexDirection = FlexDirection.Row;
        description.style.flexGrow = 1;
        description.multiline = true;
        description.RegisterValueChangedCallback(evt =>
        {
            data.description = evt.newValue;
            //Debug.Log(data);
        });

        // Label for Description
        Label descriptionLabel = new Label("Desc:");
        descriptionLabel.name = "descriptionLabel";
        descriptionLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

        // Foldout button
        Button foldoutButton = new Button();
        foldoutButton.name = "foldoutButton";
        foldoutButton.clickable.clicked += () =>
        {
            ToggleFoldout(false);
        };
        ToggleFoldout(true);

        // Structure of object:
        // -------------------- Root
        root.Add(foldoutButton);
        root.Add(content);
        // --------------- Content
        content.Add(descriptionLabel);
        content.Add(description);


        return root; // Ensure a VisualElement is returned to fix CS0161

        void ToggleFoldout(bool init)
        {
            if (!init)
                data.desFolded = !data.desFolded;
            //change Symbol on button
            if (data.desFolded) //Folded
            {
                root.style.flexGrow = 0; // Don't take up space when folded
                foldoutButton.text = "▶"; // Folded symbol
                content.style.display = DisplayStyle.None; // Hide content
                parentTitle.style.display = DisplayStyle.Flex; // Show title when folded
            }
            else // Unfolded
            {
                root.style.flexGrow = 1; // Take up space when unfolded
                foldoutButton.text = "▼"; // Unfolded symbol
                content.style.display = DisplayStyle.Flex; // Show content
                parentTitle.style.display = DisplayStyle.None; // Hide title when unfolded
            }
            //Debug.Log(data);
        }
    }
    private VisualElement CreateToDoItemElement(ToDoItemData data)
    {
        // Create a new VisualElement to represent the to-do item
        VisualElement row = new VisualElement();
        row.name = "TaskRoot";
        row.style.flexDirection = FlexDirection.Row;
        row.style.width = Length.Percent(100);

        // Create an Event Listener to save input in data when the user interacts with the UI
        // Toggle for marking the task as done
        Toggle done = new Toggle();
        done.name = "CheckBox";
        done.value = data.done;
        done.RegisterValueChangedCallback(evt =>
        {
            data.done = evt.newValue;
            //Debug.Log(data);
        });
        

        // Title field for the task
        TextField title = new TextField();
        title.name = "TaskName";
        title.value = data.title;
        title.style.flexGrow = 1;
        title.RegisterValueChangedCallback(evt =>
        {
            data.title = evt.newValue;
            Debug.Log(data);
        });


        // Button for deleting the task
        Button deleteButton = new Button(() =>
        {
            rootVisualElement.Remove(row);
        });
        deleteButton.name = "deleteButton";
        deleteButton.text = "X";
        deleteButton.style.marginLeft = Length.Auto();


        // Create the horizontal foldout for the description
        VisualElement foldOut = CreateHorizontalFoldout(data, title);


        // Add the UI elements to the row
        row.Add(done);
        row.Add(title);
        row.Add(foldOut);
        row.Add(deleteButton);

        Debug.Log(data);

        return row; // Ensure a VisualElement is returned to fix CS0161
    }
    private VisualElement CreateFolder()
    {
        VisualElement root = new VisualElement();
        root.name = "FolderRoot";


        return root;
    }
    
    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        root.name = "Root";

        VisualElement ControlMenu = new VisualElement();
        ControlMenu.name = "ControlMenu";
        ControlMenu.style.flexDirection = FlexDirection.Row;
        ControlMenu.style.width = Length.Percent(100);

        VisualElement TaskList = new VisualElement();
        TaskList.name = "TaskList";
        TaskList.style.flexDirection = FlexDirection.Row;
        TaskList.style.width = Length.Percent(100);

        Init();

        Button button_AddTask = new Button(() =>
        {
            ToDoItemData initVal = new ToDoItemData();
            initVal.done = false;
            initVal.title = $"New Task {rootVisualElement.childCount}";
            initVal.description = $"New Description {rootVisualElement.childCount}";
            initVal.desFolded = true;

            root.Add(CreateToDoItemElement(initVal));
        });
        button_AddTask.name = "AddTaskButton";
        button_AddTask.text = "Add Task";
        button_AddTask.style.width = Length.Percent(50);


        Button button_AddFolder = new Button(() =>
        {
            Debug.Log("Add Folder");
        });
        button_AddFolder.name = "AddFolderButton";
        button_AddFolder.text = "Add Folder";
        button_AddFolder.style.width = Length.Percent(50);


        root.Add(ControlMenu);
        ControlMenu.Add(button_AddTask);
        ControlMenu.Add(button_AddFolder);

        root.Add(TaskList);

    }
}
