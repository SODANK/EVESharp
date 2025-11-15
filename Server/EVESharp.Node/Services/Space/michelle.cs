using System;
using EVESharp.EVE.Network.Services;
using EVESharp.EVE.Sessions;
using EVESharp.Types;
using EVESharp.Types.Collections;

namespace EVESharp.Node.Services.Space
{
    [ConcreteService("michelle")]
    public class michelle : ClientBoundService
    {
        public michelle(IBoundServiceManager mgr, Session session, int objectID)
            : base(mgr, session, objectID)
        {
            Console.WriteLine($"[michelle] Bound instance created (OID={objectID})");
        }

        // Global constructor for service (unbound)
        public michelle(IBoundServiceManager mgr)
            : base(mgr)
        {
            Console.WriteLine("[michelle] Global service constructed");
        }

        public override AccessLevel AccessLevel => AccessLevel.None;

        // Called by client: "michelle.MachoResolveObject"
        protected override long MachoResolveObject(ServiceCall call, ServiceBindParams bindParams)
        {
            Console.WriteLine("[michelle] MachoResolveObject");
            return call.MachoNet.NodeID;
        }

        // Called after resolve: "michelle.MachoBindObject"
        protected override PyDataType MachoBindObject(ServiceCall call, ServiceBindParams bindParams, PyDataType callInfo)
        {
            Console.WriteLine("[michelle] MachoBindObject executing");
            return base.MachoBindObject(call, bindParams, callInfo);
        }

        // Client calls this: michelle.GetBallpark(solarSystemID)
        public PyDataType GetBallpark(ServiceCall call, PyInteger solarSystemID)
        {
            int ssid = (int)solarSystemID.Value;
            Console.WriteLine($"[michelle] GetBallpark called for ssid={ssid}");

            // Spawn ballparkSvc
            var bp = new ballparkSvc(BoundServiceManager, call.Session, ssid);
            BoundServiceManager.BindService(bp);

            Console.WriteLine($"[michelle] Ballpark bound, boundID={bp.BoundID}");

            return new PyInteger(ssid);
        }

        protected override BoundService CreateBoundInstance(ServiceCall call, ServiceBindParams bindParams)
        {
            return new michelle(BoundServiceManager, call.Session, bindParams.ObjectID);
        }

        public override bool IsClientAllowedToCall(Session session) => true;
        public override void ClientHasReleasedThisObject(Session session) { }
        public override void ApplySessionChange(int c, PyDictionary<PyString, PyTuple> delta) { }
        public override void DestroyService() { }
    }
}
