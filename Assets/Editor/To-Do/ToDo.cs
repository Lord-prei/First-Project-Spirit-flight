using Codice.Client.Common.GameUI;
using JetBrains.Annotations;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static UnityEngine.Rendering.DebugUI.Table;

public class ToDo : EditorWindow
{
    private bool editMode = true;
    private ToDoFolderData rootData;

    [MenuItem("Window/UI Toolkit/ToDo")] // Add menu item to open the To-Do window

    // Initialize the To-Do window
    public static void Init()
    {
        ToDo wnd = GetWindow<ToDo>();               // Get an instance of the To-Do window
        wnd.titleContent = new GUIContent("To-Do"); // Set the title of the window
    }

    // Data structure for button styling
    class ButtonStyleData
    {
        public int width;
        public int height;
    }

    // Data structure for a to-do item
    class ToDoItemData
    {
        public bool     done;
        public string   title;
        public string   description;
        public bool     desFolded;
        public string   path = "root";

        // Override ToString for easy debugging
        public override string ToString()
        {
            return $"TodoItemData\ndone =\t{done}\ntitle =\t\t{title}\ndescr =\t{description}\ndesFolded =\t{desFolded}\npath =\t{path}";
        }
    }

    // Data structure for a to-do folder
    class ToDoFolderData
    {
        public string name;

        public ToDoFolderData parent;
        public List<ToDoFolderData> children = new List<ToDoFolderData>();

        // UI reference (important bridge)
        public VisualElement ui;
    }

    // Method to change the style of a button based on the provided data
    private Button ChangeButtonStyle(Button button, ButtonStyleData data)
    {
        button.style.width = Length.Pixels(data.width);
        button.style.height = Length.Pixels(data.height);
        button.style.alignContent = Align.Center;

        return button;
    }

    private void PrintFolderTree(ToDoFolderData node, int depth = 0)
    {
        string indent = new string(' ', depth * 2);

        Debug.Log($"{indent}- {node.name}");

        foreach (var child in node.children)
        {
            PrintFolderTree(child, depth + 1);
        }
    }

