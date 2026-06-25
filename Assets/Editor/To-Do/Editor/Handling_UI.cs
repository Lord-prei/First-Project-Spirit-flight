using Handling.Data;
using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

namespace Handling.UI
{
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

    class UIHandler
    {
        // Method to create a horizontal foldout for the description of a to-do item
        public static VisualElement CreateHorizontalFoldout(ItemData data, TextField parentTitle)
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
            foldoutButton = DataHandler.ChangeButtonStyle(foldoutButton, buttonData);

            foldoutButton.style.backgroundColor =
                ToDo.editMode ? Color.darkGray : StyleKeyword.Null;

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
                    data.folded = !data.folded;

                //change Symbol on button
                if (data.folded) //Folded
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
        public static VisualElement CreateToDoItemElement(ItemData data)
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
            row.style.flexGrow = 1;


            // Create an Event Listener to save input in data when the user interacts with the UI
            // Toggle for marking the task as done
            Toggle completed = new Toggle();
            completed.name = "CheckBox";
            completed.value = data.completed;
            completed.RegisterValueChangedCallback(evt =>
            {
                data.completed = evt.newValue;
                //Debug.Log(data);
            });


            // Title field for the task
            TextField title = new TextField();
            title.name = "TaskName";
            title.value = data.name;
            title.style.flexGrow = 1;
            title.RegisterValueChangedCallback(evt =>
            {
                data.name = evt.newValue;
                //Debug.Log(data);
            });


            // Button for deleting the task
            Button deleteButton = new Button(() =>
            {
                data.parent?.children.Remove(data); // Remove the task from its parent's children list

                ToDo.ToDoReloadUI();
            });
            deleteButton.name = "deleteButton";
            deleteButton.text = "X";
            deleteButton.style.marginLeft = Length.Auto(); // Push the delete button to the right end of the row
            deleteButton = DataHandler.ChangeButtonStyle(deleteButton, buttonData);



            // EDIT BUTTONS

            VisualElement EditButtons = new VisualElement();
            EditButtons.name = "EditButtons";
            EditButtons.style.flexDirection = FlexDirection.Row;
            EditButtons.style.marginLeft = Length.Auto(); // Push the edit buttons to the right end of the row

            // Set the visibility of the edit buttons based on the edit mode
            EditButtons.style.display =
                ToDo.editMode ? DisplayStyle.Flex : DisplayStyle.None;
            deleteButton.style.display =
                ToDo.editMode ? DisplayStyle.None : DisplayStyle.Flex;


            // UP
            Button button_Up = new Button(() =>
            {
                DataMovement.MoveUP(data);

                ToDo.ToDoReloadUI();
            });
            button_Up.name = "ButtonUp";
            button_Up.text = "▲";
            button_Up = DataHandler.ChangeButtonStyle(button_Up, buttonData);

            // DOWN
            Button button_Down = new Button(() =>
            {
                DataMovement.MoveDOWN(data);

                ToDo.ToDoReloadUI();
            });
            button_Down.name = "ButtonDown";
            button_Down.text = "▼";
            button_Down = DataHandler.ChangeButtonStyle(button_Down, buttonData);

            // LEFT
            Button button_Left = new Button(() =>
            {
                DataMovement.MoveOutOffFolder(data);

                ToDo.ToDoReloadUI();
            });
            button_Left.name = "ButtonLeft";
            button_Left.text = "◀";
            button_Left = DataHandler.ChangeButtonStyle(button_Left, buttonData);

            // RIGHT
            Button button_Right = new Button(() =>
            {
                DataMovement.MoveIntoFolder(data);

                ToDo.ToDoReloadUI();
            });
            button_Right.name = "ButtonRight";
            button_Right.text = "▶";
            button_Right = DataHandler.ChangeButtonStyle(button_Right, buttonData);

            EditButtons.Add(button_Up);
            EditButtons.Add(button_Down);
            EditButtons.Add(button_Left);
            EditButtons.Add(button_Right);



            // Create the horizontal foldout for the description
            VisualElement foldOut = CreateHorizontalFoldout(data, title);


            // Add the UI elements to the row
            row.Add(completed);
            row.Add(title);
            row.Add(foldOut);
            row.Add(EditButtons);
            row.Add(deleteButton);

