using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace VFEF
{
    public class CompScaresAnimals : ThingComp
    {
        public CompProperties_ScaresAnimals Props
        {
            get
            {
                return (CompProperties_ScaresAnimals)this.props;
            }
        }

        public int TickInterval
        {
            get
            {
                return this.Props.ticksPerPulse;
            }
        }

        public float Radius
        {
            get
            {
                return this.Props.effectRadius;
            }
        }

        public bool IsCheapIntervalTick
        {
            get
            {
                return (Find.TickManager.TicksGame + this.hashOffset) % this.TickInterval == 0;
            }
        }

        public bool ShouldAffectColonyAnimal(Pawn animal)
        {
            return this.affectCA || animal.Faction != Faction.OfPlayer;
        }

        public bool IsAffectedPredator(Pawn animal)
        {
            RaceProperties race = animal.def.race;
            bool flag = race.predator && animal.Faction == Faction.OfPlayer;
            return !flag && (!race.predator || race.maxPreyBodySize < CompScaresAnimals.HumanBodySize);
        }

        public bool SmallEnoughToScare(Pawn animal)
        {
            bool flag = this.Props.maxBodySizeToScare < 0f;
            bool result;
            if (flag)
            {
                result = (animal.BodySize < CompScaresAnimals.HumanBodySize);
            }
            else
            {
                result = (animal.BodySize <= this.Props.maxBodySizeToScare);
            }
            return result;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.hashOffset = this.parent.thingIDNumber.HashOffset();
            if (respawningAfterLoad)
            {
                CompScaresAnimals.HumanBodySize = ThingDefOf.Human.race.baseBodySize;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            bool isCheapIntervalTick = this.IsCheapIntervalTick;
            if (isCheapIntervalTick)
            {
                this.ScareAnimals();
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            this.ScareAnimals();
        }

        public void ScareAnimals()
        {
            IEnumerable<Pawn> enumerable = from x in this.parent.Map.mapPawns.AllPawns
                                           where x.def.race.intelligence == Intelligence.Animal && this.SmallEnoughToScare(x) && this.IsAffectedPredator(x) && this.ShouldAffectColonyAnimal(x) && !x.Downed
                                           select x;
            if (!(enumerable == null || enumerable.Count() <= 0))
            {
                foreach (Pawn pawn in enumerable)
                {
                    if (pawn != null)
                    {
                        if ((pawn.Faction == null || (pawn.Faction == Faction.OfPlayer && this.affectCA)) && pawn.Position.DistanceTo(this.parent.Position) < this.Radius)
                        {
                            Job newJob = new Job(JobDefOf.Flee, CellFinderLoose.GetFleeDest(pawn, this.parent.Map.listerThings.ThingsOfDef(this.parent.def), this.Props.minFleeDistance), this.parent.Position);
                            pawn.jobs.StartJob(newJob, JobCondition.InterruptOptional, null, false, true, null, null, false, false);
                        }
                    }
                }
            }
        }

        private int hashOffset = 0;

        public static float HumanBodySize = 1f;

        public bool affectCA = false;
    }
}