using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

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

        public Map map;
        public CompPowerTrader compPowerTrader;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            map = parent.Map;

            LastSprinkledMotesTick = GenTicks.TicksAbs - 60000;
            affectCells = GenRadial.RadialCellsAround(parent.Position, Props.effectRadius, true).ToList();

            map.GetComponent<VFEF_SprinklersManager>().Register(this);
            compPowerTrader = parent.GetComp<CompPowerTrader>();
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            affectCells.Clear();
            map.GetComponent<VFEF_SprinklersManager>().Deregister(this);
        }

        public void StartSprinklingMotes()
        {
            curRot = 0f;
            CurrentlySprinklingMotes = true;
            MoteSprinkleEndTick = GenTicks.TicksAbs + Props.sprinkleDurationTicks;
            SprinkleMotes();
            LastSprinkledMotesTick = GenTicks.TicksAbs;
        }

        public void SprinkleMotes()
        {
            if (GenTicks.TicksAbs > MoteSprinkleEndTick)
            {
                CurrentlySprinklingMotes = false;
            }
            if (GenTicks.TicksAbs % Props.moteMod == 0)
            {
                MoteSprinkler.ThrowWaterSpray(parent.TrueCenter(), map, curRot, Props.moteThingDef);
            }
            curRot += Props.degreesPerTick;
        }
    }
}