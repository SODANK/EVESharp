using System;
using EVESharp.EVE.Network.Services;
using EVESharp.EVE.Network.Services.Validators;
using EVESharp.EVE.Sessions;
using EVESharp.Types;
using EVESharp.Types.Collections;

namespace EVESharp.Node.Services.Graphics
{
    // This must match the service name the client moniker uses.
    [ConcreteService("beyonce")]
    public class beyonce : ClientBoundService
    {
        public override AccessLevel AccessLevel => AccessLevel.None;

        // Global (unbound) service instance – constructed at startup
        public beyonce(IBoundServiceManager manager)
            : base(manager)
        {
            Console.WriteLine("[beyonce] global service constructed");
        }

        // Per-client bound instance
        private beyonce(IBoundServiceManager manager, Session session, int objectID)
            : base(manager, session, objectID)
        {
            Console.WriteLine(
                $"[beyonce] bound instance created: objectID={objectID}, charID={session.CharacterID}"
            );
        }

        // --- Binding logic ----------------------------------------------------

        /// <summary>
        /// Create the bound instance. The client moniker for beyonce uses
        /// (\"beyonce\", solarSystemID), so bindParams.ObjectID == solarSystemID.
        /// </summary>
        protected override BoundService CreateBoundInstance(ServiceCall call, ServiceBindParams bindParams)
        {
            int objectID = bindParams.ObjectID;

            Console.WriteLine($"[beyonce] CreateBoundInstance for solarSystemID={objectID}");
            return new beyonce(BoundServiceManager, call.Session, objectID);
        }

        /// <summary>
        /// Decide which node should host this object. For now, always local.
        /// </summary>
        [MustBeCharacter]
        protected override long MachoResolveObject(ServiceCall call, ServiceBindParams bindParams)
        {
            Console.WriteLine("[beyonce] MachoResolveObject -> local node");
            return call.MachoNet.NodeID;
        }

        /// <summary>
        /// Log bind requests and delegate to ClientBoundService to do the real work.
        /// </summary>
        protected override PyDataType MachoBindObject(ServiceCall call, ServiceBindParams bindParams, PyDataType callInfo)
        {
            Console.WriteLine(
                $"[beyonce] MachoBindObject called for solarSystemID={bindParams.ObjectID}, " +
                $"charID={call.Session.CharacterID}"
            );

            var result = base.MachoBindObject(call, bindParams, callInfo);

            Console.WriteLine("[beyonce] MachoBindObject completed successfully");
            return result;
        }

        // --- RPC methods ------------------------------------------------------

        /// <summary>
        /// Called by the client after binding to beyonce when entering space.
        /// We don't implement formations yet, so return an empty list for now.
        /// </summary>
        public PyDataType GetFormations(ServiceCall call)
            {
            Console.WriteLine("[beyonce] GetFormations called");

              // Empty tuple with a real 0-length backing array
               return new PyTuple(0);
               }

       
        /// <summary>
        /// Extra stub you can keep around for testing if you ever want to call it.
        /// Not currently used by the client.
        /// </summary>
        public PyNone Hello(ServiceCall call)
        {
            Console.WriteLine("[beyonce] Hello() called (stub)");
            return new PyNone();
        }


        public PyDataType GetFormationsForBall(ServiceCall call, PyInteger ballID)
        {
            Console.WriteLine($"[beyonce] GetFormationsForBall(ballID={ballID.Value})");

            // Must return a tuple, not a list
            return new PyTuple();
        }

    }
}
