using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Windows.Forms;

/*
using VRage.Game;
using VRageMath;
using VRage.Scripting;
*/

using VRageMath;
using VRage.Game;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;



namespace SEConsoleLab
{
	public class Program
	{

		//控制台
		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		public static extern Boolean AllocConsole();
		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		public static extern Boolean FreeConsole();

		[STAThread]
		static void Main(string[] args)
		{
			/*
			char tempc = SE_LCDProgramTest_20180904.OSDisplay.Color(1, 2, 3);
			int R, G, B = 0;
			SE_LCDProgramTest_20180904.OSDisplay.RevColor(tempc, out R, out G, out B);

			Console.WriteLine(String.Format("R:{0}, G:{1}, B:{2}", R, G, B));			//测试RGB输出是否正常
			*/

			//SE_LCDProgramTest_20180904.OSDisplay.OSCharSet oscs = new SE_LCDProgramTest_20180904.OSDisplay.OSCharSet(Vector2I.Zero,Vector2I.Zero,'0');
			//oscs.InitialCharSet_HZK16("一￿￿￿￿￿￿翾￿￿￿￿￿￿￿￿乙￿￰`Àƀ̀؀ఀ᠀　 怄䀄怆㿼￿二￿㿸￿￿￿￿￿￿￿￿翾￿￿￿");
			//var obj = new SE_LCDProgramTest_20180904.OSDisplay.OSDot(new Vector2I(0,0), '\0');

			//return;

			/*
			StringGenerateHelper.PicGenerator SGHPG = new StringGenerateHelper.PicGenerator();
			SGHPG.GetImage();
			string result = SGHPG.GIF_Style1().ToString();
			*/


			ProjectReflectionTestHelper PRTH = new ProjectReflectionTestHelper();
			PRTH.Ini();
			return;


			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form_LCDDisplay());
			

			/*
			TEST.Program();
			TEST.Main();
			WorkThread.Start();
			Console.ReadKey();
			*/
		}

		static void WorkMe()
		{
			Vector2I moveStep = new Vector2I(1, 1);
			while (true)
			{
				if (TEST.NewRec.End.X >= 63 || TEST.NewRec.End.Y >= 63)
				{
					moveStep = new Vector2I(-1, -1);
				}
				if (TEST.NewRec.Start.X <= 0 || TEST.NewRec.Start.Y <= 0)
				{
					moveStep = new Vector2I(1, 1);
				}

				
				Console.Clear();
				/*for (int Y = 0; Y < 63; Y++)
				{
					for (int X = 0; X < 63; X++)
					{
						result += iloveSE[X, Y];
					}
					result += '\n';
				}*/
				Console.Write(SE_LCDProgramTest_20180904.OSDisplay.Get_ColorString());
				TEST.NewRec.OSMoveControlDelta(moveStep);
				
				Thread.Sleep(200);
			}
		}

		static Thread WorkThread = new Thread(WorkMe);
		static SE_LCDProgramTest_20180904 TEST = new SE_LCDProgramTest_20180904();
	}
}
