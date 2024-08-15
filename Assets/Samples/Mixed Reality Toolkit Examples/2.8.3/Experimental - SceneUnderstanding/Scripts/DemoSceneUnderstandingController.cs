// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Examples.Demos;
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.SceneUnderstanding
{
    /// <summary>
    /// Demo class to show different ways of visualizing the space using scene understanding.
    /// </summary>

    public class DemoSceneUnderstandingController : DemoSpatialMeshHandler, IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>
    {
        #region Public Variables

        // by hasan sayour

        [Header("Layers")]
        [Tooltip("Layer for the Scene Understanding Wall objects")]
        public int LayerForWallObjects = 10;
        [Tooltip("Layer for the Scene Understanding Floor objects")]
        public int LayerForFloorObjects = 9;
        [Tooltip("Layer for the Scene Understanding Ceiling objects")]
        public int LayerForCeilingObjects = 7;
        [Tooltip("Layer for the Scene Understanding World objects")]
        public int LayerForWorldObjects = 12;

        [Tooltip("string")]
        public string  testt;

        [Tooltip("Display Quads")]
        public bool DisplayQuads = false;

        bool ss = true;

        #endregion

        #region Private Fields

        #region Serialized Fields

        [SerializeField]
        private string SavedSceneNamePrefix = "DemoSceneUnderstanding";

        #endregion Serialized Fields

        private IMixedRealitySceneUnderstandingObserver observer;

        private List<GameObject> instantiatedPrefabs;

        private Dictionary<SpatialAwarenessSurfaceTypes, Dictionary<int, SpatialAwarenessSceneObject>> observedSceneObjects;

        #endregion Private Fields

        #region MonoBehaviour Functions

        protected override void Start()
        {
            observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySceneUnderstandingObserver>();

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

            foreach (var quad in eventData.SpatialObject.Quads)
             {

                // by hasan sayour 


                //quad.GameObject.GetComponent<Renderer>().material.color = ColorForSurfaceType(eventData.SpatialObject.SurfaceType);
                //quad.GameObject.GetComponent<Renderer>().material.color = new Color32(38, 139, 210, 255); // blue
                quad.GameObject.layer = LayerForSurfaceType(eventData.SpatialObject.SurfaceType);  // giving every quad a layer based on its type
                quad.GameObject.GetComponent<Renderer>().material.color = ColorLayer(quad.GameObject.layer);  // coloring every quad based on its layer
                if (!DisplayQuads) 
                 {
                     quad.GameObject.GetComponent<MeshRenderer>().enabled = false;     // disabling all quads mesh renderers (removing them from the users scene, keeping them in bg)
                 }
                var quadn = quad.GameObject.transform.localScale;
                quad.GameObject.transform.localScale = new Vector3(quadn.x, quadn.y, 1);   // creating an invisible barrier blocking world mesh from enabled quads
                quad.GameObject.GetComponent<BoxCollider>().size = new Vector3(1,1,0.2f);  // creating an invisible barrier blocking world mesh from enabled quads
                if (ss)
                {
                    testt = quad.GameObject.transform.parent.name + ",   Pos : (" + quad.GameObject.transform.parent.position.x + ", " + quad.GameObject.transform.parent.position.y + ", " + quad.GameObject.transform.parent.position.z
                        + ")  ,   Rotation :" + quad.GameObject.transform.parent.rotation.eulerAngles + ",    scale : (" + quad.GameObject.transform.localScale.x + ", " + quad.GameObject.transform.localScale.y + ")";

                    Debug.Log(quad.GameObject.transform.parent.name + ",   Pos : (" + quad.GameObject.transform.parent.position.x + ", " + quad.GameObject.transform.parent.position.y + ", " + quad.GameObject.transform.parent.position.z
                        + ")  ,   Rotation :" + quad.GameObject.transform.parent.rotation.eulerAngles + ",    scale : (" + quad.GameObject.transform.localScale.x + ", " + quad.GameObject.transform.localScale.y + ")");
                    quad.GameObject.GetComponent<MeshRenderer>().enabled = true;
                    ss = false;
                }
            }
            foreach (var mesh in eventData.SpatialObject.Meshes)
            {
                // if (!mesh.GameObject.transform.parent.name.Contains("World"))             this if condition makes only world mesh visible 
                    mesh.GameObject.GetComponent<MeshRenderer>().enabled = false;  // disabling all world mesh renderers (removing the mesh from the users scene, keeping them in bg)
            }
        }
        private Color ColorLayer(int layer)   // by hasan sayour
        {
            // coloring quads based on its layer

            switch (layer)
            {
                case 7:
                    return new Color32(38, 139, 210, 255); // blue
                case 10:
                    return new Color32(133, 153, 0, 255); // green                                   
                case 9:
                    return new Color32(181, 137, 0, 255); // yellow
                default:
                    return new Color32(220, 50, 47, 255); // red
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

        public void ToggleDisplayQuads() // by hasan sayour
        {
            DisplayQuads = !DisplayQuads;
            ClearAndUpdateObserver();
        }
        public void ClearScene()
        {
            foreach (GameObject gameObject in instantiatedPrefabs)
            {
                Destroy(gameObject);
            }
            instantiatedPrefabs.Clear();
            observer.ClearObservations();
        }

        #endregion UI Functions

        #endregion Public Functions

        #region Helper Functions

       

        /// <summary>
        /// Gets the layer of the given surface type
        /// </summary>
        /// <param name="surfaceType">The surface type to get layer for</param>
        /// <returns>The layer of the type</returns>
        /// 
        private int LayerForSurfaceType(SpatialAwarenessSurfaceTypes surfaceType)    //by hasan sayour
        {
            switch (surfaceType)
            {
                case SpatialAwarenessSurfaceTypes.Floor:
                    return LayerForFloorObjects;
                case SpatialAwarenessSurfaceTypes.Ceiling:
                    return LayerForCeilingObjects;
                case SpatialAwarenessSurfaceTypes.Wall:
                    return LayerForWallObjects;
                case SpatialAwarenessSurfaceTypes.World:
                    return LayerForWorldObjects;
                default:
                    return 0;
                    
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
