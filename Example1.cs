#region
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using VRageMath;
using VRage.Game;
using VRage.Collections;
using Sandbox.ModAPI.Ingame;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
#endregion

namespace SpaceEngineers
{ 
    public sealed class Example1 : MyGridProgram
    {
		//Basic Hello World Program that echo Hello World
		//Also using a lcd panel to show program args.

		bool IfFirstRun = true;
		string LCDTotalString = "";
		string[] LCDGIFStringArray;
		int Index = 0;
		int IndexMax = 0;

        void Main(string args)
        {
			Echo("Hellow World");

            IMyTextPanel LCD = GridTerminalSystem.GetBlockWithName("LCDW") as IMyTextPanel;
			if (IfFirstRun)
			{
				LCDTotalString = LCD.GetPublicText();
				
				LCDGIFStringArray = LCDTotalString.Split('*');
				IndexMax = LCDGIFStringArray.Length;
				IfFirstRun = false;
			}


			LCD.WritePublicText(LCDGIFStringArray[Index]);
			Index++;
			if (Index >= IndexMax)
				Index = 0;
            Echo(Index.ToString());
			Echo(IfFirstRun.ToString());
            return;
        }
		
		
    }
}

