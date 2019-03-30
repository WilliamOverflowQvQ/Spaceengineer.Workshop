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


//using IMyTextPanel = SEConsoleLab.IMyTextPanel;

namespace SpaceEngineers
{
	public sealed class SEScriptGIFTEST20190318 : MyGridProgram
	{
		public bool IfFirstRun = true;
		public IMyTextPanel LCD;
		public List<string> L_PicString = new List<string>(50);

		public int currentPicIndex = 0;

		public SE_GIFPrinter GIF = new SE_GIFPrinter();
		public bool IfGIFInitialSync = false;

		public void Main()
		{
			if(IfFirstRun)
			{
				LCD = GridTerminalSystem.GetBlockWithName("LCD") as IMyTextPanel;
				//L_PicString = LCD.CustomData.Split('*').ToList();

				IfGIFInitialSync = GIF.Initial(LCD, this, false);
				IfFirstRun = false;
			}
			if(IfGIFInitialSync)
			{
				IfGIFInitialSync = GIF.Initial(LCD, this, true);
			}
			
			GIF.LoopGIF();
			currentPicIndex++;
			if(L_PicString[currentPicIndex] == "" || currentPicIndex >= GIF.totalGIF)
			{
				currentPicIndex = 0;
			}
			
		}

		public class SE_GIFPrinter
		{
			public IMyTextPanel LCD;
			public MyGridProgram CurrentScript;

			public StringBuilder GIFRawString = new StringBuilder(0xFFFF);
			public StringBuilder ScreenRawString = new StringBuilder(0xFFFF);

			public List<List<PixelChange2>> L_Changes = new List<List<PixelChange2>>();

			public int syncLastInitialStep = 0;
			public int syncLastInitialGIFRawStringStep = 0;
			public int singleStepMaxInstructionCount = 40000;//40_000;
			public bool ifCheckInstructionCount = true;

			public int currentGIF = 0;
			public int totalGIF = 1;
			public bool ifRepeat = true;


			public bool Initial(IMyTextPanel I_lcd, MyGridProgram I_currentScript, bool syncInitial = false)
			{
				if(!syncInitial)
				{
					LCD = I_lcd;
					CurrentScript = I_currentScript;
					GIFRawString = new StringBuilder(I_lcd.CustomData);
					ScreenRawString = new StringBuilder(I_lcd.GetPublicText());

					syncLastInitialStep = 0;
					syncLastInitialGIFRawStringStep = 0;

					currentGIF = 0;
					totalGIF = 1;

					L_Changes.Clear();

					
					totalGIF = new PixelChange2(
					GIFRawString[GIFRawString.Length - 2],
					GIFRawString[GIFRawString.Length - 1]).index + 1;

					
					for(int ig = 0; ig < totalGIF; ig++)
					{
						L_Changes.Add(new List<PixelChange2>());
					}

					
					if(GIFRawString.Length / 2 >= singleStepMaxInstructionCount)
					{
						ifCheckInstructionCount = true;
					}
					
				}

				int spareInstructionCount = singleStepMaxInstructionCount - CurrentScript.Runtime.CurrentInstructionCount;
				if(spareInstructionCount <= 0)
				{
					return true;
				}
				int iInstructionLimit = syncLastInitialGIFRawStringStep + spareInstructionCount;



				int i = syncLastInitialGIFRawStringStep;
				int iInstructionLimitMax = syncLastInitialGIFRawStringStep + spareInstructionCount;
				int iTotalPixelChanges = GIFRawString.Length / 2;
				int iMax = iTotalPixelChanges > iInstructionLimitMax ? iInstructionLimitMax : iTotalPixelChanges;

				

				PixelChange2 temp_storage = new PixelChange2();

				

				if(i + 1 == iTotalPixelChanges)
				{
					return false;
				}

				
				//只在大量费时工作启用sync检查
				for(/* :D */; i < iMax; i += 2)
				{
					temp_storage = new PixelChange2(GIFRawString[i], GIFRawString[i + 1]);
					L_Changes[temp_storage.index].Add(temp_storage);
				}
				syncLastInitialGIFRawStringStep = i;

				CurrentScript.Echo("Finished");
				return true;

			}

			public void LoopGIF()
			{
				CurrentScript.Echo("Inside Loop" + L_Changes[currentGIF].Count.ToString());
				foreach(var x in L_Changes[currentGIF])
				{
					CurrentScript.Echo(string.Format("[{0}], {1} @ {2} Stage 00", x.Pos, x.deltaColor, x.index));
					ScreenRawString[x.Pos] = (char)(x.deltaColor + ScreenRawString[x.Pos]);
					CurrentScript.Echo(string.Format("[{0}], {1} @ {2} Stage 01", x.Pos, x.deltaColor, x.index));
				}
				LCD.WritePublicText(ScreenRawString.ToString());
			}

			//[Obsolete]
			public class PixelChange
			{
				public PixelChange()
				{

				}

				public PixelChange(char I_pos, char I_char)
				{
					Pos = new Vector2I((I_pos & 0xFF) - 1, (I_pos >>= 8) & 0xFF);
					deltaColor = ((I_char & 0x3FF) - 0x200);
					index = (I_char >>= 10);
				}


				public Vector2I Pos;               //注意，此处X值域：[0,254]，Y值域：[0,255]
				public int deltaColor { get; set; } //deltaColor取值[-511, 511]
				public int index { get; set; }		//

				//[Obsolete]
				public override string ToString()
				{
					char pos = (char)(((Pos.Y & 0xFF) << 8) + ((Pos.X + 1) & 0xFF));        //X + 1以预防null，但这也导致横轴缺少一像素
					char delta = (char)(((deltaColor + 0x200) & 0x3FF) + (index << 10));                //C#的char好渣啊... ...
					return new string(new char[] { pos, delta });
					//YYYYYYYY XXXXXXXX  IIIIIICC CCCCCCCC
					//Y最小值0 X最小值1  I最小值1 C最小值0
					//想要遍历的时候直接把pos++就可以依次遍历
				}

			}

			public class PixelChange2
			{
				public PixelChange2()
				{

				}

				public PixelChange2(char I_pos, char I_char)
				{
					Pos = (int)I_pos - 1;
					deltaColor = ((I_char & 0x3FF) - 0x200);
					index = (I_char >> 10);
				}


				public int Pos { get; set; }		//注意，Pos大小取值[0, 65534] ([0, FFFE])
				public int deltaColor { get; set; } //deltaColor取值[-511, 511]
				public int index { get; set; }      //GIF页码

				//[Obsolete]
				public override string ToString()
				{
					char pos = (char)(Pos + 1);											//P + 1以预防null
					char delta = (char)(((deltaColor + 0x200) & 0x3FF) + (index << 10));//C#的char好渣啊... ...
					return new string(new char[] { pos, delta });
					//PPPPPPPP PPPPPPPP  IIIIIICC CCCCCCCC
					//P is [1, FFFF]     I最小值1 C最小值0
					//想要遍历的时候直接把pos++就可以依次遍历
				}
			}
		}
	}
}