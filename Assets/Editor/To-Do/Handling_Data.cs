using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Handling.Data
{
    // Data structure for button styling
    class ButtonStyleData
    {
        public int width;
        public int height;
    }

    // Data structure for a to-do item
    class ToDoItemData
    {
        public bool done;
        public string title;
        public string description;
        public bool desFolded;
        public string path = "root";

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

    internal class DataHandler
    {
        // Method to change the style of a button based on the provided data
        internal static Button ChangeButtonStyle(Button button, ButtonStyleData data)
        {
            button.style.width = Length.Pixels(data.width);
            button.style.height = Length.Pixels(data.height);
            button.style.alignContent = Align.Center;

            return button;
        }
    }
}
