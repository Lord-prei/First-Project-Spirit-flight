using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;


namespace Handling.Data
{
    // Data structure for button styling
    public class ButtonStyleData
    {
        public int width;
        public int height;
    }

    public enum NodeType
    {
        Task,
        Folder
    }

    [Serializable]
    public class ItemData
    {
        public string name;
        public string description;
        public bool completed;
        public bool unfolded;

        public NodeType type;

        public List<ItemData> children = new();

        [NonSerialized]
        public ItemData parent;
    }

    public class DataPersistence
    {
        public static void RebuildParents(ItemData node, ItemData parent = null)
        {
            node.parent = parent;

            foreach(var child in node.children)
            {
                RebuildParents(child, node);
            }
        }

        public static void SaveData(ItemData root)
        {
            string folder = "Assets/TodoSaves";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder); // Creates folder if it doesn't exist

            string path = Path.Combine(folder, "todo.json");

            string json = JsonConvert.SerializeObject(root, Formatting.Indented); // Saving root as a json string

            File.WriteAllText(path, json); // Saving File

            AssetDatabase.Refresh(); // Refresh unity Database
        }

        public static ItemData LoadData()
        {
            string path = "Assets/TodoSaves/todo.json";

            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);

            ItemData root = JsonConvert.DeserializeObject<ItemData>(json); // converting json to ItemData Object (WITHOUT PARENTS)

            DataPersistence.RebuildParents(root);
            return root;
        }
    }

    public class DataMovement
    {
        public static void MoveUP(ItemData node)
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

        public static void MoveDOWN(ItemData node)
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

        public static void MoveIntoFolder(ItemData node)
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

            // Item above
            ItemData targetFolder = siblings[index - 1];

            // Check if item above is a Folder
            if (targetFolder.type != NodeType.Folder)
                return;

            // Remove from current parent
            siblings.RemoveAt(index);

            // Add to folder
            targetFolder.children.Add(node);

            // Update parent reference
            node.parent = targetFolder;
        }

        public static void MoveOutOffFolder(ItemData node)
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
