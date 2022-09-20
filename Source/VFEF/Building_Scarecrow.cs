using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace VFEF
{
    internal class Building_Scarecrow : Building
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref shouldScare, "shouldScare", false, false);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            bool flag = base.Faction == Faction.OfPlayer;
            if (flag)
            {
                yield return new Command_Toggle
                {
                    defaultLabel = "VFEF_ScarecrowGizmo".Translate(),
                    defaultDesc = "VFEF_ScarecrowGizmoText".Translate(),
                    hotKey = KeyBindingDefOf.Misc3,
                    icon = ContentFinder<Texture2D>.Get("Icons/Forbidden", true),
                    isActive = (() => shouldScare),
                    toggleAction = delegate ()
                    {
                        shouldScare = !shouldScare;
                        this.TryGetComp<CompScaresAnimals>().affectCA = shouldScare;
                    }
                };
            }
            yield break;
        }

        public bool shouldScare = false;
    }
}