// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.MixedReality.SceneUnderstanding
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    internal class PromiseBase : IDisposable
    {
        private bool disposed = false;

#if DEBUG
        private System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
#endif

        private readonly CancellationTokenRegistration cancellationTokenRegistration;

        private readonly PromiseCreateOptions createOptions;

        public readonly PromiseSafeHandle Handle;

        protected PromiseBase(PromiseCreateOptions createOptions, CancellationToken cancellationToken)
        {
            PromiseSafeHandle promiseSafeHandle;

            int result = NativeLibrary.ApiGenPromiseCreate(ref createOptions, out promiseSafeHandle);
            if (result != 0)
            {
                throw new InvalidOperationException();
            }

            this.Handle = promiseSafeHandle;
            this.createOptions = createOptions;
            this.cancellationTokenRegistration = cancellationToken.Register(() => NativeLibrary.ApiGenPromiseCancel(promiseSafeHandle));
        }

        ~PromiseBase()
        {
#if DEBUG
            Debug.Assert(false, $"A {nameof(Promise)} should never be finalized. Ensure all instances are disposed. Allocation at: {this.stackTrace}");
#else
            Debug.Assert(false, $"A {nameof(Promise)} should never be finalized. Ensure all instances are disposed.");
#endif
            this.Dispose(disposing: false);
        }

        /// <summary>
        /// An implementation of <see cref="IDisposable.Dispose"/> that cleans up the resources owned by this object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// An implementation of Dispose using the Dispose pattern.
        /// </summary>
        /// <remarks>
        /// Order is important here - the <see cref="CancellationTokenRegistration"/> instance must be disposed before the <see cref="PromiseSafeHandle"/>.
        /// </remarks>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                ((IDisposable)this.cancellationTokenRegistration).Dispose();
                ((IDisposable)this.Handle).Dispose();
                this.disposed = true;
            }
        }
    }

    internal class Promise<T> : PromiseBase
    {
        private readonly TaskCompletionSource<T> taskCompletionSource;

        public Task<T> Task => this.taskCompletionSource.Task;

        public Promise(TaskCompletionSource<T> taskCompletionSource, PromiseCreateOptions createOptions, CancellationToken cancellationToken)
            : base(createOptions, cancellationToken)
        {
            this.taskCompletionSource = taskCompletionSource;
        }
    }

    internal class Promise : PromiseBase
    {
        private readonly TaskCompletionSource<bool> taskCompletionSource;

        public Task Task => this.taskCompletionSource.Task;

        public Promise(TaskCompletionSource<bool> taskCompletionSource, PromiseCreateOptions createOptions, CancellationToken cancellationToken)
            : base(createOptions, cancellationToken)
        {
            this.taskCompletionSource = taskCompletionSource;
        }

        public static Promise Create(CancellationToken cancellationToken)
        {
            return Promise.Create(null, cancellationToken);
        }

        public static Promise Create(Action<float> progress, CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            PromiseCreateOptions createOptions = BuildOptions(_ => true, taskCompletionSource, progress) ;

            return new Promise(taskCompletionSource, createOptions, cancellationToken);
        }

        public static Promise<TResult> Create<TResult>(Func<PackedValue, TResult> factory, CancellationToken cancellationToken)
        {
            return Promise.Create<TResult>(factory, null, cancellationToken);
        }

        public static Promise<TResult> Create<TResult>(Func<PackedValue, TResult> factory, Action<float> progress, CancellationToken cancellationToken)
        {
            TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
            PromiseCreateOptions createOptions = BuildOptions(factory, taskCompletionSource, progress);

            return new Promise<TResult>(taskCompletionSource, createOptions, cancellationToken);
        }

        private static PromiseCreateOptions BuildOptions<TResult>(Func<PackedValue, TResult> factory, TaskCompletionSource<TResult> taskCompletionSource, Action<float> progress)
        {
            SynchronizationContext synchronizationContext = SynchronizationContext.Current;

            return new PromiseCreateOptions()
            {
                context = IntPtr.Zero,
                onComplete = (IntPtr context, ref PackedValue value) =>
                {
                    TResult result = factory(value);

                    if (synchronizationContext != null)
                    {
                        synchronizationContext.Post(_ => taskCompletionSource.SetResult(result), null);
                    }
                    else
                    {
                        taskCompletionSource.SetResult(result);
                    }
                },
                onError = (IntPtr context, ResultCode result) =>
                {
                    if (result == ResultCode.OperationCanceled)
                    {
                        taskCompletionSource.SetCanceled();
                    }
                    else
                    {
                        taskCompletionSource.SetException(new InvalidOperationException($"{result}"));
                    }
                },
                onProgress = (progress == null ? null : new apigen_promise_on_progress((_, value) => progress(value)))
            };
        }
    }
}
