using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ToDo : EditorWindow
{
    private int maxLayer = 5;

    [MenuItem("Window/UI Toolkit/ToDo")]
    public static void ShowExample()
    {
        ToDo wnd = GetWindow<ToDo>();
        wnd.titleContent = new GUIContent("ToDo");
    }

    class ToDoItemData
    {
        public bool done;
        public string title;
        public string description;
        public int Layer;

        public override string ToString()
        {
            return $"TodoItemData\ndone =\t{done}\ntitle =\t\t{title}\ndescr =\t{description}\nLayer =\t{Layer}";
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
        

        // Arrow buttons for changing the layer of the task
        Button arrowLeft = new Button(() =>
        {
            if (data.Layer <= 1) return;
            data.Layer--;
            Debug.Log(data);
        });
        arrowLeft.text = "<";


        // Arrow buttons for changing the layer of the task
        Button arrowRight = new Button(() =>
        {
            if (data.Layer >= maxLayer) return;
            data.Layer++;
            row.Insert(data.Layer - 1, new Label("->"));
            Debug.Log(data);
        });
        arrowRight.text = ">";


        // Button for deleting the task
        Button deleteButton = new Button(() =>
        {
            rootVisualElement.Remove(row);
        });
        deleteButton.text = "X";


        // Add the UI elements to the row
        row.Add(done);
        row.Add(title);
        row.Add(arrowLeft);
        row.Add(arrowRight);
        row.Add(deleteButton);

        Debug.Log(data);

        return row; // Ensure a VisualElement is returned to fix CS0161
    }
    
    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        Button addButton = new Button(() =>
        {
            ToDoItemData initVal = new ToDoItemData();
            initVal.done = false;
            initVal.title = $"New Task {rootVisualElement.childCount}";
            initVal.description = $"New Description {rootVisualElement.childCount}";
            initVal.Layer = 1;

            root.Add(CreateToDoItemElement(initVal));
        });


        addButton.text = "Add Task";
        root.Add(addButton);
    }
}
