using System;
using EVESharp.EVE.Network.Services;
using EVESharp.EVE.Sessions;
using EVESharp.Types;
using EVESharp.Types.Collections;

namespace EVESharp.Node.Services.Space
{
    // This registers the global service called "spaceReg"
    [ConcreteService("spaceReg")]
    public class SpaceServiceRegistrar : Service
    {
        public override AccessLevel AccessLevel => AccessLevel.None;

        public SpaceServiceRegistrar()
        {
            Console.WriteLine("[SpaceServiceRegistrar] Global service constructed");
        }

        // The moniker resolver – the client calls this BEFORE binding
        public long MachoResolveObject(ServiceCall call, PyDataType arguments, PyInteger nodeID)
        {
            Console.WriteLine("[SpaceServiceRegistrar] MachoResolveObject() invoked");
            
            // Always return local node — same as dogmaIM, ship, invbroker
            return call.MachoNet.NodeID;
        }

        // The binder – the client calls this AFTER MachoResolveObject
        public PyDataType MachoBindObject(ServiceCall call, PyDataType bindArgs, PyDataType sessionInfo)
        {
            Console.WriteLine("[SpaceServiceRegistrar] MachoBindObject() invoked");

            int objectID = 0;

            // Clients often pass (ballparkID,) inside PyTuple
            if (bindArgs is PyTuple tuple && tuple.Count > 0 && tuple[0] is PyInteger pid)
                objectID = pid;

            // Create a new bound ballpark service
            var bound = new ballparkSvc(call.BoundServiceManager, call.Session, objectID);

            // MUST bind it or client won’t be able to call it
            call.BoundServiceManager.BindService(bound);

            Console.WriteLine($"[SpaceServiceRegistrar] Bound ballparkSvc with boundID={bound.BoundID}");

            // Returning NULL here is normal — ship, dogmaIM, inventory all do this
            return new PyNone();
        }
    }
}
