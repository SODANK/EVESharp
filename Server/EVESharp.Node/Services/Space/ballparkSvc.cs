using System;
using EVESharp.EVE.Data.Inventory.Items;     // ItemEntity
using EVESharp.EVE.Network.Services;
using EVESharp.EVE.Sessions;
using EVESharp.Node.Services;                // ConcreteServiceAttribute
using EVESharp.Types;
using EVESharp.Types.Collections;

namespace EVESharp.Node.Services.Space
{
    [ConcreteService("ballparkSvc")]
    public class ballparkSvc : ClientBoundService
    {
        private Ballpark mBallpark;

        public override AccessLevel AccessLevel => AccessLevel.None;

        // Global / unbound ctor – DI will construct this as the service
        public ballparkSvc(IBoundServiceManager manager)
            : base(manager)
        {
            Console.WriteLine("[ballparkSvc] Global service constructed");
        }

        // Bound instance ctor – used by CreateBoundInstance and (optionally) SpaceServiceRegistrar
        public ballparkSvc(IBoundServiceManager manager, Session session, int objectID)
            : base(manager, session, objectID)
        {
            int solarSystemID = session.SolarSystemID ?? 0;
            int ownerID       = session.CharacterID;

            mBallpark = new Ballpark(solarSystemID, ownerID);

            Console.WriteLine(
                $"[ballparkSvc] Bound instance created for char={ownerID}, solarSystemID={solarSystemID}, objectID={objectID}");
        }

        /// <summary>
        /// Standard resolver for bound services.
        /// For now we just say "this node owns all ballparks".
        /// </summary>
        protected override long MachoResolveObject(ServiceCall call, ServiceBindParams parameters)
        {
            // Later you can add logic to route different solar systems to different nodes
            return BoundServiceManager.MachoNet.NodeID;
        }

        /// <summary>
        /// Creates a bound instance for the requested object.
        /// bindParams.ObjectID will typically be the solarSystemID or location ID.
        /// </summary>
        protected override BoundService CreateBoundInstance(ServiceCall call, ServiceBindParams bindParams)
        {
            Console.WriteLine($"[ballparkSvc] CreateBoundInstance for objectID={bindParams.ObjectID}");

            // Construct a new bound ballpark service tied to this session + objectID
            var instance = new ballparkSvc(BoundServiceManager, call.Session, bindParams.ObjectID);

            return instance;
        }

        /// <summary>
        /// Called by the client on the bound object to get the current ballpark snapshot.
        /// </summary>
        public PyDataType EnterBallpark(ServiceCall call)
        {
            if (mBallpark == null)
            {
                int solarSystemID = call.Session.SolarSystemID ?? 0;
                int ownerID       = call.Session.CharacterID;

                Console.WriteLine("[ballparkSvc] EnterBallpark with no existing Ballpark, creating one.");
                mBallpark = new Ballpark(solarSystemID, ownerID);
            }

            Console.WriteLine("[ballparkSvc] Returning Ballpark snapshot to client.");
            return mBallpark.ToPyDict();
        }

        /// <summary>
        /// Helper to inject an entity into this Ballpark.
        /// Not currently used by Undock() (we reverted that),
        /// but ready for later integration.
        /// </summary>
        public void AddEntity(ItemEntity entity)
        {
            if (mBallpark == null)
            {
                int solarSystemID = this.Session.SolarSystemID ?? 0;
                int ownerID       = this.Session.CharacterID;

                Console.WriteLine("[ballparkSvc] AddEntity called with no Ballpark yet, creating one.");
                mBallpark = new Ballpark(solarSystemID, ownerID);
            }

            mBallpark.AddEntity(entity);
            Console.WriteLine($"[ballparkSvc] Added entity {entity.ID} to Ballpark.");
        }

        /// <summary>
        /// Optional clean-up hook – currently no extra work.
        /// ClientBoundService will handle unbinding & OnMachoObjectDisconnect.
        /// </summary>
        protected override void OnClientDisconnected()
        {
            Console.WriteLine("[ballparkSvc] Client disconnected from ballpark.");
        }
    }
}
