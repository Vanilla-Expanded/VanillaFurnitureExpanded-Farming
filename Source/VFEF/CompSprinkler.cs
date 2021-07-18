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
                return (CompProperties_Sprinkler)this.props;
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
            this.parent.Map.GetComponent<VFEF_SprinklersManager>().Register(this);
            this.LastSprinkledMotesTick = (long)(GenTicks.TicksAbs - 60000);
            this.affectCells.AddRange(this.parent.Map.AllCells.Where(cell => this.parent.Position.DistanceTo(cell) < this.Props.effectRadius));
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            map.GetComponent<VFEF_SprinklersManager>().Deregister(this);
            this.affectCells.Clear();
        }

        public void StartSprinklingMotes()
        {
            this.curRot = 0f;
            this.CurrentlySprinklingMotes = true;
            this.MoteSprinkleEndTick = (long)(GenTicks.TicksAbs + this.Props.sprinkleDurationTicks);
            this.SprinkleMotes();
            this.LastSprinkledMotesTick = (long)GenTicks.TicksAbs;
        }

        public void SprinkleMotes()
        {
            if (GenTicks.TicksAbs > this.MoteSprinkleEndTick)
            {
                this.CurrentlySprinklingMotes = false;
            }
            if (GenTicks.TicksAbs % this.Props.moteMod == 0)
            {
                MoteSprinkler.ThrowWaterSpray(this.parent.TrueCenter(), this.parent.Map, this.curRot, this.Props.moteThingDef);
            }
            this.curRot += this.Props.degreesPerTick;
        }
    }
}