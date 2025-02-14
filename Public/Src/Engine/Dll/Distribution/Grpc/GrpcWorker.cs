// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;
using BuildXL.Distribution.Grpc;
using Grpc.Core;
using BuildXL.Utilities;
using BuildXL.Utilities.Tasks;

namespace BuildXL.Engine.Distribution.Grpc
{
    /// <summary>
    /// Worker service impl
    /// </summary>
    public sealed class GrpcWorker : Worker.WorkerBase
    {
        private readonly IWorkerService m_workerService;

        internal GrpcWorker(IWorkerService service)
        {
            m_workerService = service;
        }

        /// Note: The logic of service methods should be replicated in Test.BuildXL.Distribution.WorkerServerMock
        /// <inheritdoc/>
        public override Task<RpcResponse> Attach(BuildStartData message, ServerCallContext context)
        {
            var bondMessage = message.ToOpenBond();

            GrpcSettings.ParseHeader(context.RequestHeaders, out string sender, out var _, out var _, out var _);

            m_workerService.Attach(bondMessage, sender);

            return Task.FromResult(new RpcResponse());
        }

        /// <inheritdoc/>
        public override Task<RpcResponse> ExecutePips(PipBuildRequest message, ServerCallContext context)
        {
            var bondMessage = message.ToOpenBond();

            m_workerService.ExecutePipsAsync(bondMessage).Forget();
            return Task.FromResult(new RpcResponse());
        }

        /// <inheritdoc/>
        public override Task<RpcResponse> Exit(BuildEndData message, ServerCallContext context)
        {
            var failure = string.IsNullOrEmpty(message.Failure) ? Optional<string>.Empty : message.Failure;
            m_workerService.ExitRequested(failure);
            return Task.FromResult(new RpcResponse());
        }
    }
}