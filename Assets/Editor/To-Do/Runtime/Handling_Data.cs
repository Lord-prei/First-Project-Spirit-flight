using System.Collections.Generic;


namespace Handling.Data
{
    // Data structure for button styling
    public class ButtonStyleData
    {
        public int width;
        public int height;
    }

    public abstract class ToDoNode
    {
        public ToDoFolderData parent;
    }

    // Data structure for a to-do item
    public class ToDoItemData : ToDoNode
    {
        public bool done = false;
        public string name = "TaskSUS";
        public string description = "DIE";
        public bool desFolded = true;

        // Override ToString for easy debugging
        public override string ToString()
        {
            return $"TodoItemData\ndone =\t{done}\ntitle =\t\t{name}\ndescr =\t{description}\ndesFolded =\t{desFolded}\nparent =\t{parent}";
        }
    }

    // Data structure for a to-do folder
    public class ToDoFolderData : ToDoNode
    {
        public string name = "FolderSUS";

        public List<ToDoNode> children = new();
    }
}
