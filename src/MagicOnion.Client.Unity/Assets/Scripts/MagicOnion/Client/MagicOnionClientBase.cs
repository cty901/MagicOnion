﻿using Grpc.Core;
using System;
using System.Threading;
using UniRx;
using ZeroFormatter.Formatters;

namespace MagicOnion.Client
{
    public abstract class MagicOnionClientBase<T> where T : IService<T>
    {
        protected string host;
        protected CallOptions option;
        protected CallInvoker callInvoker;

        protected MagicOnionClientBase()
        {

        }

        protected MagicOnionClientBase(CallInvoker callInvoker)
        {
            this.callInvoker = callInvoker;
        }

        protected abstract MagicOnionClientBase<T> Clone();

        public T WithCancellationToken(GrpcCancellationToken cancellationToken)
        {
            return WithOptions(this.option.WithCancellationToken(cancellationToken));
        }

        public T WithDeadline(DateTime deadline)
        {
            return WithOptions(this.option.WithDeadline(deadline));
        }

        public T WithHeaders(Metadata headers)
        {
            return WithOptions(this.option.WithHeaders(headers));
        }

        public T WithHost(string host)
        {
            var newInstance = Clone();
            newInstance.host = host;
            return (T)(object)newInstance;
        }

        public T WithOptions(CallOptions option)
        {
            var newInstance = Clone();
            newInstance.option = option;
            return (T)(object)newInstance;
        }
    }
}