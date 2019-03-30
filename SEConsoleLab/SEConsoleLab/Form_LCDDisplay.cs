using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;

using System.Drawing.Imaging;

using VRageMath;
using VRage.Game;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;



namespace SEConsoleLab
{
	public partial class Form_LCDDisplay : Form , IMyTextPanel
	{
		public Form_LCDDisplay()
		{
			InitializeComponent();
		}

		Vector2I moveStep = new Vector2I(1, 1);
		Vector2 SimulateVelocity = new Vector2(0, 0);

		public string Pic = "";
		SE_LCDProgramTest_20180904 TEST = new SE_LCDProgramTest_20180904();
		ITSTIMETODEBUGSOMETRUETHING_20190318_GIFTEST TEST20190318 = new ITSTIMETODEBUGSOMETRUETHING_20190318_GIFTEST();

		public bool IfFirstRun = true;

		public void PrintMe()
		{
			



			LCD_Refresh(Pic);
			//Thread.Sleep(200);
		}

		public class ITSTIMETODEBUGSOMETRUETHING_20190318_GIFTEST
		{


			public StringGenerateHelper.PicGenerator stuff = new StringGenerateHelper.PicGenerator();
			public string GIF_Save_Path = @"D:\Code-Half\SpaceengineerProjects\Labs\SEConsoleLab\LCDPICTexts\\GIF";

			public StringBuilder SB_ShowOnScreen = new StringBuilder();
			public StringBuilder SB_GIFRawString = new StringBuilder();

			public bool IfFirstRun = true;
			public string GIFResultString = "";

			public List<List<StringGenerateHelper.PicGenerator.PixelChange2>> L_Changes = new List<List<StringGenerateHelper.PicGenerator.PixelChange2>>();

			public SpaceEngineers.SEScriptGIFTEST20190318.SE_GIFPrinter gif = new SpaceEngineers.SEScriptGIFTEST20190318.SE_GIFPrinter();


			public int loopindex = 0;

			public void Initial()
			{
				stuff.GetImage();

				SB_ShowOnScreen = new StringBuilder(
					stuff.TargetImageSize.Height * (stuff.TargetImageSize.Width + 2));
				SB_GIFRawString = new StringBuilder(
					stuff.TargetImageSize.Height * (stuff.TargetImageSize.Width + 2));

				var fdm = new FrameDimension(stuff.targetImage.FrameDimensionsList[0]);
				stuff.targetImage.SelectActiveFrame(fdm, stuff.targetImage.GetFrameCount(fdm) - 1);

				SB_ShowOnScreen.Append(
					stuff.CSInstance.Get_StringPic(
						stuff.CSInstance.Get_ColorArray(
							stuff.CSInstance.Get_PicMidFillBackground(
								(Bitmap)stuff.targetImage, stuff.TargetImageSize, stuff.TargetImageBackgroundColor))));


				L_Changes = new List<List<StringGenerateHelper.PicGenerator.PixelChange2>>(stuff.GIFLength);

				
				SB_GIFRawString = stuff.GIF_Style1();          //OOOOOF GIF is comming


				//System.IO.StringWriter gifSaver = new System.IO.StringWriter(SB_GIFRawString);
				System.IO.File.WriteAllText(GIF_Save_Path + "1.txt"
					, SB_GIFRawString.ToString(), Encoding.UTF8);



				StringGenerateHelper.PicGenerator.PixelChange2 currentPixel;
				for(int i = 0; i < stuff.GIFLength; i++)
				{
					L_Changes.Add(new List<StringGenerateHelper.PicGenerator.PixelChange2>());
				}
				for(int i = 0, iMax = SB_GIFRawString.Length; i < iMax; i += 2)
				{
					currentPixel = new StringGenerateHelper.PicGenerator.PixelChange2(SB_GIFRawString[i], SB_GIFRawString[i + 1]);
					L_Changes[currentPixel.index].Add(currentPixel);
				}

				
			}
		

			
			public void LoopGIF()
			{
				if(IfFirstRun)
				{
					Initial();
					//gif.Initial(null, null, false);

					IfFirstRun = false;
				}

				
				foreach(var currentPixel in L_Changes[loopindex])
				{
					//int temp_index = currentPixel.Pos.X + (currentPixel.Pos.Y * (stuff.TargetImageSize.Width + 1));
					int temp_index = currentPixel.Pos;
					SB_ShowOnScreen[temp_index] = (char)((int)SB_ShowOnScreen[temp_index] + currentPixel.deltaColor);
				}
				loopindex++;
				if(stuff.GIFLength <= loopindex)
				{
					loopindex = 0;
					Console.Beep();
				}
			}


		}


