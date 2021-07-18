using RimWorld;
using Verse;

namespace VFEF
{
    internal class Building_PlantGrower_NoEmptyLines : Building_PlantGrower
    {
        public override string GetInspectString()
        {
            bool spawned = base.Spawned;
            string result;
            if (spawned)
            {
                result = ((!PlantUtility.GrowthSeasonNow(base.Position, base.Map, true)) ? "CannotGrowBadSeasonTemperature".Translate() : "GrowSeasonHereNow".Translate());
            }
            else
            {
                result = "VFEF.Building_PlantGrower_NoEmptyLines: GetInspectString() called but building is not spawned.";
            }
            return result;
        }
    }
}