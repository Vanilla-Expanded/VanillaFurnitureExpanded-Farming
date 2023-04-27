
using RimWorld;
using Verse;


namespace VFEF
{
    [DefOf]
    public static class InternalDefOf
    {
        public static ThingDef VFE_PlanterBox;
        public static ThingDef VFE_PlanterBox_Tilable;
       

        static InternalDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(InternalDefOf));
        }
    }
}
