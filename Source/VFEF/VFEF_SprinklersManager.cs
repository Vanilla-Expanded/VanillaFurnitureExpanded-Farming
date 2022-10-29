using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VFEF
{
    class VFEF_SprinklersManager : MapComponent
    {
        public VFEF_SprinklersManager(Map map) : base(map)
        {
        }

        private List<IntVec3> affectedCells = new List<IntVec3>();

        private readonly List<CompSprinkler> comps = new List<CompSprinkler>();
        private readonly List<string> exception = new List<string>()
        {
            "VFE_PlanterBox",
            "VFE_PlanterBox_Tilable"
        };

        public void Register(CompSprinkler c)
        {
            comps.Add(c);
            ReCacheCells();
        }

        public void Deregister(CompSprinkler c)
        {
            comps.Remove(c);
            ReCacheCells();
        }

        private void BoostPlantAt(IntVec3 cell)
        {
            if (cell.GetPlant(map) is Plant _plant && !_plant.def.plant.IsTree) // No tree growth boost
            {
                _plant.Growth += 0.04f;
            }
        }

        private void ReCacheCells()
        {
            for (int i = 0; i < comps.Count; i++)
            {
                affectedCells.AddRange(comps[i].affectCells);
            }
            affectedCells = affectedCells.Distinct().ToList(); // Remove duplicate cells
        }

        public override void MapComponentTick()
        {
            if (GenLocalDate.HourOfDay(map) == 7)
            {
                var ticksAbs = GenTicks.TicksAbs;
                for (int c = 0; c < comps.Count; c++)
                {
                    CompSprinkler _sprinkler = comps[c];
                    if (_sprinkler.Props.shouldSprinkleMotes)
                    {
                        if (_sprinkler.compPowerTrader.PowerOn && !_sprinkler.CurrentlySprinklingMotes && ticksAbs - _sprinkler.LastSprinkledMotesTick >= 57500L)
                        {
                            _sprinkler.StartSprinklingMotes();
                            for (int i = 0; i < affectedCells.Count; i++)
                            {
                                var cell = affectedCells[i];
                                var list = map.thingGrid.ThingsListAt(cell);
                                if (list.Count == 0 || !list.Any(b => b is Building building && exception.Contains(building.def.defName)))
                                {
                                    BoostPlantAt(cell);
                                }
                            }
                        }
                        else if (_sprinkler.CurrentlySprinklingMotes)
                        {
                            _sprinkler.SprinkleMotes();
                        }
                    }
                }
            }
        }
    }
}
