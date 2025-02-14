// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildXL.Cache.ContentStore.Hashing;
using BuildXL.Cache.ContentStore.Interfaces.Sessions;
using BuildXL.Utilities;

namespace BuildXL.Cache.Interfaces.Test
{
    public class CallbackCacheReadOnlySessionWrapper : ICacheReadOnlySession
    {
        private readonly ICacheReadOnlySession m_realSession;

        public CallbackCacheReadOnlySessionWrapper(ICacheReadOnlySession realCache)
        {
            m_realSession = realCache;
        }

        /// <summary>
        /// The underlying session
        /// </summary>
        public ICacheReadOnlySession WrappedSession => m_realSession;

        public Func<ICacheReadOnlySession, CacheId> CacheIdCallback;

        public CacheId CacheId
        {
            get
            {
                var callback = CacheIdCallback;
                if (callback != null)
                {
                    return callback(m_realSession);
                }
                else
                {
                    return m_realSession.CacheId;
                }
            }
        }

        public Func<ICacheReadOnlySession, string> CacheSessionIdCallback;

        public string CacheSessionId
        {
            get
            {
                var callback = CacheSessionIdCallback;
                if (callback != null)
                {
                    return callback(m_realSession);
                }
                else
                {
                    return m_realSession.CacheSessionId;
                }
            }
        }

        public Func<ICacheReadOnlySession, bool> IsClosedCallback;

        public bool IsClosed
        {
            get
            {
                var callback = IsClosedCallback;
                if (callback != null)
                {
                    return callback(m_realSession);
                }
                else
                {
                    return m_realSession.IsClosed;
                }
            }
        }

        public Func<ICacheReadOnlySession, bool> StrictMetadataCasCouplingCallback;

        public bool StrictMetadataCasCoupling
        {
            get
            {
                var callback = StrictMetadataCasCouplingCallback;
                if (callback != null)
                {
                    return callback(m_realSession);
                }
                else
                {
                    return m_realSession.StrictMetadataCasCoupling;
                }
            }
        }

        public Func<Guid, ICacheReadOnlySession, Task<Possible<string, Failure>>> CloseAsyncCallback;

        public Task<Possible<string, Failure>> CloseAsync(Guid activityId)
        {
            var callback = CloseAsyncCallback;
            if (callback != null)
            {
                return callback(activityId, m_realSession);
            }
            else
            {
                return m_realSession.CloseAsync(activityId);
            }
        }

        public Func<WeakFingerprintHash, OperationHints, Guid, ICacheReadOnlySession, IEnumerable<Task<Possible<StrongFingerprint, Failure>>>> EnumerateStrongFingerprintsCallback;

        public IEnumerable<Task<Possible<StrongFingerprint, Failure>>> EnumerateStrongFingerprints(WeakFingerprintHash weak, OperationHints hints, Guid activityId)
        {
            var callback = EnumerateStrongFingerprintsCallback;
            if (callback != null)
            {
                return callback(weak, hints, activityId, m_realSession);
            }
            else
            {
                return m_realSession.EnumerateStrongFingerprints(weak, hints, activityId);
            }
        }

        public Func<StrongFingerprint, UrgencyHint, Guid, ICacheReadOnlySession, Task<Possible<CasEntries, Failure>>> GetCacheEntryAsyncCallback;

        public Task<Possible<CasEntries, Failure>> GetCacheEntryAsync(StrongFingerprint strong, OperationHints hints, Guid activityId)
        {
            var callback = GetCacheEntryAsyncCallback;
            if (callback != null)
            {
                return callback(strong, hints.Urgency, activityId, m_realSession);
            }
            else
            {
                return m_realSession.GetCacheEntryAsync(strong, hints, activityId);
            }
        }

        public Func<Guid, ICacheReadOnlySession, Task<Possible<CacheSessionStatistics[], Failure>>> GetStatisticsAsyncCallback;

        public Task<Possible<CacheSessionStatistics[], Failure>> GetStatisticsAsync(Guid activityId = default(Guid))
        {
            var callback = GetStatisticsAsyncCallback;
            if (callback != null)
            {
                return callback(activityId, m_realSession);
            }
            else
            {
                return m_realSession.GetStatisticsAsync(activityId);
            }
        }

        public Func<CasHash, UrgencyHint, Guid, ICacheReadOnlySession, Task<Possible<StreamWithLength, Failure>>> GetStreamAsyncCallback;

        public Task<Possible<StreamWithLength, Failure>> GetStreamAsync(CasHash hash, OperationHints hints, Guid activityId)
        {
            var callback = GetStreamAsyncCallback;
            if (callback != null)
            {
                return callback(hash, hints, activityId, m_realSession);
            }
            else
            {
                return m_realSession.GetStreamAsync(hash, hints, activityId);
            }
        }

        public Func<CasEntries, OperationHints, Guid, CancellationToken, ICacheReadOnlySession, Task<Possible<string, Failure>[]>> PinToCasMultipleAsyncCallback;

        public Task<Possible<string, Failure>[]> PinToCasAsync(CasEntries hashes, CancellationToken cancellationToken, OperationHints hints, Guid activityId)
        {
            var callback = PinToCasMultipleAsyncCallback;
            if (callback != null)
            {
                return callback(hashes, hints, activityId, cancellationToken, m_realSession);
            }
            else
            {
                return m_realSession.PinToCasAsync(hashes, cancellationToken, hints, activityId);
            }
        }

        public Func<CasHash, OperationHints, Guid, CancellationToken, ICacheReadOnlySession, Task<Possible<string, Failure>>> PinToCasAsyncCallback;

        public Task<Possible<string, Failure>> PinToCasAsync(CasHash hash, CancellationToken cancellationToken, OperationHints hints, Guid activityId)
        {
            var callback = PinToCasAsyncCallback;
            if (callback != null)
            {
                return callback(hash, hints, activityId, cancellationToken, m_realSession);
            }
            else
            {
                return m_realSession.PinToCasAsync(hash, cancellationToken, hints, activityId);
            }
        }

        public Func<CasHash, UrgencyHint, Guid, ICacheReadOnlySession, Task<Possible<ValidateContentStatus, Failure>>> ValidateContentAsyncCallback;

        public Task<Possible<ValidateContentStatus, Failure>> ValidateContentAsync(CasHash hash, UrgencyHint urgencyHint, Guid activityId)
        {
            var callback = ValidateContentAsyncCallback;
            if (callback != null)
            {
                return callback(hash, urgencyHint, activityId, m_realSession);
            }
            else
            {
                return m_realSession.ValidateContentAsync(hash, urgencyHint, activityId);
            }
        }

        public Func<CasHash,
            string,
            FileState,
            UrgencyHint,
            Guid,
            CancellationToken,
            ICacheReadOnlySession,
            Task<Possible<string, Failure>>> ProduceFileAsyncCallback;

        public Task<Possible<string, Failure>> ProduceFileAsync(
            CasHash hash,
            string filename,
            FileState fileState,
            OperationHints hints,
            Guid activityId,
            CancellationToken cancellationToken)
        {
            var callback = ProduceFileAsyncCallback;
            if (callback != null)
            {
                return callback(
                    hash,
                    filename,
                    fileState,
                    hints,
                    activityId,
                    cancellationToken,
                    m_realSession);
            }
            else
            {
                return m_realSession.ProduceFileAsync(
                    hash,
                    filename,
                    fileState,
                    hints,
                    activityId,
                    cancellationToken);
            }
        }
    }
}
