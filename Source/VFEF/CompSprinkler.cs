using RimWorld;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace VFEF
{
    public class CompSprinkler : ThingComp
    {
        public CompProperties_Sprinkler Props
        {
            get
            {
                return (CompProperties_Sprinkler)props;
            }
        }

        public bool CurrentlySprinklingMotes = false;

        public float curRot;

        public long MoteSprinkleEndTick;

        public long LastSprinkledMotesTick;

        public List<IntVec3> affectCells = new List<IntVec3>();

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            parent.Map.GetComponent<VFEF_SprinklersManager>().Register(this);
            LastSprinkledMotesTick = (long)(GenTicks.TicksAbs - 60000);
            affectCells.AddRange(parent.Map.AllCells.Where(cell => parent.Position.DistanceTo(cell) < Props.effectRadius));
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            map.GetComponent<VFEF_SprinklersManager>().Deregister(this);
            affectCells.Clear();
        }

        public void StartSprinklingMotes()
        {
            curRot = 0f;
            CurrentlySprinklingMotes = true;
            MoteSprinkleEndTick = (long)(GenTicks.TicksAbs + Props.sprinkleDurationTicks);
            SprinkleMotes();
            LastSprinkledMotesTick = (long)GenTicks.TicksAbs;
        }

        public void SprinkleMotes()
        {
            if (GenTicks.TicksAbs > MoteSprinkleEndTick)
            {
                CurrentlySprinklingMotes = false;
            }
            if (GenTicks.TicksAbs % Props.moteMod == 0)
            {
                MoteSprinkler.ThrowWaterSpray(parent.TrueCenter(), parent.Map, curRot, Props.moteThingDef);
            }
            curRot += Props.degreesPerTick;
        }
    }
}