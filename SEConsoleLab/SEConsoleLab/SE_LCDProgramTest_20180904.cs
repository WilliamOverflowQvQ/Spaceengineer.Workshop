
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
	public class SE_LCDProgramTest_20180904
	{
		
		public const string GroupName_ProgrammableBlock = "G_FCOS";                    //编程块的组名

		public void Program()
		{
			OSDisplay.OForeColor = '\uE100';
			OSDisplay.OLineColor = '\uE100';
			OSDisplay.OBackgroundColor = '\uE2FF';
			OSDisplay.OSInitialDisplay();
			OSDisplay.OSCheckInitialSafety();
		}

		public OSDisplay.OSRec NewRec = new OSDisplay.OSRec(new Vector2I(0, 0), new Vector2I(16, 16), '\uE200');

		public OSDisplay.OSControl NewControl_CrossHair = new OSDisplay.OSControl(new Vector2I(0, 0), new Vector2I(8, 8));

		public OSDisplay.OSLine NewVelocity_Line = new OSDisplay.OSLine(new Vector2I(63, 63), new Vector2I(63, 63), '\uE100');


		public void Main()
		{
			
			Console.WriteLine("Yooo");
			
			NewRec.OSCreate();
			
		}

		public OSDisplay osd = new OSDisplay();

		public class OSDisplay
		{
			/* 说明：
			 * 只是一个新的LCD显示算法，使用图形类传导绘图与优先渲染原理
			 * 可以在不太大影响显示的情况下大量刷新异步图像，且本算法优化了动态图形算法，
			 * 使得代码更简洁
			 * 
			 * 传导绘图：在一个像素由于某个图形的变换被修改的同时，该像素的颜色被顺推至下一个图形的颜色
			 * 避免了大面积刷新
			 * 优先渲染：优先更新动态的、重要的图形，
			*/

			//Color = 0x100 ~0x2FF -> 0x100 + 1 * + 8 * + 64 *
			public const int ODisplayWidth = 127;
			public const int ODisplayHeight = 127;

			public static Vector2I ODisplayZero = Vector2I.Zero;
			public static Vector2I ODisplayMax = new Vector2I(ODisplayWidth, ODisplayWidth);
			public static Vector2I ODisplayArrayMax = new Vector2I(ODisplayWidth - 1, ODisplayWidth - 1);
			
			public static char OForeColor = '\uE200';
			public static char OBackgroundColor = '\uE2FF';
			public static char OLineColor = '\uE120';


			//在向像素点集合加入像素点时，使用List.Add(item, 0);来加到最前面，否则无法被显示。
			//亦可使用List.OrderBy()的排序函数来实现。
			//按照List.Add(OSDot)的方法按先后（或使用显示优先级）加入点集
			//在加入完成后使用
			//public static List<OSDot> L_TempDots = new List<OSDot>();   //优先显示的点集（最后加入，例如HUD等...）
			//public static List<OSDot> L_StaticDots = new List<OSDot>(); //最后显示的点集（最先加入，例如边线等...）
			public static List<OSDot> L_BackGroundDots = new List<OSDot>(); //背景像素点集（始终不变，不允许修改可以用于创建空集）

			public static OSPixel[,] Array_PixelSet = new OSPixel[ODisplayWidth, ODisplayHeight];

			//public static List<OSDot> L_RenderList = new List<OSDot>(); //即将被载入的所有点（被加入的点）
			public static char[,] Array_Render = new char[ODisplayWidth, ODisplayHeight];

			public static List<OSControl> L_ControlList = new List<OSControl>();


			public static IMyGridProgramRuntimeInfo OSRuntime;


			public static void OSInitialDisplay()
			{
				L_ControlList = new List<OSControl>();

				//L_TempDots = new List<OSDot>();
				//L_StaticDots = new List<OSDot>();
				//L_BackGroundDots = new List<OSDot>();
				//填充背景
				for (int Y = 0; Y < ODisplayHeight; Y++)
				{
					for (int X = 0; X < ODisplayWidth; X++)
					{
						L_BackGroundDots.Add(new OSDot(new Vector2I(X, Y), OBackgroundColor, true));
						Array_PixelSet[X, Y] = new OSPixel(new Vector2I(X, Y), L_BackGroundDots[L_BackGroundDots.Count - 1]);

					}
				}
			}

			public static int OSCheckInitialSafety()
			{
				int result = 0;
				if (OSRuntime == null)
				{
					result += 1;
				}
				
				return result;
			}

			public static char[,] Get_ColorArray()
			{
				char[,] result = new char[ODisplayWidth, ODisplayHeight];
				for (int X = 0; X < ODisplayWidth; X++)
				{
					for (int Y = 0; Y < ODisplayHeight; Y++)
					{
						result[X, Y] = Array_PixelSet[X, Y].L_DotSet[0].color;
					}
				}
				return result;
			}

			public static string Get_ColorString()
			{
				StringBuilder StepString = new StringBuilder((ODisplayWidth + 1) * ODisplayHeight);
				for (int Y = 0; Y < ODisplayHeight; Y++)
				{
					for (int X = 0; X < ODisplayWidth; X++)
					{
						StepString.Append(Array_PixelSet[X, Y].L_DotSet[0].color);
					}
					StepString.Append('\n');
				}
				return StepString.ToString();
			}


			//Color = 0x100 ~0x2FF -> 0x100 + 1 * + 8 * + 64 *
			public static char Color(int R, int G, int B)
			{
				return (char)(0xE100 + (R << 6) + (G << 3) + B);
			}

			public static bool RevColor(char color, out int R, out int G, out int B)
			{
				if (color >= 0xE100 && color <= 0xE2FF)
				{
					color -= (char)0xE100;
					R = (color & 0x1C0) >> 6;
					G = (color & 0x038) >> 3;
					B = color & 0x007;
					return true;
				}
				R = 0;
				G = 0;
				B = 0;
				return false;
			}

			public static bool OSCheckDotIfEdge(Vector2I I_pos)
			{
				return I_pos.X >= ODisplayZero.X && I_pos.X <= ODisplayArrayMax.X && I_pos.Y >= ODisplayZero.Y && I_pos.Y <= ODisplayArrayMax.Y;
			}

			public class OSPixel
			{
				public List<OSDot> L_DotSet = new List<OSDot>();
				public Vector2I pos = new Vector2I(0, 0);

				public OSPixel(Vector2I I_pos, OSDot I_backgroundDot)
				{
					pos = I_pos;
					L_DotSet.Add(I_backgroundDot);
				}
			}

			public class OSDot
			{
				public bool RequestRemove = false;
				public bool Display = true;

				public OSPixel CurrentParent;
				public Vector2I pos = new Vector2I(0, 0);
				public char color = OBackgroundColor;
				

				public OSDot(Vector2I I_pos, char I_color, bool I_IfDisplay = true)
				{
					pos = I_pos;
					color = I_color;
					Display = I_IfDisplay;
					CurrentParent = Array_PixelSet[I_pos.X, I_pos.Y];
				}

				public void OSMoveDot(Vector2I I_target)
				{
					pos = I_target;
					if (CurrentParent != null)
						CurrentParent.L_DotSet.Remove(this);
					if (!OSCheckDotIfEdge(I_target))
					{
						CurrentParent = null;
						return;
					}
					CurrentParent = Array_PixelSet[I_target.X, I_target.Y];
					CurrentParent.L_DotSet.Insert(0, this);
				}

				

			}




			public class OSControl
			{
				public Vector2I Start = new Vector2I(0, 0);
				public Vector2I Size = new Vector2I(0, 0);

				public char ForeColor = OForeColor;
				public List<OSDot> L_TotalDots = new List<OSDot>();

				public Vector2I End
				{
					get
					{
						return (Size + Start);
					}
				}
				public Vector2 Middle
				{
					get
					{
						return (new Vector2(Size.X, Size.Y) / 2 + Start);
					}
				}



				/// <summary>
				/// 创建OSControl控件类，使用起始端与控件大小进行初始化，使用默认的填充函数
				/// 不进行位置错误检测
				/// </summary>
				/// <param name="I_start">起始端，应为左上</param>
				/// <param name="I_size">大小</param>
				public OSControl(Vector2I I_start, Vector2I I_size, char I_foreColor)
				{
					Start = I_start;
					Size = I_size;
					ForeColor = I_foreColor;
				}

				/// <summary>
				/// 用于子类的构造函数，使用起始端与终端进行初始化，不能使用默认填充函数
				/// </summary>
				/// <param name="I_start">起始端，应为左上</param>
				/// <param name="I_end">终端，应为右下</param>
				public OSControl(Vector2I I_start, Vector2I I_end)
				{
					Start = I_start;
					Size = I_end - I_start;
					ForeColor = OForeColor;
				}

				
				//IOSDrawable
				public virtual void OSCreate()
				{
					foreach (OSDot sd in L_TotalDots)
					{
						if (!OSCheckDotIfEdge(sd.pos))
							continue;
						sd.CurrentParent.L_DotSet.Insert(0, sd);
					}
				}

				public virtual void OSHide()
				{
					L_TotalDots.ForEach(sd => sd.Display = false);
				}

				public virtual void OSShow(bool I_IfDisplay = true)
				{
					L_TotalDots.ForEach(sd => sd.Display = I_IfDisplay);
				}

				public virtual void OSRequestRemove()
				{
					L_TotalDots.ForEach(sd => sd.RequestRemove = true);
				}

				public virtual void OSMoveControlDelta(Vector2I I_deltaSwift)
				{
					Start += I_deltaSwift;
					L_TotalDots.ForEach(sd =>
					{
						sd.OSMoveDot(sd.pos + I_deltaSwift);
					});
				}

				//IOSInterfaceable
				public virtual void IOSI_MouseAbove()
				{

				}

				public virtual void IOSI_MouseClick()
				{

				}

				public virtual void IOSI_KeyDown()
				{

				}

				public virtual void IOSI_KeyUp()
				{

				}
				
			}

			public class OSLine : OSControl
			{
				public OSLine(Vector2I I_start, Vector2I I_end, char I_color) : base(I_start, I_end)
				{
					ForeColor = I_color;
					Vector2I deltaV2I = I_end - I_start;
					List<OSDot> L_Result = new List<OSDot>();
					
					if (deltaV2I.X == 0)    // |
					{
						//L_TotalDots.AddRange(L_BackGroundDots.Range(0, deltaV2I.Y));
						for (int yStep = I_start.Y, yMax = I_end.Y, xSwift = I_start.X; yStep < yMax; yStep++)
						{
							//L_TotalDots[yStep - yStart] = new OSDot(new Vector2I(xSwift, yStep), I_color);
							L_TotalDots.Add(new OSDot(new Vector2I(xSwift, yStep), ForeColor));
						}
					}
					if (deltaV2I.Y == 0)    // -
					{
						//L_TotalDots.AddRange(L_BackGroundDots.Range(0, deltaV2I.X));
						for (int xStep = I_start.X, xMax = I_end.X, ySwift = I_start.Y; xStep < xMax; xStep++)
						{
							//L_TotalDots[xStep - xStart] = new OSDot(new Vector2I(xStep, ySwift), I_color);
							L_TotalDots.Add(new OSDot(new Vector2I(xStep, ySwift), ForeColor));
						}
					}
					else
					{                       // \
						float slope = (float)deltaV2I.Y / (float)deltaV2I.X;
						if (slope < 1)
						{   //斜率小的按X遍历
							//L_TotalDots.AddRange(L_BackGroundDots.Range(0, deltaV2I.X));
							for (int xStep = 0, xMax = deltaV2I.X, xSwift = I_start.X, ySwift = I_start.Y; xStep < xMax; xStep++)
							{
								//L_TotalDots[xStep] = new OSDot(new Vector2I((int)(xStep * slope + xSwift), ySwift), I_color);
								L_TotalDots.Add(new OSDot(new Vector2I(xStep + xSwift, (int)((float)xStep * slope + ySwift)), ForeColor));
							}
						}
						else
						{   //斜率大的就按Y遍历吧
							//L_TotalDots.AddRange(L_BackGroundDots.Range(0, deltaV2I.Y));
							for (int yStep = 0, yMax = deltaV2I.Y, xSwift = I_start.X, ySwift = I_start.Y; yStep < yMax; yStep++)
							{
								//L_TotalDots[yStep] = new OSDot(new Vector2I(xSwift, (int)(yStep / slope + ySwift)), I_color);
								L_TotalDots.Add(new OSDot(new Vector2I((int)((float)yStep / slope + xSwift), yStep + ySwift), ForeColor));
							}
							
						}
					}
				}
			}

			public class OSRec : OSControl
			{				
				public OSRec(Vector2I I_start, Vector2I I_end, char I_color) : base(I_start, I_end)
				{
					//Fills
					int XLength = I_end.X - I_start.X;
					int YLength = I_end.Y - I_start.Y;

					
					//L_FillDots.AddRange(L_BackGroundDots.Range(0, XLength * YLength));

					for (int X = I_start.X, xMax = I_end.X; X < xMax; X++)
					{
						for (int Y = I_start.Y, yMax = I_end.Y; Y < yMax; Y++)
						{
							L_TotalDots.Add(new OSDot(new Vector2I(X, Y), ForeColor));
						}
					}
				}
			}

			public class OSRecFrame : OSControl
			{
				public List<OSLine> L_FrameLines = new List<OSLine>();
				
				//Lines
				public OSRecFrame(Vector2I I_start, Vector2I I_end, char I_color) : base(I_start, I_end)
				{
					ForeColor = I_color;

					//Fills
					int XLength = I_end.X - I_start.X;
					int YLength = I_end.Y - I_start.Y;


					//L_FillDots.AddRange(L_BackGroundDots.Range(0, XLength * YLength));
					L_FrameLines.Add(new OSLine(I_start, new Vector2I(I_end.X, I_start.Y), ForeColor)); // ~
					L_FrameLines.Add(new OSLine(new Vector2I(I_end.X, I_start.Y), I_end, ForeColor));   //  |
					L_FrameLines.Add(new OSLine(new Vector2I(I_start.X, I_end.Y), I_end, ForeColor));   // _
					L_FrameLines.Add(new OSLine(I_start, new Vector2I(I_start.X, I_end.Y), ForeColor)); //|

					L_TotalDots.AddRange(L_FrameLines[0].L_TotalDots);
					L_TotalDots.AddRange(L_FrameLines[1].L_TotalDots);
					L_TotalDots.AddRange(L_FrameLines[2].L_TotalDots);
					L_TotalDots.AddRange(L_FrameLines[3].L_TotalDots);
				}
			}

			public class OSGIF : OSControl
			{
				

				public OSGIF(Vector2I I_start, Vector2I I_size, char I_foreColor, string I_gifIni) : base(I_start, I_size, I_foreColor)
				{

				}

				public bool UpdateGIF(string I_gifIni)
				{
					return false;
				}
			}

			public class OSPic : OSControl
			{
				public string picRaw = "";
				public ENUMPicDisplayMtd PicDisplayMtd = 0;
				public ENUMPicConvertMtd PicConvertMtd = 0;

				public enum ENUMPicDisplayMtd : int
				{
					WaitHold = 0,
				}

				
				public enum ENUMPicConvertMtd : byte
				{
					WaitHold = 0,
					FrameSyncClassic = 1,		//经典帧集合解析法
					PixelLength = 2,			//将图片降维衔接后按同色像素的长度进行解析
					PixelPos = 4,				//按照给出的像素点的位置来进行解析，忽略空像素
					PixelPosF = 8,              //按照给出的像素点的位置来进行解析，图片大小中的空像素填充为图片的ForeColor
				}

				public OSPic(Vector2I I_start, Vector2I I_size, char I_foreColor, string I_picIni) : base(I_start, I_size, I_foreColor)
				{
					L_TotalDots = new List<OSDot>(I_size.X * I_size.Y);
				}

				public bool UpdatePic(string I_picIni)
				{
					string picInfo = I_picIni.Substring(0, 4);  //原picIni的前4个字符是图片信息单元
					PicConvertMtd = (ENUMPicConvertMtd)(byte)picInfo[0];
					PicDisplayMtd = (ENUMPicDisplayMtd)(byte)picInfo[1];
					switch (PicConvertMtd)
					{
						case ENUMPicConvertMtd.WaitHold:
							break;
						case ENUMPicConvertMtd.FrameSyncClassic:
							break;
						case ENUMPicConvertMtd.PixelLength:
							break;
						case ENUMPicConvertMtd.PixelPos:
							break;
						case ENUMPicConvertMtd.PixelPosF:
							break;
						default:
							return false;
					}
					return false;
				}
			}

			public class OSCharSet : OSControl
			{
				//public const int CharSetCharLength = 0;
				public Dictionary<char, int> Dic_CharIndex = new Dictionary<char, int>();
				public string RawCharSetString = "";

				public OSCharSet(Vector2I I_start, Vector2I I_size, char I_foreColor) : base(I_start, I_size, I_foreColor)
				{


				}

				public List<OSDot> DrawChar_HZK16(char I_char, Vector2I I_start)
				{
					List<OSDot> L_ResultDot = new List<OSDot>(256);
					int RawCharStartIndex = Dic_CharIndex[I_char];
					char RawCharOnGoing = '\0';
					for (int y = 0; y < 16; y++)
					{
						RawCharOnGoing = RawCharSetString[I_char + 1 + y];
						for (int x = 0; y < 16; y++)
						{

						}
					}
					return null;
				}

				public int InitialCharSet_HZK16(string I_RawString)
				{
					//HZK16
					RawCharSetString = I_RawString;
					Dic_CharIndex.Clear();
					int CharRawIndexMax = I_RawString.Length;
					int CharIndexMax = CharRawIndexMax / 17;
					Dic_CharIndex = new Dictionary<char, int>(CharIndexMax + 4);	//多留4个用于错误处理等
					for (int CharIndex = 0; CharIndex < CharIndexMax; CharIndex++)
					{
						Dic_CharIndex.Add(I_RawString[CharIndex * 17], CharIndex);
					}
					GC.Collect(0);													//触发垃圾回收，减少内存损耗
					return CharIndexMax;
				}


			}


			public class OSForm : OSControl
			{
				public OSForm(Vector2I I_start, Vector2I I_end, char I_fillColor, char I_lineColor) : base(I_start, I_end)
				{

				}


			}

			

			#region TextDisplays
			

			#endregion
		}
		

	}
}