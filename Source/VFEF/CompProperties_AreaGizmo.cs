using System.Collections.Generic;
using Verse;

namespace VFEF
{
    internal class CompProperties_AreaGizmo : CompProperties
    {
        public CompProperties_AreaGizmo()
        {
            this.compClass = typeof(CompAreaGizmo);
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parent)
        {
            base.ConfigErrors(parent);
            bool flag = this.radius < 0;
            if (flag)
            {
                yield return "CompProperties_AreaGizmo: radius cannot be less than 0.";
            }
            yield break;
        }

        public int radius = 0;

        public string iconPath = null;
    }
}