using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


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

    public class DataMovement
    {
        public static void MoveUP(ToDoNode node)
        {
            // Get the parent of the node and check if it exists
            var parent = node.parent;
            if (parent == null)
            {
                Debug.LogWarning("Cannot move up: node has no parent.");
                return;
            }

            // Getting the List of children from the parent and finding the index of the node
            var siblings = parent.children;
            int index = siblings.IndexOf(node);

            if (index <= 0)
                return; // Node is already at the top or not found

            // tuple Swap
            (siblings[index - 1], siblings[index]) = (siblings[index], siblings[index - 1]);
        }

        public static void MoveDOWN(ToDoNode node)
        {
            // Get the parent of the node and check if it exists
            var parent = node.parent;
            if (parent == null)
            {
                Debug.LogWarning("Cannot move up: node has no parent.");
                return;
            }

            // Getting the List of children from the parent and finding the index of the node
            var siblings = parent.children;
            int index = siblings.IndexOf(node);

            if (index < 0 || index >= siblings.Count - 1) 
                return; // return when you are the last sibling

            // tuple Swap
            (siblings[index + 1], siblings[index]) = (siblings[index], siblings[index + 1]);
        }

        public static void MoveIntoFolder(ToDoNode node)
        {
            // Get the parent of the node and check if it exists
            var parent = (node.parent);
            if (parent == null)
                return;

            var siblings = parent.children;

            int index = siblings.IndexOf(node);

            // Need smt above us
            if (index <= 0)
                return;

            // Check if item above is a Folder
            if (siblings[index - 1] is not ToDoFolderData targetFolder)
                return;

            // Remove from current parent
            siblings.RemoveAt(index);

            // Add to folder
            targetFolder.children.Add(node);

            // Update parent reference
            node.parent = targetFolder;
        }

        public static void MoveOutOffFolder(ToDoNode node)
        {
            // Get the currentFolder (parent) of the node and check if it exists
            var currentFolder = (node.parent);
            if (currentFolder == null)
                return;

            
            var grandParent = currentFolder.parent;
            if (grandParent == null)
                return; // Prevents moving Out off root

            // Remove from current folder
            currentFolder.children.Remove(node);

            // Insert below current folder
            int folderIndex = grandParent.children.IndexOf(currentFolder);

            grandParent.children.Insert(folderIndex + 1, node);

            // Update parent
            node.parent = grandParent;
        }
    }
}
