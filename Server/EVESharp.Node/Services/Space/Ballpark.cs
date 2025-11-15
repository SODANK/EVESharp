using System;
using System.Collections.Generic;
using EVESharp.EVE.Data.Inventory.Items;
using EVESharp.Types;
using EVESharp.Types.Collections;

namespace EVESharp.Node.Services.Space
{
    public class Ballpark
    {
        public int SolarSystemID { get; }
        public int OwnerID { get; }

        private readonly Dictionary<int, ItemEntity> mEntities =
            new Dictionary<int, ItemEntity>();

        public Ballpark(int solarSystemID, int ownerID)
        {
            SolarSystemID = solarSystemID;
            OwnerID       = ownerID;
        }

        public void AddEntity(ItemEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            mEntities[entity.ID] = entity;
        }

        public PyDictionary ToPyDict()
{
    // ENTITY TUPLE (not list)
    var entitiesTuple = new PyTuple(mEntities.Count);

    int i = 0;
    foreach (ItemEntity entity in mEntities.Values)
    {
        // POSITION MUST be a PyTuple of PyInteger
        var position = new PyTuple(3)
        {
            [0] = new PyInteger(0),
            [1] = new PyInteger(0),
            [2] = new PyInteger(0)
        };

        var pyEntity = new PyDictionary
        {
            ["itemID"]   = new PyInteger(entity.ID),
            ["typeID"]   = new PyInteger(entity.Type.ID),
            ["ownerID"]  = new PyInteger(entity.OwnerID),
            ["position"] = position
        };

        entitiesTuple[i++] = pyEntity;
    }

    var ballpark = new PyDictionary
    {
        ["solarsystemID"] = new PyInteger(SolarSystemID),
        ["entities"]      = entitiesTuple,
        ["formations"]    = new PyTuple()     // REQUIRED by client Michelle.AddBallpark
    };

    return ballpark;
}

    }
}
