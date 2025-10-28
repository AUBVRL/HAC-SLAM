using System.Collections.Generic;
using UnityEngine;

public static class VoxelUtilities
{
    // Expose RoundToVoxel/Chunk accessors to the manager (you already have these functions)
    // Replace these calls with your actual implementations if their names differ
    public static Vector3 RoundToVoxel(Vector3 point) => VoxelManager.RoundToVoxel(point);
    public static Vector3 RoundToChunk(Vector3 voxel) => VoxelManager.RoundToChunk(voxel);

    // PUBLIC wrappers that record history
    public static void AddVoxelWithUndo(Vector3 point, bool state)
    {
        AddVoxelInternal(point, state, record: true);
    }

    public static void DeleteVoxelWithUndo(Vector3 point)
    {
        DeleteVoxelInternal(point, record: true);
    }

    // INTERNAL add (reusable by UndoRedoManager). When record==false, do not push into history.
    public static void AddVoxelInternal(Vector3 point, bool state, bool record)
    {
        Vector3 voxelVector = RoundToVoxel(point);
        Vector3 chunkVector = RoundToChunk(voxelVector);
        if (!VoxelManager.ChunksDict.ContainsKey(chunkVector))
        {
            VoxelManager.ChunksDict.Add(chunkVector, new Chunk(chunkVector));
        }
        if (!VoxelManager.ChunksDict[chunkVector].VoxelsDict.ContainsKey(voxelVector))
        {
            VoxelManager.ChunksDict[chunkVector].VoxelsDict.Add(voxelVector, new Voxel(voxelVector, state ? PrefabsManager.addedVoxelPrefab : PrefabsManager.voxelPrefab, true));
        }

        // Record action if requested
        if (record)
        {
            var action = new EditAction(EditType.Add, new Vector3[] { voxelVector }, new bool[] { state });
            UndoRedoManager2.RecordAction(action);
        }
    }

    // INTERNAL delete (reusable by UndoRedoManager). When record==false, do not push into history.
    public static void DeleteVoxelInternal(Vector3 point, bool record)
    {
        Vector3 voxelVector = RoundToVoxel(point);
        Vector3 boxSize = new Vector3(PrefabsManager.voxelSize - 0.001f, PrefabsManager.voxelSize - 0.001f, PrefabsManager.voxelSize - 0.001f);
        if (Physics.CheckBox(voxelVector, boxSize / 2, Quaternion.identity, 1 << 3))
        {
            Vector3 chunkVector = RoundToChunk(voxelVector);
            if (VoxelManager.ChunksDict.ContainsKey(chunkVector))
            {
                Chunk chunk = VoxelManager.ChunksDict[chunkVector];
                if (chunk.VoxelsDict.ContainsKey(voxelVector))
                {
                    Voxel voxel = chunk.VoxelsDict[voxelVector];

                    // Try to determine the "state" of this voxel so we can re-add it on undo.
                    // Adapt this if your Voxel has a stored field; if so, use that instead.
                    bool wasAddedState = false;
                    // If Voxel stores a flag, use it:
                    // wasAddedState = voxel.isAdded;
                    // Otherwise we heuristically compare prefab names (instance name contains prefab name + "(Clone)")
                    var go = voxel.prefab;
                    if (go != null)
                    {
                        var nameLower = go.name.ToLower();
                        if (nameLower.Contains(PrefabsManager.addedVoxelPrefab.name.ToLower()))
                            wasAddedState = true;
                    }

                    // Destroy voxel's GameObject
                    GameObject.Destroy(voxel.prefab);

                    // instantiate deleted placeholder (existing behavior)
                    GameObject.Instantiate(PrefabsManager.deletedVoxelPrefab, voxel.Position, Quaternion.identity, PrefabsManager.deletedVoxelPrefabParent.transform);

                    // remove from voxels dict
                    chunk.VoxelsDict.Remove(voxelVector);

                    // Record action if requested
                    if (record)
                    {
                        var action = new EditAction(EditType.Delete, new Vector3[] { voxelVector }, new bool[] { wasAddedState });
                        UndoRedoManager2.RecordAction(action);
                    }
                }
            }
        }
    }