    // Method to create a horizontal foldout for the description of a to-do item
    private VisualElement CreateHorizontalFoldout(ToDoItemData data, TextField parentTitle)
    {

        // Data for styling the foldout button
        ButtonStyleData buttonData = new ButtonStyleData();
        buttonData.width = 20;
        buttonData.height = 20;

        // Create a new VisualElement to represent the horizontal foldout
        VisualElement root = new VisualElement();
        root.name = "foldoutRoot";
        root.style.flexDirection = FlexDirection.Row; // Arrange foldout button and content horizontally
        root.style.flexGrow = 0;    // Don't take up space when folded


        // Content of the foldout
        VisualElement content = new VisualElement();
        content.name = "foldoutContent";
        content.style.flexDirection = FlexDirection.Row; // Arrange description label and field horizontally

        // Set the content to take up remaining space when unfolded
        content.style.flexGrow = 1;
        content.style.flexBasis = Length.Percent(100);


        // Description field for the task
        TextField description = new TextField();
        description.name = "Description";
        description.value = data.description;
        description.style.flexDirection = FlexDirection.Row; // Arrange description label and field horizontally

        // Set the description field to take up remaining space when unfolded and allow multiline input
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
        descriptionLabel.style.unityTextAlign = TextAnchor.MiddleLeft; // Align text to the left and center vertically


        // Foldout button
        Button foldoutButton = new Button();
        foldoutButton.name = "foldoutButton";
        foldoutButton = ChangeButtonStyle(foldoutButton, buttonData);

        foldoutButton.style.backgroundColor =
            editMode ? Color.darkGray : StyleKeyword.Null;

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
            if (!init) // Only toggle foldout state on button click, not during initialization
                data.desFolded = !data.desFolded;

            //change Symbol on button
            if (data.desFolded) //Folded
            {
                root.style.flexGrow = 0;        // Don't take up space when folded
                foldoutButton.text = "▶";       // Folded symbol
                content.style.display = DisplayStyle.None;      // Hide content
                parentTitle.style.display = DisplayStyle.Flex;  // Show title when folded
            }
            else // Unfolded
            {
                root.style.flexGrow = 1;        // Take up space when unfolded
                foldoutButton.text = "▼";       // Unfolded symbol
                content.style.display = DisplayStyle.Flex;      // Show content
                parentTitle.style.display = DisplayStyle.None;  // Hide title when unfolded
            }
            //Debug.Log(data);
        }
    }

    // Method to create a VisualElement for a to-do item based on the provided data
    private VisualElement CreateToDoItemElement(ToDoItemData data)
    {
        // Data for styling the buttons
        ButtonStyleData buttonData = new ButtonStyleData();
        buttonData.width = 20;
        buttonData.height = 20;

        // Create a new VisualElement to represent the to-do item
        VisualElement row = new VisualElement();
        row.name = "TaskRoot";

        // Set the layout of the row to be horizontal and take up the full width of the parent
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
            //Debug.Log(data);
        });


        // Button for deleting the task
        Button deleteButton = new Button(() =>
        {
            row.parent?.Remove(row);
        });
        deleteButton.name = "deleteButton";
        deleteButton.text = "X";
        deleteButton.style.marginLeft = Length.Auto(); // Push the delete button to the right end of the row
        deleteButton = ChangeButtonStyle(deleteButton, buttonData);



        // EDIT BUTTONS

        VisualElement EditButtons = new VisualElement();
        EditButtons.name = "EditButtons";
        EditButtons.style.flexDirection = FlexDirection.Row;
        EditButtons.style.marginLeft = Length.Auto(); // Push the edit buttons to the right end of the row

        // Set the visibility of the edit buttons based on the edit mode
        EditButtons.style.display =
            editMode ? DisplayStyle.Flex : DisplayStyle.None;
        deleteButton.style.display =
            editMode ? DisplayStyle.None : DisplayStyle.Flex;


        // UP
        Button button_Up = new Button(() =>
        {
            Debug.Log("Move Up");
        });
        button_Up.name = "ButtonUp";
        button_Up.text = "▲";
        button_Up = ChangeButtonStyle(button_Up, buttonData);

        // DOWN
        Button button_Down = new Button(() =>
        {
            Debug.Log("Move Down");
        });
        button_Down.name = "ButtonDown";
        button_Down.text = "▼";
        button_Down = ChangeButtonStyle(button_Down, buttonData);

        // LEFT
        Button button_Left = new Button(() =>
        {
            Debug.Log("Move Left");
        });
        button_Left.name = "ButtonLeft";
        button_Left.text = "◀";
        button_Left = ChangeButtonStyle(button_Left, buttonData);

        // RIGHT
        Button button_Right = new Button(() =>
        {
            Debug.Log("Move Right");
        });
        button_Right.name = "ButtonRight";
        button_Right.text = "▶";
        button_Right = ChangeButtonStyle(button_Right, buttonData);

        EditButtons.Add(button_Up);
        EditButtons.Add(button_Down);
        EditButtons.Add(button_Left);
        EditButtons.Add(button_Right);



        // Create the horizontal foldout for the description
        VisualElement foldOut = CreateHorizontalFoldout(data, title);


        // Add the UI elements to the row
        row.Add(done);
        row.Add(title);
        row.Add(foldOut);
        row.Add(EditButtons);
        row.Add(deleteButton);

        //Debug.Log(data);

        return row; // Ensure a VisualElement is returned to fix CS0161
    }

    // Method to create a VisualElement for a to-do folder based on the provided data
    private VisualElement CreateFolder(ToDoFolderData data)
    {
        ButtonStyleData buttonData = new ButtonStyleData();
        buttonData.width = 20;
        buttonData.height = 20;

        VisualElement root = new VisualElement();
        root.name = "FolderRoot";
        root.style.flexDirection = FlexDirection.Row;
        root.style.height = Length.Pixels(22);

        Foldout foldout = new Foldout();
        foldout.name = "FolderFoldout";
        foldout.text = data.name;

        // Button for deleting the task
        Button deleteButton = new Button(() =>
        {
            root.parent?.Remove(root);
        });
        deleteButton.name = "deleteButton";
        deleteButton.text = "X";
        deleteButton.style.marginLeft = Length.Auto(); // Push the delete button to the right end of the row
        deleteButton = ChangeButtonStyle(deleteButton, buttonData);

        VisualElement childrenContainer = new VisualElement();
        childrenContainer.name = "ChildrenContainer";

        // EDIT BUTTONS

        VisualElement EditButtons = new VisualElement();
        EditButtons.name = "EditButtons";
        EditButtons.style.flexDirection = FlexDirection.Row;
        EditButtons.style.marginLeft = Length.Auto(); // Push the edit buttons to the right end of the row

        // Set the visibility of the edit buttons based on the edit mode
        EditButtons.style.display =
            editMode ? DisplayStyle.Flex : DisplayStyle.None;
        deleteButton.style.display =
            editMode ? DisplayStyle.None : DisplayStyle.Flex;


        // UP
        Button button_Up = new Button(() =>
        {
            Debug.Log("Move Up");
        });
        button_Up.name = "ButtonUp";
        button_Up.text = "▲";
        button_Up = ChangeButtonStyle(button_Up, buttonData);

        // DOWN
        Button button_Down = new Button(() =>
        {
            Debug.Log("Move Down");
        });
        button_Down.name = "ButtonDown";
        button_Down.text = "▼";
        button_Down = ChangeButtonStyle(button_Down, buttonData);

        // LEFT
        Button button_Left = new Button(() =>
        {
            Debug.Log("Move Left");
        });
        button_Left.name = "ButtonLeft";
        button_Left.text = "◀";
        button_Left = ChangeButtonStyle(button_Left, buttonData);

        // RIGHT
        Button button_Right = new Button(() =>
        {
            Debug.Log("Move Right");
        });
        button_Right.name = "ButtonRight";
        button_Right.text = "▶";
        button_Right = ChangeButtonStyle(button_Right, buttonData);

        EditButtons.Add(button_Up);
        EditButtons.Add(button_Down);
        EditButtons.Add(button_Left);
        EditButtons.Add(button_Right);


        root.Add(foldout);
        foldout.Add(childrenContainer);

        root.Add(EditButtons);
        root.Add(deleteButton);

        return root;
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

        Init();

        Button button_AddTask = new Button(() =>
        {
            ToDoItemData initVal = new ToDoItemData();
            initVal.done = false;
            initVal.title = $"New Task {rootVisualElement.childCount}";
            initVal.description = $"New Description {rootVisualElement.childCount}";
            initVal.desFolded = true;

            TaskList.Add(CreateToDoItemElement(initVal));
        });
        button_AddTask.name = "AddTaskButton";
        button_AddTask.text = "Add Task";
        button_AddTask.style.width = Length.Percent(25);


        Button button_AddFolder = new Button(() =>
        {
            ToDoFolderData newFolder = new ToDoFolderData();
            newFolder.name = $"New Folder {rootData.children.Count}";
            newFolder.parent = rootData;

            // attach to root data tree
            rootData.children.Add(newFolder);

            // create UI
            VisualElement folderUI = CreateFolder(newFolder);
            newFolder.ui = folderUI;

            TaskList.Add(folderUI);
            //Debug.Log("Add Folder");
        });

        Button debugTree = new Button(() =>
        {
            PrintFolderTree(rootData);
        });
        debugTree.text = "Print Folder Tree";
        debugTree.style.width = Length.Percent(25);

        button_AddFolder.name = "AddFolderButton";
        button_AddFolder.text = "Add Folder";
        button_AddFolder.style.width = Length.Percent(25);

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


        root.Add(ControlMenu);
        ControlMenu.Add(button_AddTask);
        ControlMenu.Add(button_AddFolder);
        ControlMenu.Add(toggle_EditMode);
        ControlMenu.Add(debugTree);

        root.Add(TaskList);

    }
}
