using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace VFEF
{
    internal class CompAreaGizmo : ThingComp
    {
        public CompProperties_AreaGizmo Props
        {
            get
            {
                return (CompProperties_AreaGizmo)this.props;
            }
        }

        public int Radius
        {
            get
            {
                return this.Props.radius;
            }
        }

        public string TexturePath
        {
            get
            {
                return this.Props.iconPath;
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            base.CompGetGizmosExtra();
            bool flag = DesignatorUtility.FindAllowedDesignator<Designator_AreaAllowedExpand>() != null;
            if (flag)
            {
                yield return new Command_Action
                {
                    action = new Action(this.DesignateArea),
                    hotKey = KeyBindingDefOf.Misc1,
                    defaultDesc = "VFEF_CommandMakeCompAreaDesc".Translate(this.parent.Label),
                    icon = ContentFinder<Texture2D>.Get(this.TexturePath, true),
                    defaultLabel = "VFEF_CommandMakeCompAreaLabel".Translate(this.parent.Label)
                };
            }
            yield break;
        }

        private void DesignateArea()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            Designator_AreaAllowedExpand des = DesignatorUtility.FindAllowedDesignator<Designator_AreaAllowedExpand>() as Designator_AreaAllowedExpand;
            using (IEnumerator<Area_Allowed> enumerator = this.parent.Map.areaManager.AllAreas.OfType<Area_Allowed>().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Area_Allowed a = enumerator.Current;
                    list.Add(new FloatMenuOption(a.Label, delegate ()
                    {
                        this.SetArea(des, a);
                        des.DesignateMultiCell(this.CellsInRectangularRadius());
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private IEnumerable<IntVec3> CellsInRectangularRadius()
        {
            IntVec3 bottomLeft = this.parent.OccupiedRect().BottomLeft;
            int minX = bottomLeft.x - this.Radius;
            int minZ = bottomLeft.z - this.Radius;
            IntVec3 topRight = this.parent.OccupiedRect().TopRight;
            int maxX = topRight.x + this.Radius;
            int maxZ = topRight.z + this.Radius;
            foreach (IntVec3 cell in this.parent.Map.AllCells)
            {
                if (cell.x >= minX && cell.x <= maxX && cell.z >= minZ && cell.z <= maxZ)
                {
                    yield return cell;
                }
            }
            yield break;
        }

        private void SetArea(Designator_AreaAllowedExpand des, Area_Allowed a)
        {
            bool flag = a == null;
            if (!flag)
            {
                Type typeFromHandle = typeof(Designator_AreaAllowed);
                FieldInfo field = typeFromHandle.GetField("selectedArea", BindingFlags.Static | BindingFlags.NonPublic);
                bool flag2 = field != null;
                if (flag2)
                {
                    field.SetValue(null, a);
                }
            }
        }
    }
}