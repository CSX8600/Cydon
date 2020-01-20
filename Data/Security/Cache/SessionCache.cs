using System;
using System.Collections.Generic;
using System.Linq;
using Cydon.Data.Base;

namespace Cydon.Data.Security.Cache
{
    public class SessionCache : BaseCache
    {
        private object _lock = new object();
        private Dictionary<string, CachedSession> _cachedSessionBySessionID = new Dictionary<string, CachedSession>();
        public override string Name => "Session";

        internal override void Update()
        {
            lock (_lock)
            {
                _cachedSessionBySessionID = new Dictionary<string, CachedSession>();
                Context context = new Context();
                IEnumerable<Session> sessions = context.Sessions;
                foreach (Session session in sessions)
                {
                    AddCachedSession(session);
                }
            }
        }
        private void AddCachedSession(Session session)
        {
            _cachedSessionBySessionID[session.SessionStateID] = new CachedSession(session.SessionID, session.Expiration, session.UserID);
        }

        public CachedSession GetSessionBySessionID(string sessionID)
        {
            lock (_lock)
            {
                if (!_cachedSessionBySessionID.ContainsKey(sessionID))
                {
                    // Maybe cache is outdated
                    Context context = new Context();
                    Session newSession = context.Sessions.FirstOrDefault(sesh => sesh.SessionStateID == sessionID);
                    if (newSession == null)
                    {
                        return null;
                    }

                    AddCachedSession(newSession);
                }

                return _cachedSessionBySessionID[sessionID];
            }
        }

        public void ForceRefreshSession(string sessionID)
        {
            CachedSession cachedSession = GetSessionBySessionID(sessionID);
            if (cachedSession != null)
            {
                lock (_lock)
                {
                    _cachedSessionBySessionID.Remove(sessionID);
                }
            }

            Context context = new Context();
            Session session = context.Sessions.FirstOrDefault(s => s.SessionStateID == sessionID);
            if (session != null)
            {
                lock(_lock)
                {
                    AddCachedSession(session);
                }
            }
        }

        public class CachedSession
        {
            public long SessionID { get; private set; }
            public DateTime Expiration { get; private set; }
            public long UserID { get; private set; }

            public CachedSession(long sessionID, DateTime expiration, long userID)
            {
                SessionID = sessionID;
                Expiration = expiration;
                UserID = userID;
            }

            public void ResetSessionExpiration()
            {
                Expiration = DateTime.Now.AddMinutes(30);
            }
        }
    }
}
