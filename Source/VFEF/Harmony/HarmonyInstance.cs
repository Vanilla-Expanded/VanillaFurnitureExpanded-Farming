using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using System.Reflection.Emit;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Verse.AI;
using RimWorld.Planet;



namespace VFEF
{

    public class VFEF : Mod
    {
        public VFEF(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("com.vanillafarmingexpanded");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

}
















