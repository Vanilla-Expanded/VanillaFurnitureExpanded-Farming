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
            Plant _plant = cell.GetPlant(map);
            if (_plant != null && !_plant.def.plant.IsTree) // No tree growth boost
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
                for (int i = 0; i < comps.Count; i++)
                {
                    CompSprinkler _sprinkler = comps[i];
                    if (_sprinkler.parent.GetComp<CompPowerTrader>().PowerOn && !_sprinkler.CurrentlySprinklingMotes && GenTicks.TicksAbs - _sprinkler.LastSprinkledMotesTick >= 57500L)
                    {
                        if (_sprinkler.Props.shouldSprinkleMotes)
                        {
                            _sprinkler.StartSprinklingMotes();
                        }
                    }
                    else if (_sprinkler.Props.shouldSprinkleMotes && _sprinkler.CurrentlySprinklingMotes)
                    {
                        _sprinkler.SprinkleMotes();
                    }
                }

                for (int i = 0; i < affectedCells.Count; i++)
                {
                    IntVec3 cell = affectedCells[i];
                    List<Thing> list = map.thingGrid.ThingsListAt(cell);
                    if (list.Count == 0)
                    {
                        BoostPlantAt(cell);
                    }
                    else if (!list.FindAll(b => b is Building building && building != null && building.def.altitudeLayer != AltitudeLayer.Conduits && !exception.Contains(building.def.defName)).Any())
                    {
                        BoostPlantAt(cell);
                    }
                }
            }
        }
    }
}
