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
                return (CompProperties_ScaresAnimals)props;
            }
        }

        public int TickInterval
        {
            get
            {
                return Props.ticksPerPulse;
            }
        }

        public float Radius
        {
            get
            {
                return Props.effectRadius;
            }
        }

        public bool IsCheapIntervalTick
        {
            get
            {
                return (Find.TickManager.TicksGame + hashOffset) % TickInterval == 0;
            }
        }

        public bool ShouldAffectColonyAnimal(Pawn animal)
        {
            return affectCA || animal.Faction != Faction.OfPlayer;
        }

        public bool IsAffectedPredator(Pawn animal)
        {
            RaceProperties race = animal.def.race;
            bool flag = race.predator && animal.Faction == Faction.OfPlayer;
            return !flag && (!race.predator || race.maxPreyBodySize < CompScaresAnimals.HumanBodySize);
        }

        public bool SmallEnoughToScare(Pawn animal)
        {
            bool flag = Props.maxBodySizeToScare < 0f;
            bool result;
            if (flag)
            {
                result = (animal.BodySize < CompScaresAnimals.HumanBodySize);
            }
            else
            {
                result = (animal.BodySize <= Props.maxBodySizeToScare);
            }
            return result;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            hashOffset = parent.thingIDNumber.HashOffset();
            if (respawningAfterLoad)
            {
                CompScaresAnimals.HumanBodySize = ThingDefOf.Human.race.baseBodySize;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            bool isCheapIntervalTick = IsCheapIntervalTick;
            if (isCheapIntervalTick)
            {
                ScareAnimals();
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            ScareAnimals();
        }

        public void ScareAnimals()
        {
            IEnumerable<Pawn> enumerable = from x in parent.Map.mapPawns.AllPawns
                                           where x.def.race.intelligence == Intelligence.Animal && SmallEnoughToScare(x) && IsAffectedPredator(x) && ShouldAffectColonyAnimal(x) && !x.Downed
                                           select x;
            if (!(enumerable == null || enumerable.Count() <= 0))
            {
                foreach (Pawn pawn in enumerable)
                {
                    if (pawn != null)
                    {
                        if ((pawn.Faction == null || (pawn.Faction == Faction.OfPlayer && affectCA)) && pawn.Position.DistanceTo(parent.Position) < Radius)
                        {
                            Job newJob = new Job(JobDefOf.Flee, CellFinderLoose.GetFleeDest(pawn, parent.Map.listerThings.ThingsOfDef(parent.def), Props.minFleeDistance), parent.Position);
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