using UnityEngine;
using Verse;

namespace VFEF
{
    internal static class MoteSprinkler
    {
        public static MoteThrown NewMote(ThingDef def)
        {
            MoteThrown moteThrown = ThingMaker.MakeThing(def, null) as MoteThrown;
            moteThrown.Scale = 1.5f;
            moteThrown.rotationRate = (float)Rand.RangeInclusive(-30, 30);
            return moteThrown;
        }

        public static void ThrowWaterSpray(Vector3 loc, Map map, float angle, ThingDef def)
        {
            bool flag = loc.ShouldSpawnMotesAt(map) && !map.moteCounter.SaturatedLowPriority;
            if (flag)
            {
                MoteThrown moteThrown = MoteSprinkler.NewMote(def);
                moteThrown.exactPosition = loc;
                moteThrown.SetVelocity(angle, Rand.Range(MoteSprinkler.minVelocity, MoteSprinkler.maxVelocity));
                MoteThrown moteThrown2 = MoteSprinkler.NewMote(def);
                moteThrown2.exactPosition = loc;
                moteThrown2.SetVelocity(angle + 180f, Rand.Range(MoteSprinkler.minVelocity, MoteSprinkler.maxVelocity));
                GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
                GenSpawn.Spawn(moteThrown2, loc.ToIntVec3(), map, WipeMode.Vanish);
            }
        }

        public static float minVelocity = 1.7f;

        public static float maxVelocity = 2f;
    }
}