            //Debug.Log(data);

            return row; // Ensure a VisualElement is returned to fix CS0161
        }

        // Method to create a VisualElement for a to-do folder based on the provided data
        public static VisualElement CreateFolder(ItemData data, out VisualElement childrenContainer)
        {
            ButtonStyleData buttonData = new ButtonStyleData();
            buttonData.width = 20;
            buttonData.height = 20;

            VisualElement root = new VisualElement();
            root.name = "FolderRoot";
            root.style.flexDirection = FlexDirection.Row;
            root.style.width = Length.Auto();

            Foldout foldout = new Foldout();
            foldout.value = !data.folded;
            foldout.RegisterValueChangedCallback(evt =>
            {
                data.folded = !evt.newValue;
            });
            foldout.name = "FolderFoldout";
            foldout.text = data.name;
            foldout.style.flexGrow = 1;
            foldout.style.width = Length.Percent(100);

            // Button for deleting the task
            Button deleteButton = new Button(() =>
            {
                data.parent?.children.Remove(data); // Remove the task from its parent's children list

                ToDo.ToDoReloadUI();
            });

            deleteButton.name = "deleteButton";
            deleteButton.text = "X";
            //deleteButton.style.marginLeft = Length.Auto(); // Push the delete button to the right end of the row
            deleteButton.style.position = Position.Absolute; // Position the delete button absolutely within the row
            deleteButton.style.right = 0; // Align the delete button to the right edge of the row
            deleteButton = DataHandler.ChangeButtonStyle(deleteButton, buttonData);

            childrenContainer = new VisualElement();
            childrenContainer.name = "ChildrenContainer";

            // EDIT BUTTONS

            VisualElement EditButtons = new VisualElement();
            EditButtons.name = "EditButtons";
            EditButtons.style.flexDirection = FlexDirection.Row;
            //EditButtons.style.marginLeft = Length.Auto(); // Push the edit buttons to the right end of the row
            EditButtons.style.position = Position.Absolute; // Position the edit buttons absolutely within the row
            EditButtons.style.right = 0; // Align the edit buttons to the right edge of the row

            // Set the visibility of the edit buttons based on the edit mode
            EditButtons.style.display =
                ToDo.editMode ? DisplayStyle.Flex : DisplayStyle.None;
            deleteButton.style.display =
                ToDo.editMode ? DisplayStyle.None : DisplayStyle.Flex;


            // UP
            Button button_Up = new Button(() =>
            {
                DataMovement.MoveUP(data);

                ToDo.ToDoReloadUI();
            });
            button_Up.name = "ButtonUp";
            button_Up.text = "▲";
            button_Up = DataHandler.ChangeButtonStyle(button_Up, buttonData);

            // DOWN
            Button button_Down = new Button(() =>
            {
                DataMovement.MoveDOWN(data);

                ToDo.ToDoReloadUI();
            });
            button_Down.name = "ButtonDown";
            button_Down.text = "▼";
            button_Down = DataHandler.ChangeButtonStyle(button_Down, buttonData);

            // LEFT
            Button button_Left = new Button(() =>
            {
                DataMovement.MoveOutOffFolder(data);

                ToDo.ToDoReloadUI();
            });
            button_Left.name = "ButtonLeft";
            button_Left.text = "◀";
            button_Left = DataHandler.ChangeButtonStyle(button_Left, buttonData);

            // RIGHT
            Button button_Right = new Button(() =>
            {
                DataMovement.MoveIntoFolder(data);

                ToDo.ToDoReloadUI();
            });
            button_Right.name = "ButtonRight";
            button_Right.text = "▶";
            button_Right = DataHandler.ChangeButtonStyle(button_Right, buttonData);

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

        public static void ReloadUI(ItemData dataRoot, VisualElement visualRoot)
        {
            visualRoot.Clear();

            foreach (var child in dataRoot.children)
            {
                if (child.type == NodeType.Task)
                {
                    VisualElement ToDoItem = CreateToDoItemElement(child);

                    visualRoot.Add(ToDoItem);
                }
                else if (child.type == NodeType.Folder)
                {
                    VisualElement folderUI = CreateFolder(child, out var container);

                    visualRoot.Add(folderUI);

                    ReloadUI(child, container);
                }
            }
        }
    }
}
