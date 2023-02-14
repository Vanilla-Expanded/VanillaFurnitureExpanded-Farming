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

        private readonly HashSet<IntVec3> allAffectedCells = new HashSet<IntVec3>();
        private readonly Dictionary<CompSprinkler, List<IntVec3>> affectedCells = new Dictionary<CompSprinkler, List<IntVec3>>();

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
            // Clear everything
            affectedCells.Clear();
            allAffectedCells.Clear();
            // Recache
            for (int i = 0; i < comps.Count; i++)
            {
                var comp = comps[i];
                var affect = comp.affectCells;

                affectedCells.Add(comp, new List<IntVec3>());
                for (int o = 0; o < affect.Count; o++)
                {
                    // If cell affected by comp is not already affected by another one
                    var cell = affect[o];
                    if (!allAffectedCells.Contains(cell))
                        affectedCells[comp].Add(cell);
                }
                // Add all the cell affected
                allAffectedCells.AddRange(affectedCells[comp]);
            }
        }

        public override void MapComponentTick()
        {
            if (GenLocalDate.HourOfDay(map) == 7)
            {
                var ticksAbs = GenTicks.TicksAbs;
                for (int c = 0; c < comps.Count; c++)
                {
                    var _sprinkler = comps[c];
                    if (_sprinkler.Props.shouldSprinkleMotes)
                    {
                        if (_sprinkler.compPowerTrader.PowerOn && !_sprinkler.CurrentlySprinklingMotes && ticksAbs - _sprinkler.LastSprinkledMotesTick >= 57500L)
                        {
                            _sprinkler.StartSprinklingMotes();
                            var cells = affectedCells[_sprinkler];

                            for (int i = 0; i < cells.Count; i++)
                            {
                                var cell = cells[i];
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
