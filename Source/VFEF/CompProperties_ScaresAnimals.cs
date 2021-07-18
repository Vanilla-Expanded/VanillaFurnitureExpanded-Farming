using Verse;

namespace VFEF
{
    public class CompProperties_ScaresAnimals : CompProperties
    {
        public CompProperties_ScaresAnimals()
        {
            this.compClass = typeof(CompScaresAnimals);
        }

        public int ticksPerPulse;

        public float effectRadius;

        public float minFleeDistance = 23f;

        public float maxBodySizeToScare = -1f;

        public bool AffectColonyAnimals = true;
    }
}