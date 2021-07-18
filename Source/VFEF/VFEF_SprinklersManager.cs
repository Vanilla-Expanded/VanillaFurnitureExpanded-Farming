using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace VFEF
{
    class VFEF_SprinklersManager : MapComponent
    {
        public VFEF_SprinklersManager(Map map) : base(map)
        {
        }

        private List<CompSprinkler> comps = new List<CompSprinkler>();

        private List<IntVec3> affectedCells = new List<IntVec3>();

        private List<string> exception = new List<string>()
        {
            "VFE_PlanterBox",
            "VFE_PlanterBox_Tilable"
        };

        public void Register(CompSprinkler c)
        {
            this.comps.Add(c);
        }

        public void Deregister(CompSprinkler c)
        {
            this.comps.Remove(c);
        }

        private void BoostPlantAt(IntVec3 cell)
        {
            Plant _plant = cell.GetPlant(map);
            if (_plant != null && !_plant.def.plant.IsTree) // No tree growth boost
            {
                _plant.Growth += 0.04f;
            }
        }

        public override void MapComponentTick()
        {
            if (GenLocalDate.HourOfDay(this.map) == 7)
            {
                foreach (CompSprinkler _sprinkler in this.comps)
                {
                    if (_sprinkler.parent.GetComp<CompPowerTrader>().PowerOn &&  !_sprinkler.CurrentlySprinklingMotes && (long)GenTicks.TicksAbs - _sprinkler.LastSprinkledMotesTick >= 57500L)
                    {
                        this.affectedCells.AddRange(_sprinkler.affectCells);
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

                this.affectedCells = this.affectedCells.Distinct().ToList(); // Remove duplicate cells

                foreach (IntVec3 cell in this.affectedCells)
                {
                    List<Thing> list = map.thingGrid.ThingsListAt(cell);
                    if (list.Count == 0)
                    {
                        this.BoostPlantAt(cell);
                    }
                    else if (!list.FindAll(b => b is Building building && building != null && building.def.altitudeLayer != AltitudeLayer.Conduits && !this.exception.Contains(building.def.defName)).Any())
                    {
                        this.BoostPlantAt(cell);
                    }
                }
                this.affectedCells.Clear(); // clear the cells for the next day calculation
            }
        }
    }
}
