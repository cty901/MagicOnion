﻿using System;
using Grpc.Core;
using UniRx;

namespace MagicOnion
{
    /// <summary>
    /// Wrapped AsyncUnaryCall.
    /// </summary>
    public struct UnaryResult<TResponse>
    {
        readonly AsyncUnaryCall<byte[]> inner;
        readonly Marshaller<TResponse> marshaller;

        public UnaryResult(AsyncUnaryCall<byte[]> inner, Marshaller<TResponse> marshaller)
        {
            this.inner = inner;
            this.marshaller = marshaller;
        }

        /// <summary>
        /// Asynchronous call result.
        /// </summary>
        public IObservable<TResponse> ResponseAsync
        {
            get
            {
                var m = marshaller; // struct can not use field value in lambda(if avoid, we needs to implement SelectWithState)
                return inner.ResponseAsync.Select(x => m.Deserializer(x));
            }
        }

        public IObservable<TResponse> ResponseAsyncOnMainThread
        {
            get
            {
                return ResponseAsync.ObserveOnMainThread();
            }
        }

        /// <summary>
        /// Asynchronous access to response headers.
        /// </summary>
        public IObservable<Metadata> ResponseHeadersAsync
        {
            get
            {
                return inner.ResponseHeadersAsync;
            }
        }

        /// <summary>
        /// Gets the call status if the call has already finished.
        /// Throws InvalidOperationException otherwise.
        /// </summary>
        public Status GetStatus()
        {
            return inner.GetStatus();
        }

        /// <summary>
        /// Gets the call trailing metadata if the call has already finished.
        /// Throws InvalidOperationException otherwise.
        /// </summary>
        public Metadata GetTrailers()
        {
            return inner.GetTrailers();
        }

        /// <summary>
        /// Provides means to cleanup after the call.
        /// If the call has already finished normally (request stream has been completed and call result has been received), doesn't do anything.
        /// Otherwise, requests cancellation of the call which should terminate all pending async operations associated with the call.
        /// As a result, all resources being used by the call should be released eventually.
        /// </summary>
        /// <remarks>
        /// Normally, there is no need for you to dispose the call unless you want to utilize the
        /// "Cancel" semantics of invoking <c>Dispose</c>.
        /// </remarks>
        public void Dispose()
        {
            inner.Dispose();
        }
    }
}