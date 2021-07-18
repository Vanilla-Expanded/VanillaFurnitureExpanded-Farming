using Verse;

namespace VFEF
{
    public class CompProperties_Sprinkler : CompProperties
    {
        public CompProperties_Sprinkler()
        {
            this.compClass = typeof(CompSprinkler);
        }

        public float effectRadius;

        public bool shouldSprinkleMotes = false;

        public int sprinkleDurationTicks;

        public int moteMod;

        public float degreesPerTick;

        public ThingDef moteThingDef;
    }
}