		public void WorkMe()
		{





			if (IfFirstRun)
			{
				DrawANiceShape();
				IfFirstRun = false;
			}
			SimulateVelocity += new Vector2((float)(new Random().NextDouble() - 0.5) * 16,
				(float)(new Random().NextDouble() - 0.5) * 16);
			if (SimulateVelocity.X >= 127 || SimulateVelocity.X <= 0 || SimulateVelocity.Y >= 127 || SimulateVelocity.Y <= 0)
			{
				SimulateVelocity = new Vector2(0, 0);
			}
			
			#region DebugHide
			//while (true)
			//{


			if (TEST.NewRec.End.X >= 63 || TEST.NewRec.End.Y >= 63)
			{
				//moveStep = new Vector2I(-1, -1);
			}
			if (TEST.NewRec.Start.X <= 0 || TEST.NewRec.Start.Y <= 0)
			{
				//moveStep = new Vector2I(1, 1);
			}

			if (TEST.NewControl_CrossHair.End.X >= 140 || TEST.NewControl_CrossHair.End.Y >= 140)
			{
				moveStep = new Vector2I(-1, -1);
			}
			if (TEST.NewControl_CrossHair.Start.X <= 0 || TEST.NewControl_CrossHair.Start.Y <= 0)
			{
				moveStep = new Vector2I(1, 1);
			}
			/*for (int Y = 0; Y < 63; Y++)
			{
				for (int X = 0; X < 63; X++)
				{
					result += iloveSE[X, Y];
				}
				result += '\n';
			}*/
			//TEST.NewRec.OSMoveControl(moveStep);
			TEST.NewControl_CrossHair.OSMoveControlDelta(moveStep);
			//Thread.Sleep(2000);
			Pic = SE_LCDProgramTest_20180904.OSDisplay.Get_ColorString();

			//}
			#endregion
			



		}

		public void LCD_Refresh(string I_Picture)
		{
			string[] StepLine_String = I_Picture.Split('\n');

			Bitmap bitmap = new Bitmap(255, 255);

			int x = 0;
			int y = 0;
			foreach (var sstr in StepLine_String)
			{
				foreach (char schar in sstr)
				{
					bitmap.SetPixel(x, y, RevColorEx(sstr[x]));
					x++;
				}
				x = 0;
				y++;
			}
			LCD0.Image = bitmap;
		}
		

		private void Form_LCDDisplay_Load(object sender, EventArgs e)
		{
			TEST.Program();
			//WorkThread.Start();
			//PrintThread.Start();
		}

		public static System.Drawing.Color RevColorEx(char color)
		{
			if (color >= 0xE100 && color <= 0xE2FF)
			{
				color -= (char)0xE100;
				int R = (color & 0x1C0) >> 6;
				int G = (color & 0x038) >> 3;
				int B = color & 0x007;
				return System.Drawing.Color.FromArgb(R << 5, G << 5, B << 5);
			}
			else
			{
				
				//throw new Exception(string.Format("Color Error: \n{0}", color));
				return System.Drawing.Color.FromArgb(255, 0, 220);

			}


		}

		public void DrawANiceShape()
		{
			TEST.NewControl_CrossHair.L_TotalDots.Add(new SE_LCDProgramTest_20180904.OSDisplay.OSDot(new Vector2I(0, 3), '\uE200'));
			TEST.NewControl_CrossHair.L_TotalDots.Add(new SE_LCDProgramTest_20180904.OSDisplay.OSDot(new Vector2I(1, 3), '\uE200'));
			TEST.NewControl_CrossHair.L_TotalDots.Add(new SE_LCDProgramTest_20180904.OSDisplay.OSDot(new Vector2I(2, 3), '\uE200'));
			TEST.NewControl_CrossHair.L_TotalDots.Add(new SE_LCDProgramTest_20180904.OSDisplay.OSDot(new Vector2I(3, 2), '\uE200'));
			TEST.NewControl_CrossHair.L_TotalDots.Add(new SE_LCDProgramTest_20180904.OSDisplay.OSDot(new Vector2I(4, 1), '\uE200'));
			TEST.NewControl_CrossHair.L_TotalDots.Add(new SE_LCDProgramTest_20180904.OSDisplay.OSDot(new Vector2I(5, 2), '\uE200'));
			TEST.NewControl_CrossHair.L_TotalDots.Add(new SE_LCDProgramTest_20180904.OSDisplay.OSDot(new Vector2I(6, 3), '\uE200'));
			TEST.NewControl_CrossHair.L_TotalDots.Add(new SE_LCDProgramTest_20180904.OSDisplay.OSDot(new Vector2I(7, 3), '\uE200'));
			TEST.NewControl_CrossHair.L_TotalDots.Add(new SE_LCDProgramTest_20180904.OSDisplay.OSDot(new Vector2I(8, 3), '\uE200'));
			TEST.NewControl_CrossHair.OSCreate();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			TEST20190318.LoopGIF();
			LCD_Refresh(TEST20190318.SB_ShowOnScreen.ToString());
			timer1.Enabled = true;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			TEST20190318.LoopGIF();
			LCD_Refresh(TEST20190318.SB_ShowOnScreen.ToString());
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			
		}

		public bool WritePublicText(string value, bool append = false)
		{
			LCD_Refresh(value);
			return true;
			//throw new NotImplementedException();
		}

		public string GetPublicText()
		{
			throw new NotImplementedException();
		}
		
	}
}

//Known issues:

//20190320  index溢出问题
//由于字符大小限制，二阶char delta取值为[1, FFFF]，其中deltaColor占10个位
//index占6个位，取值为[0, 7F] ([0, 127])
//所以当gif图像超过（或包含）128个帧时，
//index溢出，造成部分图像像素损坏
//修改建议1：将index改为帧切换定位符，当需要切换帧时将index赋值为1，此外为0
//修改建议2：增加一个字符用于储存index
