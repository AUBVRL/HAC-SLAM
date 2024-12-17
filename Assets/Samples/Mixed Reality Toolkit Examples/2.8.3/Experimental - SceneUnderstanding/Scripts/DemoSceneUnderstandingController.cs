// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Examples.Demos;
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.SceneUnderstanding
{
    /// <summary>
    /// Demo class to show different ways of visualizing the space using scene understanding.
    /// </summary>
    public class DemoSceneUnderstandingController : DemoSpatialMeshHandler, IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>
    {
        #region Private Fields

        #region Serialized Fields

        [SerializeField]
        private string SavedSceneNamePrefix = "DemoSceneUnderstanding";
        [SerializeField]
        private bool InstantiatePrefabs = false;
        [SerializeField]
        private GameObject InstantiatedPrefab = null;
        [SerializeField]
        private Transform InstantiatedParent = null;


        #endregion Serialized Fields

        private IMixedRealitySceneUnderstandingObserver observer;

        private List<GameObject> instantiatedPrefabs;

        private Dictionary<SpatialAwarenessSurfaceTypes, Dictionary<int, SpatialAwarenessSceneObject>> observedSceneObjects;

        #endregion Private Fields
        
        #region MonoBehaviour Functions

        protected override void Start()
        {
            observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySceneUnderstandingObserver>();
            Debug.Log(observer.QueryRadius);

            if (observer == null)
            {
                Debug.LogError("Couldn't access Scene Understanding Observer! Please make sure the current build target is set to Universal Windows Platform. "
                    + "Visit https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/spatial-awareness/scene-understanding for more information.");
                return;
            }
            instantiatedPrefabs = new List<GameObject>();
            observedSceneObjects = new Dictionary<SpatialAwarenessSurfaceTypes, Dictionary<int, SpatialAwarenessSceneObject>>();
        }

        protected override void OnEnable()
        {
            RegisterEventHandlers<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>, SpatialAwarenessSceneObject>();
        }

        protected override void OnDisable()
        {
            UnregisterEventHandlers<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>, SpatialAwarenessSceneObject>();
        }

        protected override void OnDestroy()
        {
            UnregisterEventHandlers<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>, SpatialAwarenessSceneObject>();
        }

        #endregion MonoBehaviour Functions

        #region IMixedRealitySpatialAwarenessObservationHandler Implementations

        /// <inheritdoc />
        /// 
        public void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            // This method called everytime a SceneObject created by the SU observer
            // The eventData contains everything you need do something useful

            AddToData(eventData.Id);

            if (observedSceneObjects.TryGetValue(eventData.SpatialObject.SurfaceType, out Dictionary<int, SpatialAwarenessSceneObject> sceneObjectDict))
            {
                sceneObjectDict.Add(eventData.Id, eventData.SpatialObject);
            }
            else
            {
                observedSceneObjects.Add(eventData.SpatialObject.SurfaceType, new Dictionary<int, SpatialAwarenessSceneObject> { { eventData.Id, eventData.SpatialObject } });
            }
            Debug.Log(InstantiatePrefabs);
            Debug.Log(eventData.SpatialObject.Quads.Count);
            if (InstantiatePrefabs && eventData.SpatialObject.Quads.Count > 0)
            {
                var prefab = Instantiate(InstantiatedPrefab);
                prefab.transform.SetPositionAndRotation(eventData.SpatialObject.Position, eventData.SpatialObject.Rotation);
                float sx = eventData.SpatialObject.Quads[0].Extents.x;
                float sy = eventData.SpatialObject.Quads[0].Extents.y;
                prefab.transform.localScale = new Vector3(sx, sy, .1f);
                prefab.GetComponent<Renderer>().material.color = ColorForSurfaceType(eventData.SpatialObject.SurfaceType);
                if (InstantiatedParent)
                {
                    prefab.transform.SetParent(InstantiatedParent);
                }
                instantiatedPrefabs.Add(prefab);
            }
            else
            {
                foreach (var quad in eventData.SpatialObject.Quads)
                {
                    quad.GameObject.GetComponent<Renderer>().material.color = ColorForSurfaceType(eventData.SpatialObject.SurfaceType);

                }
            }
            foreach (var mesh in eventData.SpatialObject.Meshes)
            {
                mesh.GameObject.GetComponent<MeshRenderer>().enabled = false;  // disabling all world mesh renderers (removing the mesh from the users scene, keeping them in bg)
            }
        }

        /// <inheritdoc />
        public void OnObservationUpdated(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            UpdateData(eventData.Id);

            if (observedSceneObjects.TryGetValue(eventData.SpatialObject.SurfaceType, out Dictionary<int, SpatialAwarenessSceneObject> sceneObjectDict))
            {
                observedSceneObjects[eventData.SpatialObject.SurfaceType][eventData.Id] = eventData.SpatialObject;
            }
            else
            {
                observedSceneObjects.Add(eventData.SpatialObject.SurfaceType, new Dictionary<int, SpatialAwarenessSceneObject> { { eventData.Id, eventData.SpatialObject } });
            }
        }

        /// <inheritdoc />
        public void OnObservationRemoved(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            RemoveFromData(eventData.Id);

            foreach (var sceneObjectDict in observedSceneObjects.Values)
            {
                sceneObjectDict?.Remove(eventData.Id);
            }
        }

        #endregion IMixedRealitySpatialAwarenessObservationHandler Implementations

        #region Public Functions

        /// <summary>
        /// Get all currently observed SceneObjects of a certain type.
        /// </summary>
        /// <remarks>
        /// Before calling this function, the observer should be configured to observe the specified type by including that type in the SurfaceTypes property.
        /// </remarks>
        /// <returns>A dictionary with the scene objects of the requested type being the values and their ids being the keys.</returns>
        public IReadOnlyDictionary<int, SpatialAwarenessSceneObject> GetSceneObjectsOfType(SpatialAwarenessSurfaceTypes type)
        {
            if (!observer.SurfaceTypes.IsMaskSet(type))
            {
                Debug.LogErrorFormat("The Scene Objects of type {0} are not being observed. You should add {0} to the SurfaceTypes property of the observer in advance.", type);
            }

            if (observedSceneObjects.TryGetValue(type, out Dictionary<int, SpatialAwarenessSceneObject> sceneObjects))
            {
                return sceneObjects;
            }
            else
            {
                observedSceneObjects.Add(type, new Dictionary<int, SpatialAwarenessSceneObject>());
                return observedSceneObjects[type];
            }
        }

        #region UI Functions

        /// <summary>
        /// Request the observer to update the scene
        /// </summary>
        public void UpdateScene()
        {
            observer.UpdateOnDemand();
        }

        /// <summary>
        /// Request the observer to save the scene
        /// </summary>
        public void SaveScene()
        {
            observer.SaveScene(SavedSceneNamePrefix);
        }

        /// <summary>
        /// Request the observer to clear the observations in the scene
        /// </summary>
        public void ClearScene()
        {
            foreach (GameObject gameObject in instantiatedPrefabs)
            {
                Destroy(gameObject);
            }
            instantiatedPrefabs.Clear();
            observer.ClearObservations();
        }

        /// <summary>
        /// Change the auto update state of the observer
        /// </summary>
        public void ToggleAutoUpdate()
        {
            observer.AutoUpdate = !observer.AutoUpdate;
        }

        public void SetAutoUpdate(bool state)
        {
            observer.AutoUpdate = state;
        }


        public void ToggleInferRegions()
        {
            observer.InferRegions = !observer.InferRegions;
            ClearAndUpdateObserver();
        }

 


        #endregion UI Functions

        #endregion Public Functions

        #region Helper Functions


        /// <summary>
        /// Gets the color of the given surface type
        /// </summary>
        /// <param name="surfaceType">The surface type to get color for</param>
        /// <returns>The color of the type</returns>
        private Color ColorForSurfaceType(SpatialAwarenessSurfaceTypes surfaceType)
        {
            // shout-out to solarized!

            switch (surfaceType)
            {
                case SpatialAwarenessSurfaceTypes.Unknown:
                    return new Color32(220, 50, 47, 255); // red
                case SpatialAwarenessSurfaceTypes.Floor:
                    return new Color32(38, 139, 210, 255); // blue
                case SpatialAwarenessSurfaceTypes.Ceiling:
                    return new Color32(108, 113, 196, 255); // violet
                case SpatialAwarenessSurfaceTypes.Wall:
                    return new Color32(181, 137, 0, 255); // yellow
                case SpatialAwarenessSurfaceTypes.Platform:
                    return new Color32(133, 153, 0, 255); // green
                case SpatialAwarenessSurfaceTypes.Background:
                    return new Color32(203, 75, 22, 255); // orange
                case SpatialAwarenessSurfaceTypes.World:
                    return new Color32(211, 54, 130, 255); // magenta
                case SpatialAwarenessSurfaceTypes.Inferred:
                    return new Color32(42, 161, 152, 255); // cyan
                default:
                    return new Color32(220, 50, 47, 255); // red
            }
        }

        private void ClearAndUpdateObserver()
        {
            ClearScene();
            observer.UpdateOnDemand();
        }

        private void ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes surfaceType)
        {
            if (observer.SurfaceTypes.IsMaskSet(surfaceType))
            {
                observer.SurfaceTypes &= ~surfaceType;
            }
            else
            {
                observer.SurfaceTypes |= surfaceType;
            }
        }

        #endregion Helper Functions
    }
}