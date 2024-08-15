// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.MixedReality.SceneUnderstanding
{
    using System;
    using System.Runtime.InteropServices;

    internal delegate void apigen_promise_on_complete(IntPtr context, ref PackedValue value);

    internal delegate void apigen_promise_on_error(IntPtr context, ResultCode result);

    internal delegate void apigen_promise_on_progress(IntPtr context, float progress);

    [StructLayout(LayoutKind.Sequential)]
    internal struct PromiseCreateOptions
    {
        public IntPtr context;
        public apigen_promise_on_complete onComplete;
        public apigen_promise_on_error onError;
        public apigen_promise_on_progress onProgress;
    }
}
