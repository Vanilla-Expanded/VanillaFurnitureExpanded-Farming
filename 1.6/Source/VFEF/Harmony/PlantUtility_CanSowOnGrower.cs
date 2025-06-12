using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;

namespace VFEF
{
    [HarmonyPatch(typeof(PlantUtility), nameof(PlantUtility.CanSowOnGrower))]
    static class VFEF_PlantUtility_CanSowOnGrower_Patch
    {

        public static List<ThingDef> plantersList = new List<ThingDef>() { InternalDefOf.VFE_PlanterBox, InternalDefOf.VFE_PlanterBox_Tilable };

        public static bool Postfix(bool __result, ThingDef plantDef, object obj)
        {
            if ((obj is Building_PlantGrower planter) && plantersList.Contains(planter.def))
            {
                if (planter.def.fertility >= 0f && planter.def.fertility < plantDef.plant.fertilityMin)
                {
                    return false;
                }
            }

            return __result;
        }
    }
}
