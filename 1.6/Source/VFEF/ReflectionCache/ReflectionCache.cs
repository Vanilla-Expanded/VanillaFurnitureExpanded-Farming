using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;



namespace VFEF
{
    public class ReflectionCache
    {
        public static readonly AccessTools.FieldRef<Building_PlantGrower, ThingDef> plantDefToGrow =
           AccessTools.FieldRefAccess<Building_PlantGrower, ThingDef>(AccessTools.Field(typeof(Building_PlantGrower), "plantDefToGrow"));

      
    }
}
