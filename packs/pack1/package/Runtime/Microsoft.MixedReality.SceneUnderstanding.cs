#if UNITY_WSA
//
// Microsoft.MixedReality.SceneUnderstanding
// This file was auto-generated from AbiDefinition.cs.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.SceneUnderstanding
{
    internal enum status
    {
        /// <summary>
        /// Success
        /// </summary>
        OK = 0,
        /// <summary>
        /// Failed
        /// </summary>
        Failed = 1,
        /// <summary>
        /// Cannot access a disposed object.
        /// </summary>
        ObjectDisposed = 2,
        /// <summary>
        /// Out of memory.
        /// </summary>
        OutOfMemory = 12,
        /// <summary>
        /// Invalid argument.
        /// </summary>
        InvalidArgument = 22,
        /// <summary>
        /// The value is out of range.
        /// </summary>
        OutOfRange = 34,
        /// <summary>
        /// Not implemented.
        /// </summary>
        NotImplemented = 38,
        /// <summary>
        /// The key does not exist in the collection.
        /// </summary>
        KeyNotFound = 77,
    }

    internal static class NativeLibraryHelpers
    {
        internal static string[] IntPtrToStringArray(IntPtr result, int result_length)
        {
            // an empty list is returned as a single null character
            if (result_length == 1)
            {
                return Array.Empty<string>();
            }

            byte[] bytes = new byte[result_length - 1];
            System.Runtime.InteropServices.Marshal.Copy(result, bytes, 0, result_length - 1);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(result);
            return System.Text.Encoding.UTF8.GetString(bytes).Split('\0');
        }

        internal static void CheckStatus(status value)
        {
            if (value == status.OK)
            {
                return;
            }

            string fullMessage = string.Empty;

            switch (value)
            {
                case status.OK:
                    return;
                case status.Failed:
                    throw new InvalidOperationException(fullMessage);
                case status.ObjectDisposed:
                    throw new ObjectDisposedException(fullMessage);
                case status.OutOfMemory:
                    throw new OutOfMemoryException(fullMessage);
                case status.InvalidArgument:
                    throw new ArgumentException(fullMessage);
                case status.OutOfRange:
                    throw new ArgumentOutOfRangeException("", fullMessage);
                case status.NotImplemented:
                    throw new NotImplementedException(fullMessage);
                case status.KeyNotFound:
                    throw new KeyNotFoundException(fullMessage);
            }
        }

        internal static void CheckStatus(IntPtr handle, status value)
        {
            if (value == status.OK)
            {
                return;
            }

            string message;
            string requestCorrelationVector;
            string responseCorrelationVector;

            Microsoft.MixedReality.SceneUnderstanding.status code = Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_get_error_details(handle, out message, out requestCorrelationVector, out responseCorrelationVector);

            string fullMessage;
            if (code == status.Failed)
            {
                throw new InvalidOperationException("Unexpected error in exception handling.");
            }
            else if (code != status.OK)
            {
                fullMessage = "Exception thrown and an unexpected error in exception handling.";
            }
            else
            {
                fullMessage = message + ". Request CV: " + requestCorrelationVector + ". Response CV: " + responseCorrelationVector + ".";
            }

            switch (value)
            {
                case status.OK:
                    return;
                case status.Failed:
                    throw new InvalidOperationException(fullMessage);
                case status.ObjectDisposed:
                    throw new ObjectDisposedException(fullMessage);
                case status.OutOfMemory:
                    throw new OutOfMemoryException(fullMessage);
                case status.InvalidArgument:
                    throw new ArgumentException(fullMessage);
                case status.OutOfRange:
                    throw new ArgumentOutOfRangeException("", fullMessage);
                case status.NotImplemented:
                    throw new NotImplementedException(fullMessage);
                case status.KeyNotFound:
                    throw new KeyNotFoundException(fullMessage);
            }
        }
    }

    /// <summary>
    /// This interface is implemented by classes with events to help track callbacks.
    /// </summary>
    internal interface ICookie
    {
        /// <summary>
        /// Unique cookie value for callback identification.
        /// </summary>
        ulong Cookie { get; set; }
    }

    internal static class CookieTracker<T>
        where T : class, ICookie
    {
        private static ulong lastCookie;

        private static Dictionary<ulong, System.WeakReference<T>> tracked = new Dictionary<ulong, System.WeakReference<T>>();

        internal static void Add(T instance)
        {
            lock (tracked)
            {
                instance.Cookie = ++lastCookie;
                tracked[instance.Cookie] = new System.WeakReference<T>(instance);
            }
        }

        internal static T Lookup(ulong cookie)
        {
            T result;
            System.WeakReference<T> reference;
            bool found;

            lock (tracked)
            {
                found = tracked.TryGetValue(cookie, out reference);
            }

            if (!found)
            {
                return null;
            }

            found = reference.TryGetTarget(out result);

            if (!found)
            {
                lock (tracked)
                {
                    tracked.Remove(cookie);
                }
            }

            return result;
        }

        internal static void Remove(T instance)
        {
            lock (tracked)
            {
                tracked.Remove(instance.Cookie);
            }
        }
    }

    internal static partial class NativeLibrary
    {
        internal const string DllName = "Microsoft.MixedReality.SceneUnderstanding.dll";
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_quad_get_extents(IntPtr handle, out System.Numerics.Vector2 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_quad_get_alignment(IntPtr handle, out Microsoft.MixedReality.SceneUnderstanding.SceneQuadAlignment result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_quad_get_surface_mask(IntPtr handle, ushort mask_width, ushort mask_height, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4), In, Out] byte[] surface_mask, int surface_mask_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_quad_find_centermost_placement(IntPtr handle, System.Numerics.Vector2 object_extents, out System.Numerics.Vector2 placement_center, [MarshalAs(UnmanagedType.U1)] out bool result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_observer_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_observer_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_observer_is_supported([MarshalAs(UnmanagedType.U1)] out bool result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_observer_request_access_async(out Microsoft.MixedReality.SceneUnderstanding.SceneObserverAccessStatus result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_observer_compute_async_scene_query_settings_settings_single_query_radius(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi settings, float query_radius, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_deserialize_serialized_scene([MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] byte[] serialized_scene, int serialized_scene_count, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_deserialize_serialized_scene_scene_previous_scene([MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] byte[] serialized_scene, int serialized_scene_count, IntPtr previous_scene, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_from_fragments_scene_fragments([MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] IntPtr[] scene_fragments, int scene_fragments_count, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_fragment_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_fragment_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_fragment_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_fragment_deserialize([MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] byte[] serialized_scene_fragment, int serialized_scene_fragment_count, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_fragment_dispose(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_from_fragments_scene_fragments_scene_previous_scene([MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] IntPtr[] scene_fragments, int scene_fragments_count, IntPtr previous_scene, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_get_scene_objects(IntPtr handle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] out IntPtr[] result, out int result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_object_get_kind(IntPtr handle, out Microsoft.MixedReality.SceneUnderstanding.SceneObjectKind result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_object_get_quads(IntPtr handle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] out IntPtr[] result, out int result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_object_get_meshes(IntPtr handle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] out IntPtr[] result, out int result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_mesh_get_triangle_index_count(IntPtr handle, out uint result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_mesh_get_triangle_indices(IntPtr handle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2), In, Out] uint[] index_buffer, int index_buffer_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_mesh_get_vertex_count(IntPtr handle, out uint result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_mesh_get_vertex_positions(IntPtr handle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2), In, Out] System.Numerics.Vector3[] vertices, int vertices_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_object_get_collider_meshes(IntPtr handle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] out IntPtr[] result, out int result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_object_get_position(IntPtr handle, out System.Numerics.Vector3 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_object_get_orientation(IntPtr handle, out System.Numerics.Quaternion result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_object_get_location_as_matrix(IntPtr handle, out System.Numerics.Matrix4x4 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_find_component(IntPtr handle, System.Guid component_id, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_component_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_component_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_component_get_id(IntPtr handle, out System.Guid result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_dispose(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_get_origin_spatial_graph_node_id(IntPtr handle, out System.Guid result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_get_origin(IntPtr handle, out Microsoft.MixedReality.SceneUnderstanding.SpatialGraphCoordinateSystem result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_observer_compute_async_scene_query_settings_settings_single_query_radius_scene_previous_scene(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi settings, float query_radius, IntPtr previous_scene, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_observer_compute_serialized_async(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi settings, float query_radius, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_buffer_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_buffer_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_buffer_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_buffer_get_size(IntPtr handle, out uint result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_buffer_dispose(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_buffer_get_data(IntPtr handle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2), In, Out] byte[] data, int data_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_observer_compute_with_bounds_async_scene_query_settings_settings_bounds(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi settings, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] IntPtr[] bounds, int bounds_count, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_bounds_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_bounds_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_bounds_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_bounds_from_field_of_view(Microsoft.MixedReality.SceneUnderstanding.SpatialGraphCoordinateSystem coordinate_system, Microsoft.MixedReality.SceneUnderstanding.SpatialFieldOfView field_of_view, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_bounds_from_oriented_box(Microsoft.MixedReality.SceneUnderstanding.SpatialGraphCoordinateSystem coordinate_system, Microsoft.MixedReality.SceneUnderstanding.SpatialOrientedBox box, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_bounds_from_sphere(Microsoft.MixedReality.SceneUnderstanding.SpatialGraphCoordinateSystem coordinate_system, Microsoft.MixedReality.SceneUnderstanding.SpatialSphere sphere, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_observer_compute_with_bounds_async_scene_query_settings_settings_bounds_scene_previous_scene(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi settings, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] IntPtr[] bounds, int bounds_count, IntPtr previous_scene, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_scene_observer_compute_with_bounds_serialized_async(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi settings, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] IntPtr[] bounds, int bounds_count, out IntPtr result);
        [DllImport(DllName, EntryPoint="su_get_error_details_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_get_error_details(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result_message, [MarshalAs(UnmanagedType.LPWStr)] out string result_requestCorrelationVector, [MarshalAs(UnmanagedType.LPWStr)] out string result_responseCorrelationVector);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern Microsoft.MixedReality.SceneUnderstanding.status su_get_handle_type(IntPtr handle, out Microsoft.MixedReality.SceneUnderstanding.HandleType result);
    }

    // CODE STARTS HERE

    abstract class BasePrivateDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        protected abstract int InternalGetCount();
        protected abstract TKey InternalGetKey(int index);
        protected abstract TValue InternalGetItem(TKey key);
        protected abstract void InternalSetItem(TKey key, TValue value);
        protected abstract void InternalRemoveKey(TKey key);

        public TValue this[TKey key] { get { return InternalGetItem(key); } set { InternalSetItem(key, value); } }

        public ICollection<TKey> Keys { get { return Enumerable.Range(0, InternalGetCount()).Select(n => InternalGetKey(n)).ToList().AsReadOnly(); } }

        public ICollection<TValue> Values { get { return Enumerable.Range(0, InternalGetCount()).Select(n => InternalGetKey(n)).Select(k => InternalGetItem(k)).ToList().AsReadOnly(); } }

        public int Count { get { return InternalGetCount(); } }

        public bool IsReadOnly { get { return false; } }

        public void Add(TKey key, TValue value)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                InternalSetItem(key, value);
                return;
            }
            throw new ArgumentException();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            while (InternalGetCount() > 0)
            {
                TKey key = InternalGetKey(0);
                InternalRemoveKey(key);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            try
            {
                TValue value = InternalGetItem(item.Key);
                if (Comparer<TValue>.Default.Compare(value, item.Value) == 0)
                {
                    return true;
                }
                return false;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public bool ContainsKey(TKey key)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            return true;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                yield return new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }

        public bool Remove(TKey key)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            InternalRemoveKey(key);
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                value = InternalGetItem(key);
                return true;
            }
            catch (KeyNotFoundException)
            {
                value = default(TValue);
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                yield return new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }
    }

    abstract class BasePrivateList<T> : IList<T>
    {
        protected abstract int InternalGetCount();
        protected abstract T InternalGetItem(int index);
        protected abstract void InternalSetItem(int index, T value);
        protected abstract void InternalRemoveItem(int index);

        public int Count { get { return InternalGetCount(); } }

        public bool IsReadOnly { get { return false; } }

        public T this[int index] { get { return InternalGetItem(index); } set { InternalSetItem(index, value); } }

        public int IndexOf(T item)
        {
            int count = InternalGetCount();
            for (int i = 0; i < count; i++)
            {
                if (Comparer<T>.Default.Compare(item, InternalGetItem(i)) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            InternalSetItem(index, item);
        }

        public void RemoveAt(int index)
        {
            InternalRemoveItem(index);
        }

        public void Add(T item)
        {
            InternalSetItem(InternalGetCount(), item);
        }

        public void Clear()
        {
            while (InternalGetCount() > 0)
            {
                InternalRemoveItem(0);
            }
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; ++i)
            {
                array[i + arrayIndex] = this[i];
            }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0) return false;
            InternalRemoveItem(index);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }
    }

    static class Factories
    {
        internal static Microsoft.MixedReality.SceneUnderstanding.SceneComponent SceneComponentFactory(IntPtr handle, bool transfer)
        {
            if (handle == IntPtr.Zero)
            {
                return null;
            }

            HandleType result;
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_get_handle_type(handle, out result));
            Microsoft.MixedReality.SceneUnderstanding.SceneComponent result_object;
            switch (result)
            {
                case HandleType.SceneQuad:
                    result_object = new Microsoft.MixedReality.SceneUnderstanding.SceneQuad(handle, transfer);
                    break;
                case HandleType.SceneObject:
                    result_object = new Microsoft.MixedReality.SceneUnderstanding.SceneObject(handle, transfer);
                    break;
                case HandleType.SceneMesh:
                    result_object = new Microsoft.MixedReality.SceneUnderstanding.SceneMesh(handle, transfer);
                    break;
                default:
                    throw new ArgumentException();
            }
            return result_object;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SceneQuerySettingsAbi
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool EnableSceneObjectQuads;

        [MarshalAs(UnmanagedType.U1)]
        public bool EnableSceneObjectMeshes;

        [MarshalAs(UnmanagedType.U1)]
        public bool EnableOnlyObservedSceneObjects;

        [MarshalAs(UnmanagedType.U1)]
        public bool EnableWorldMesh;

        public Microsoft.MixedReality.SceneUnderstanding.SceneMeshLevelOfDetail RequestedMeshLevelOfDetail;

        public Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettings Convert()
        {
            return Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi.Convert(this);
        }

        public static Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettings Convert(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi value)
        {
            Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettings result = new Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettings();

            result.EnableSceneObjectQuads = value.EnableSceneObjectQuads;
            result.EnableSceneObjectMeshes = value.EnableSceneObjectMeshes;
            result.EnableOnlyObservedSceneObjects = value.EnableOnlyObservedSceneObjects;
            result.EnableWorldMesh = value.EnableWorldMesh;
            result.RequestedMeshLevelOfDetail = value.RequestedMeshLevelOfDetail;

            return result;
        }
    }

    public partial struct SceneQuerySettings
    {
        public bool EnableSceneObjectQuads;

        public bool EnableSceneObjectMeshes;

        public bool EnableOnlyObservedSceneObjects;

        public bool EnableWorldMesh;

        public Microsoft.MixedReality.SceneUnderstanding.SceneMeshLevelOfDetail RequestedMeshLevelOfDetail;

        internal Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi Convert()
        {
            return Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettings.Convert(this);
        }

        internal static Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi Convert(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettings value)
        {
            Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi result = new Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi();

            result.EnableSceneObjectQuads = value.EnableSceneObjectQuads;
            result.EnableSceneObjectMeshes = value.EnableSceneObjectMeshes;
            result.EnableOnlyObservedSceneObjects = value.EnableOnlyObservedSceneObjects;
            result.EnableWorldMesh = value.EnableWorldMesh;
            result.RequestedMeshLevelOfDetail = value.RequestedMeshLevelOfDetail;

            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct SpatialGraphCoordinateSystem
    {
        public System.Guid NodeId;

        public System.Numerics.Matrix4x4 CoordinateSystemToNodeTransform;

    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct SpatialFieldOfView
    {
        public System.Numerics.Vector3 Position;

        public System.Numerics.Quaternion Orientation;

        public float FarDistance;

        public float HorizontalFieldOfViewInDegrees;

        public float AspectRatio;

    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct SpatialOrientedBox
    {
        public System.Numerics.Vector3 Center;

        public System.Numerics.Vector3 Extents;

        public System.Numerics.Quaternion Orientation;

    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct SpatialSphere
    {
        public System.Numerics.Vector3 Center;

        public float Radius;

    }

    public enum SceneQuadAlignment : int
    {
        NonOrthogonal = 0,
        Horizontal = 1,
        Vertical = 2,
    }

    public enum SceneRegionSurfaceKind : int
    {
        NotSurface = 0,
        SurfaceObserved = 1,
        SurfaceInferred = 2,
    }

    public enum SceneMeshLevelOfDetail : int
    {
        Coarse = 0,
        Medium = 1,
        Fine = 2,
        Unlimited = 255,
    }

    public enum SceneObserverAccessStatus : int
    {
        Allowed = 0,
        DeniedBySystem = 1,
        DeniedByUser = 2,
        NotDeclaredByApp = 3,
        UserPromptRequired = 4,
    }

    public enum SceneObjectKind : int
    {
        Background = 0,
        Wall = 1,
        Floor = 2,
        Ceiling = 3,
        Platform = 4,
        Unknown = 247,
        World = 248,
        CompletelyInferred = 249,
    }

    public enum HandleType : int
    {
        Unknown = 0,
        SceneQuad = 1,
        SceneObject = 2,
        SceneMesh = 3,
    }

    public partial class SceneBounds
    {
        internal IntPtr handle;
        internal SceneBounds(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_bounds_addref(ahandle);
        }
        public SceneBounds()
        {
            Microsoft.MixedReality.SceneUnderstanding.status resultStatus = (Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_bounds_create(out this.handle));
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        ~SceneBounds()
        {
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_bounds_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        public static Microsoft.MixedReality.SceneUnderstanding.SceneBounds FromFieldOfView(Microsoft.MixedReality.SceneUnderstanding.SpatialGraphCoordinateSystem coordinateSystem, Microsoft.MixedReality.SceneUnderstanding.SpatialFieldOfView fieldOfView)
        {
            IntPtr result_handle;
            Microsoft.MixedReality.SceneUnderstanding.SceneBounds result_object;
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_bounds_from_field_of_view(coordinateSystem, fieldOfView, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.MixedReality.SceneUnderstanding.SceneBounds(result_handle, transfer:true) : null;
            return result_object;
        }

        public static Microsoft.MixedReality.SceneUnderstanding.SceneBounds FromOrientedBox(Microsoft.MixedReality.SceneUnderstanding.SpatialGraphCoordinateSystem coordinateSystem, Microsoft.MixedReality.SceneUnderstanding.SpatialOrientedBox box)
        {
            IntPtr result_handle;
            Microsoft.MixedReality.SceneUnderstanding.SceneBounds result_object;
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_bounds_from_oriented_box(coordinateSystem, box, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.MixedReality.SceneUnderstanding.SceneBounds(result_handle, transfer:true) : null;
            return result_object;
        }

        public static Microsoft.MixedReality.SceneUnderstanding.SceneBounds FromSphere(Microsoft.MixedReality.SceneUnderstanding.SpatialGraphCoordinateSystem coordinateSystem, Microsoft.MixedReality.SceneUnderstanding.SpatialSphere sphere)
        {
            IntPtr result_handle;
            Microsoft.MixedReality.SceneUnderstanding.SceneBounds result_object;
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_bounds_from_sphere(coordinateSystem, sphere, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.MixedReality.SceneUnderstanding.SceneBounds(result_handle, transfer:true) : null;
            return result_object;
        }

    }

    public partial class SceneBuffer : IDisposable
    {
        internal IntPtr handle;
        internal SceneBuffer(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_buffer_addref(ahandle);
        }
        public SceneBuffer()
        {
            Microsoft.MixedReality.SceneUnderstanding.status resultStatus = (Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_buffer_create(out this.handle));
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SceneBuffer"/> class.
        /// </summary>
        ~SceneBuffer()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.su_scene_buffer_release(this.handle));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.su_scene_buffer_dispose (this.handle));
        }

        public uint Size
        {
            get
            {
                uint result;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_buffer_get_size(this.handle, out result));
                return result;
            }
        }

        public void GetData(byte[] data)
        {
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_buffer_get_data(this.handle, data, data?.Length ?? 0));
        }

    }

    public abstract partial class SceneComponent
    {
        internal IntPtr handle;
        internal SceneComponent(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_component_addref(ahandle);
        }
        ~SceneComponent()
        {
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_component_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        public System.Guid Id
        {
            get
            {
                System.Guid result;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_component_get_id(this.handle, out result));
                return result;
            }
        }

    }

    public partial class SceneFragment : IDisposable
    {
        internal IntPtr handle;
        internal SceneFragment(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_fragment_addref(ahandle);
        }
        public SceneFragment()
        {
            Microsoft.MixedReality.SceneUnderstanding.status resultStatus = (Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_fragment_create(out this.handle));
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SceneFragment"/> class.
        /// </summary>
        ~SceneFragment()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.su_scene_fragment_release(this.handle));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.su_scene_fragment_dispose (this.handle));
        }

        public static Microsoft.MixedReality.SceneUnderstanding.SceneFragment Deserialize(byte[] serializedSceneFragment)
        {
            IntPtr result_handle;
            Microsoft.MixedReality.SceneUnderstanding.SceneFragment result_object;
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_fragment_deserialize(serializedSceneFragment, serializedSceneFragment?.Length ?? 0, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.MixedReality.SceneUnderstanding.SceneFragment(result_handle, transfer:true) : null;
            return result_object;
        }

    }

    public partial class Scene : IDisposable
    {
        internal IntPtr handle;
        internal Scene(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_addref(ahandle);
        }
        public Scene()
        {
            Microsoft.MixedReality.SceneUnderstanding.status resultStatus = (Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_create(out this.handle));
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Scene"/> class.
        /// </summary>
        ~Scene()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.su_scene_release(this.handle));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.su_scene_dispose (this.handle));
        }

        public System.Collections.Generic.IReadOnlyList<Microsoft.MixedReality.SceneUnderstanding.SceneObject> SceneObjects
        {
            get
            {
                System.Collections.Generic.IReadOnlyList<Microsoft.MixedReality.SceneUnderstanding.SceneObject> result;
                IntPtr[] result_array;
                int result_length;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_get_scene_objects(this.handle, out result_array, out result_length));
                result = (result_array ?? Enumerable.Empty<IntPtr>()).Select(handle => new Microsoft.MixedReality.SceneUnderstanding.SceneObject(handle, transfer:true)).ToArray();
                return result;
            }
        }

        public System.Guid OriginSpatialGraphNodeId
        {
            get
            {
                System.Guid result;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_get_origin_spatial_graph_node_id(this.handle, out result));
                return result;
            }
        }

        public Microsoft.MixedReality.SceneUnderstanding.SpatialGraphCoordinateSystem Origin
        {
            get
            {
                Microsoft.MixedReality.SceneUnderstanding.SpatialGraphCoordinateSystem result;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_get_origin(this.handle, out result));
                return result;
            }
        }

        public static Microsoft.MixedReality.SceneUnderstanding.Scene Deserialize(byte[] serializedScene)
        {
            IntPtr result_handle;
            Microsoft.MixedReality.SceneUnderstanding.Scene result_object;
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_deserialize_serialized_scene(serializedScene, serializedScene?.Length ?? 0, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.MixedReality.SceneUnderstanding.Scene(result_handle, transfer:true) : null;
            return result_object;
        }

        public static Microsoft.MixedReality.SceneUnderstanding.Scene Deserialize(byte[] serializedScene, Microsoft.MixedReality.SceneUnderstanding.Scene previousScene)
        {
            IntPtr result_handle;
            Microsoft.MixedReality.SceneUnderstanding.Scene result_object;
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_deserialize_serialized_scene_scene_previous_scene(serializedScene, serializedScene?.Length ?? 0, previousScene != null ? previousScene.handle : IntPtr.Zero, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.MixedReality.SceneUnderstanding.Scene(result_handle, transfer:true) : null;
            return result_object;
        }

        public static Microsoft.MixedReality.SceneUnderstanding.Scene FromFragments(Microsoft.MixedReality.SceneUnderstanding.SceneFragment[] sceneFragments)
        {
            IntPtr[] sceneFragments_array = sceneFragments?.Select(x => x?.handle ?? IntPtr.Zero).ToArray() ?? Array.Empty<IntPtr>();
            IntPtr result_handle;
            Microsoft.MixedReality.SceneUnderstanding.Scene result_object;
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_from_fragments_scene_fragments(sceneFragments_array, sceneFragments?.Length ?? 0, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.MixedReality.SceneUnderstanding.Scene(result_handle, transfer:true) : null;
            return result_object;
        }

        public static Microsoft.MixedReality.SceneUnderstanding.Scene FromFragments(Microsoft.MixedReality.SceneUnderstanding.SceneFragment[] sceneFragments, Microsoft.MixedReality.SceneUnderstanding.Scene previousScene)
        {
            IntPtr[] sceneFragments_array = sceneFragments?.Select(x => x?.handle ?? IntPtr.Zero).ToArray() ?? Array.Empty<IntPtr>();
            IntPtr result_handle;
            Microsoft.MixedReality.SceneUnderstanding.Scene result_object;
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_from_fragments_scene_fragments_scene_previous_scene(sceneFragments_array, sceneFragments?.Length ?? 0, previousScene != null ? previousScene.handle : IntPtr.Zero, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.MixedReality.SceneUnderstanding.Scene(result_handle, transfer:true) : null;
            return result_object;
        }

        public Microsoft.MixedReality.SceneUnderstanding.SceneComponent FindComponent(System.Guid componentId)
        {
            IntPtr result_handle;
            Microsoft.MixedReality.SceneUnderstanding.SceneComponent result_object;
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_find_component(this.handle, componentId, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? Factories.SceneComponentFactory(result_handle, transfer:true) : null;
            return result_object;
        }

    }

    public partial class SceneMesh : SceneComponent
    {
        internal SceneMesh(IntPtr ahandle, bool transfer) : base(ahandle, transfer)
        {
        }
        public uint TriangleIndexCount
        {
            get
            {
                uint result;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_mesh_get_triangle_index_count(this.handle, out result));
                return result;
            }
        }

        public uint VertexCount
        {
            get
            {
                uint result;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_mesh_get_vertex_count(this.handle, out result));
                return result;
            }
        }

        public void GetTriangleIndices(uint[] indexBuffer)
        {
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_mesh_get_triangle_indices(this.handle, indexBuffer, indexBuffer?.Length ?? 0));
        }

        public void GetVertexPositions(System.Numerics.Vector3[] vertices)
        {
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_mesh_get_vertex_positions(this.handle, vertices, vertices?.Length ?? 0));
        }

    }

    public partial class SceneObject : SceneComponent
    {
        internal SceneObject(IntPtr ahandle, bool transfer) : base(ahandle, transfer)
        {
        }
        public Microsoft.MixedReality.SceneUnderstanding.SceneObjectKind Kind
        {
            get
            {
                Microsoft.MixedReality.SceneUnderstanding.SceneObjectKind result;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_object_get_kind(this.handle, out result));
                return result;
            }
        }

        public System.Collections.Generic.IReadOnlyList<Microsoft.MixedReality.SceneUnderstanding.SceneQuad> Quads
        {
            get
            {
                System.Collections.Generic.IReadOnlyList<Microsoft.MixedReality.SceneUnderstanding.SceneQuad> result;
                IntPtr[] result_array;
                int result_length;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_object_get_quads(this.handle, out result_array, out result_length));
                result = (result_array ?? Enumerable.Empty<IntPtr>()).Select(handle => new Microsoft.MixedReality.SceneUnderstanding.SceneQuad(handle, transfer:true)).ToArray();
                return result;
            }
        }

        public System.Collections.Generic.IReadOnlyList<Microsoft.MixedReality.SceneUnderstanding.SceneMesh> Meshes
        {
            get
            {
                System.Collections.Generic.IReadOnlyList<Microsoft.MixedReality.SceneUnderstanding.SceneMesh> result;
                IntPtr[] result_array;
                int result_length;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_object_get_meshes(this.handle, out result_array, out result_length));
                result = (result_array ?? Enumerable.Empty<IntPtr>()).Select(handle => new Microsoft.MixedReality.SceneUnderstanding.SceneMesh(handle, transfer:true)).ToArray();
                return result;
            }
        }

        public System.Collections.Generic.IReadOnlyList<Microsoft.MixedReality.SceneUnderstanding.SceneMesh> ColliderMeshes
        {
            get
            {
                System.Collections.Generic.IReadOnlyList<Microsoft.MixedReality.SceneUnderstanding.SceneMesh> result;
                IntPtr[] result_array;
                int result_length;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_object_get_collider_meshes(this.handle, out result_array, out result_length));
                result = (result_array ?? Enumerable.Empty<IntPtr>()).Select(handle => new Microsoft.MixedReality.SceneUnderstanding.SceneMesh(handle, transfer:true)).ToArray();
                return result;
            }
        }

        public System.Numerics.Vector3 Position
        {
            get
            {
                System.Numerics.Vector3 result;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_object_get_position(this.handle, out result));
                return result;
            }
        }

        public System.Numerics.Quaternion Orientation
        {
            get
            {
                System.Numerics.Quaternion result;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_object_get_orientation(this.handle, out result));
                return result;
            }
        }

        public System.Numerics.Matrix4x4 GetLocationAsMatrix()
        {
            System.Numerics.Matrix4x4 result;
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_object_get_location_as_matrix(this.handle, out result));
            return result;
        }

    }

    public partial class SceneObserver
    {
        internal IntPtr handle;
        internal SceneObserver(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_observer_addref(ahandle);
        }
        ~SceneObserver()
        {
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_observer_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        public static bool IsSupported()
        {
            bool result;
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_observer_is_supported(out result));
            return result;
        }

        public static async System.Threading.Tasks.Task<Microsoft.MixedReality.SceneUnderstanding.SceneObserverAccessStatus> RequestAccessAsync()
        {
            return await Task.Run(() =>
            {
                Microsoft.MixedReality.SceneUnderstanding.SceneObserverAccessStatus result;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_observer_request_access_async(out result));
                return result;
            });
        }

        public static async System.Threading.Tasks.Task<Microsoft.MixedReality.SceneUnderstanding.Scene> ComputeAsync(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettings settings, float queryRadius)
        {
            return await Task.Run(() =>
            {
                Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi settings_converted = settings.Convert();
                IntPtr result_handle;
                Microsoft.MixedReality.SceneUnderstanding.Scene result_object;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_observer_compute_async_scene_query_settings_settings_single_query_radius(settings_converted, queryRadius, out result_handle));
                result_object = new Microsoft.MixedReality.SceneUnderstanding.Scene(result_handle, transfer:true);
                return result_object;
            });
        }

        public static async System.Threading.Tasks.Task<Microsoft.MixedReality.SceneUnderstanding.Scene> ComputeAsync(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettings settings, float queryRadius, Microsoft.MixedReality.SceneUnderstanding.Scene previousScene)
        {
            return await Task.Run(() =>
            {
                Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi settings_converted = settings.Convert();
                IntPtr result_handle;
                Microsoft.MixedReality.SceneUnderstanding.Scene result_object;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_observer_compute_async_scene_query_settings_settings_single_query_radius_scene_previous_scene(settings_converted, queryRadius, previousScene != null ? previousScene.handle : IntPtr.Zero, out result_handle));
                result_object = new Microsoft.MixedReality.SceneUnderstanding.Scene(result_handle, transfer:true);
                return result_object;
            });
        }

        public static async System.Threading.Tasks.Task<Microsoft.MixedReality.SceneUnderstanding.SceneBuffer> ComputeSerializedAsync(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettings settings, float queryRadius)
        {
            return await Task.Run(() =>
            {
                Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi settings_converted = settings.Convert();
                IntPtr result_handle;
                Microsoft.MixedReality.SceneUnderstanding.SceneBuffer result_object;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_observer_compute_serialized_async(settings_converted, queryRadius, out result_handle));
                result_object = new Microsoft.MixedReality.SceneUnderstanding.SceneBuffer(result_handle, transfer:true);
                return result_object;
            });
        }

        public static async System.Threading.Tasks.Task<Microsoft.MixedReality.SceneUnderstanding.Scene> ComputeWithBoundsAsync(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettings settings, Microsoft.MixedReality.SceneUnderstanding.SceneBounds[] bounds)
        {
            return await Task.Run(() =>
            {
                Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi settings_converted = settings.Convert();
                IntPtr[] bounds_array = bounds?.Select(x => x?.handle ?? IntPtr.Zero).ToArray() ?? Array.Empty<IntPtr>();
                IntPtr result_handle;
                Microsoft.MixedReality.SceneUnderstanding.Scene result_object;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_observer_compute_with_bounds_async_scene_query_settings_settings_bounds(settings_converted, bounds_array, bounds?.Length ?? 0, out result_handle));
                result_object = new Microsoft.MixedReality.SceneUnderstanding.Scene(result_handle, transfer:true);
                return result_object;
            });
        }

        public static async System.Threading.Tasks.Task<Microsoft.MixedReality.SceneUnderstanding.Scene> ComputeWithBoundsAsync(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettings settings, Microsoft.MixedReality.SceneUnderstanding.SceneBounds[] bounds, Microsoft.MixedReality.SceneUnderstanding.Scene previousScene)
        {
            return await Task.Run(() =>
            {
                Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi settings_converted = settings.Convert();
                IntPtr[] bounds_array = bounds?.Select(x => x?.handle ?? IntPtr.Zero).ToArray() ?? Array.Empty<IntPtr>();
                IntPtr result_handle;
                Microsoft.MixedReality.SceneUnderstanding.Scene result_object;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_observer_compute_with_bounds_async_scene_query_settings_settings_bounds_scene_previous_scene(settings_converted, bounds_array, bounds?.Length ?? 0, previousScene != null ? previousScene.handle : IntPtr.Zero, out result_handle));
                result_object = new Microsoft.MixedReality.SceneUnderstanding.Scene(result_handle, transfer:true);
                return result_object;
            });
        }

        public static async System.Threading.Tasks.Task<Microsoft.MixedReality.SceneUnderstanding.SceneBuffer> ComputeWithBoundsSerializedAsync(Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettings settings, Microsoft.MixedReality.SceneUnderstanding.SceneBounds[] bounds)
        {
            return await Task.Run(() =>
            {
                Microsoft.MixedReality.SceneUnderstanding.SceneQuerySettingsAbi settings_converted = settings.Convert();
                IntPtr[] bounds_array = bounds?.Select(x => x?.handle ?? IntPtr.Zero).ToArray() ?? Array.Empty<IntPtr>();
                IntPtr result_handle;
                Microsoft.MixedReality.SceneUnderstanding.SceneBuffer result_object;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_observer_compute_with_bounds_serialized_async(settings_converted, bounds_array, bounds?.Length ?? 0, out result_handle));
                result_object = new Microsoft.MixedReality.SceneUnderstanding.SceneBuffer(result_handle, transfer:true);
                return result_object;
            });
        }

    }

    public partial class SceneQuad : SceneComponent
    {
        internal SceneQuad(IntPtr ahandle, bool transfer) : base(ahandle, transfer)
        {
        }
        public System.Numerics.Vector2 Extents
        {
            get
            {
                System.Numerics.Vector2 result;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_quad_get_extents(this.handle, out result));
                return result;
            }
        }

        public Microsoft.MixedReality.SceneUnderstanding.SceneQuadAlignment Alignment
        {
            get
            {
                Microsoft.MixedReality.SceneUnderstanding.SceneQuadAlignment result;
                Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_quad_get_alignment(this.handle, out result));
                return result;
            }
        }

        public void GetSurfaceMask(ushort maskWidth, ushort maskHeight, byte[] surfaceMask)
        {
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_quad_get_surface_mask(this.handle, maskWidth, maskHeight, surfaceMask, surfaceMask?.Length ?? 0));
        }

        public bool FindCentermostPlacement(System.Numerics.Vector2 objectExtents, out System.Numerics.Vector2 placementCenter)
        {
            bool result;
            Microsoft.MixedReality.SceneUnderstanding.NativeLibraryHelpers.CheckStatus(this.handle, Microsoft.MixedReality.SceneUnderstanding.NativeLibrary.su_scene_quad_find_centermost_placement(this.handle, objectExtents, out placementCenter, out result));
            return result;
        }

    }

}

#endif