    // Utility called by UndoRedoManager to remove any "deletedVoxelPrefab" instances at a given position.
    // Useful when undoing a delete (we re-add real voxel, so destroy the deleted placeholder).
    public static void RemoveDeletedPlaceholderAt(Vector3 position)
    {
        var parent = PrefabsManager.deletedVoxelPrefabParent;
        if (parent == null) return;
        float eps = 0.001f;
        foreach (Transform child in parent.transform)
        {
            if (Vector3.Distance(child.position, position) < eps)
            {
                GameObject.Destroy(child.gameObject);
                break;
            }
        }
    }

    // Call this at the *end* of a stroke/paint or after a selection-add operation
    public static void AddVoxelsWithUndo(List<Vector3> positions, List<bool> states)
    {
        // perform all adds without recording
        for (int i = 0; i < positions.Count; ++i)
            AddVoxelInternal(positions[i], states[i], record: false);

        // record one composite action
        var action = new EditAction(EditType.Add, positions.ToArray(), states.ToArray());
        UndoRedoManager2.RecordAction(action);
    }

    // Call this for deleting a selection (positions = set of voxel positions to delete)
    public static void DeleteVoxelsWithUndo(List<Vector3> selectorPoints)
    {
        var deletedPositions = new List<Vector3>();
        var deletedStates = new List<bool>();

        foreach (var rawPoint in selectorPoints)
        {
            // canonicalize to voxel grid
            Vector3 voxelVector = RoundToVoxel(rawPoint);

            // avoid duplicates from overlapping selector points
            if (deletedPositions.Contains(voxelVector)) continue;

            Vector3 boxSize = new Vector3(PrefabsManager.voxelSize - 0.001f,
                                          PrefabsManager.voxelSize - 0.001f,
                                          PrefabsManager.voxelSize - 0.001f);

            // check whether a voxel is actually present at this voxel position
            if (!Physics.CheckBox(voxelVector, boxSize / 2, Quaternion.identity, 1 << 3))
                continue;

            Vector3 chunkVector = RoundToChunk(voxelVector);
            if (!VoxelManager.ChunksDict.ContainsKey(chunkVector)) continue;

            var chunk = VoxelManager.ChunksDict[chunkVector];
            if (!chunk.VoxelsDict.ContainsKey(voxelVector)) continue;

            var voxel = chunk.VoxelsDict[voxelVector];

            // determine state (prefer a stored flag if available)
            bool wasAddedState = false;
            // if your Voxel class has isAdded or prefab reference, use it:
            // wasAddedState = voxel.isAdded;
            var go = voxel.prefab;
            if (go != null && go.name.ToLower().Contains(PrefabsManager.addedVoxelPrefab.name.ToLower()))
                wasAddedState = true;

            // perform deletion (use internal delete so it doesn't record itself)
            DeleteVoxelInternal(voxelVector, record: false);

            // collect for one composite undo action
            deletedPositions.Add(voxelVector);
            deletedStates.Add(wasAddedState);
        }

        // only record if we actually deleted something
        if (deletedPositions.Count > 0)
        {
            var action = new EditAction(EditType.Delete, deletedPositions.ToArray(), deletedStates.ToArray());
            UndoRedoManager2.RecordAction(action);
        }
    }


    // Convenience overload: single-voxel add -> wraps to batch API
    public static void AddVoxelsWithUndo(Vector3 position, bool state)
    {
        AddVoxelsWithUndo(new List<Vector3> { position }, new List<bool> { state });
    }

    // Convenience overload: single-voxel delete -> wraps to batch API
    public static void DeleteVoxelsWithUndo(Vector3 position)
    {
        DeleteVoxelsWithUndo(new List<Vector3> { position });
    }


}