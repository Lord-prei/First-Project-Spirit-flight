using NUnit.Framework.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ToDo : EditorWindow
{
    

    [MenuItem("Window/UI Toolkit/ToDo")]
    public static void ShowExample()
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

        public override string ToString()
        {
            return $"TodoItemData\ndone =\t{done}\ntitle =\t\t{title}\ndescr =\t{description}\ndesFolded =\t{desFolded}";
        }
    }

    private VisualElement CreateHorizontalFoldout(ToDoItemData data)
    {
        // Create a new VisualElement to represent the horizontal foldout
        VisualElement root = new VisualElement();
        root.style.flexDirection = FlexDirection.Row;

        Button foldoutButton = new Button();
        ToggleFoldout(); // Set initial state
        foldoutButton.clickable.clicked += () =>
        {
            ToggleFoldout();
        };
        

        root.Add(foldoutButton);
        

        return root; // Ensure a VisualElement is returned to fix CS0161

        void ToggleFoldout()
        {
            data.desFolded = !data.desFolded;
            //change Symbol on button
            if (data.desFolded)
                foldoutButton.text = "▶"; // Folded symbol
            else
                foldoutButton.text = "▼"; // Unfolded symbol
            //Debug.Log(data);
        }
    }
    private VisualElement CreateToDoItemElement(ToDoItemData data)
    {
        // Create a new VisualElement to represent the to-do item
        VisualElement row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;

        // Create an Event Listener to save input in data when the user interacts with the UI
        // Toggle for marking the task as done
        Toggle done = new Toggle();
        done.value = data.done;
        done.RegisterValueChangedCallback(evt =>
        {
            data.done = evt.newValue;
            //Debug.Log(data);
        });
        

        // Title field for the task
        TextField title = new TextField();
        title.value = data.title;
        title.style.flexGrow = 1;
        title.RegisterValueChangedCallback(evt =>
        {
            data.title = evt.newValue;
            //Debug.Log(data);
        });

        TextField description = new TextField();
        description.value = data.description;
        description.style.flexGrow = 1;
        description.multiline = true;
        description.RegisterValueChangedCallback(evt =>
        {
            data.description = evt.newValue;
            //Debug.Log(data);
        });


        // Button for deleting the task
        Button deleteButton = new Button(() =>
        {
            rootVisualElement.Remove(row);
        });
        deleteButton.text = "X";

        // Description foldout
        Foldout test = new Foldout();
        test.text = "Description";
        test.style.flexGrow = 1;
        test.value = false;
        test.RegisterValueChangedCallback(evt =>
        {
            data.desFolded = evt.newValue;
            if (data.desFolded)
                title.style.display = DisplayStyle.None;
            else
                title.style.display = DisplayStyle.Flex;
            //Debug.Log(data);
        });

        // Add the UI elements to the row
        row.Add(done);
        row.Add(title);
        
        row.Add(test);
        test.Add(description);

        row.Add(deleteButton);

        Debug.Log(data);

        return row; // Ensure a VisualElement is returned to fix CS0161
    }
    
    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        ShowExample();

        Button addButton = new Button(() =>
        {
            ToDoItemData initVal = new ToDoItemData();
            initVal.done = false;
            initVal.title = $"New Task {rootVisualElement.childCount}";
            initVal.description = $"New Description {rootVisualElement.childCount}";
            initVal.desFolded = false;

            root.Add(CreateToDoItemElement(initVal));
        });


        addButton.text = "Add Task";
        root.Add(addButton);

        ToDoItemData test = new ToDoItemData();
        test.done = false;
        test.title = $"New Task {rootVisualElement.childCount}";
        test.description = $"New Description {rootVisualElement.childCount}";
        test.desFolded = false;
        root.Add(CreateHorizontalFoldout(test));
    }
}
