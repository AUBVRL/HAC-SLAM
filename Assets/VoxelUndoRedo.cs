using System;
using System.Collections.Generic;
using UnityEngine;

// Simple enum describing the edit
public enum EditType { Add, Delete }

// Minimal record for an edit action
[Serializable]
public struct EditAction
{
    public EditType Type;               // Add or Delete
    public Vector3[] VoxelPositions;    // positions touched by this action (one or many)
    public bool[] AddedStates;          // same length: whether each voxel was 'added' (prefab choice)

    public EditAction(EditType type, Vector3[] positions, bool[] addedStates)
    {
        Type = type;
        VoxelPositions = positions;
        AddedStates = addedStates;
    }
}

// Static manager for undo/redo
public static class UndoRedoManager2
{
    private static readonly Stack<EditAction> undoStack = new Stack<EditAction>();
    private static readonly Stack<EditAction> redoStack = new Stack<EditAction>();
    private const int MaxHistory = 300; // tune to memory / UX

    public static bool CanUndo => undoStack.Count > 0;
    public static bool CanRedo => redoStack.Count > 0;

    public static void RecordAction(EditAction action)
    {
        undoStack.Push(action);
        // capacity trim
        if (undoStack.Count > MaxHistory)
        {
            // remove oldest: copy to array, skip last element
            var arr = undoStack.ToArray();
            Array.Resize(ref arr, arr.Length - 1);
            undoStack.Clear();
            for (int i = arr.Length - 1; i >= 0; --i) undoStack.Push(arr[i]);
        }
        // clear redo when new action is added
        redoStack.Clear();
    }

    public static void Undo()
    {
        if (undoStack.Count == 0) return;
        var action = undoStack.Pop();

        if (action.Type == EditType.Add)
        {
            // inverse of Add => remove all positions in this action
            foreach (var pos in action.VoxelPositions)
                VoxelUtilities.DeleteVoxelInternal(pos, record: false);

            // push same action into redo so redo reapplies the whole batch
            redoStack.Push(action);
        }
        else if (action.Type == EditType.Delete)
        {
            // inverse of Delete => re-add all voxels with their original states
            for (int i = 0; i < action.VoxelPositions.Length; ++i)
                VoxelUtilities.AddVoxelInternal(action.VoxelPositions[i], action.AddedStates[i], record: false);

            redoStack.Push(action);
        }
    }


    public static void Redo()
    {
        if (redoStack.Count == 0) return;
        var action = redoStack.Pop();

        if (action.Type == EditType.Add)
        {
            // reapply add for all
            for (int i = 0; i < action.VoxelPositions.Length; ++i)
                VoxelUtilities.AddVoxelInternal(action.VoxelPositions[i], action.AddedStates[i], record: false);

            undoStack.Push(action);
        }
        else if (action.Type == EditType.Delete)
        {
            // reapply delete for all
            foreach (var pos in action.VoxelPositions)
                VoxelUtilities.DeleteVoxelInternal(pos, record: false);

            undoStack.Push(action);
        }
    }


    public static void Clear()
    {
        undoStack.Clear();
        redoStack.Clear();
    }
}