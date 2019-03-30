
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Windows.Forms;

using System.Threading;
using System.Drawing.Imaging;

namespace SEConsoleLab
{
	public class ConvertSupport
	{

		#region AdvancePicControls

		#endregion

		#region ToolFunctions
		public Color[,] Get_ColorArray(Bitmap I_TargetBitmap)
		{
			int TargetBitmap_X = I_TargetBitmap.Width;
			int TargetBitmap_Y = I_TargetBitmap.Height;

			Color[,] Result = new Color[TargetBitmap_X, TargetBitmap_Y];

			for(int iY = 0; iY < TargetBitmap_Y; iY++)
			{
				for(int iX = 0; iX < TargetBitmap_X; iX++)
				{
					Result[iX, iY] = I_TargetBitmap.GetPixel(iX, iY);
				}
			}
			return Result;
		}

		public string Get_StringPic(Color[,] I_ColorArray)
		{
			StringBuilder Result = new StringBuilder(I_ColorArray.Length);
			for(int iY = 0, Max_Y = I_ColorArray.GetLength(1); iY < Max_Y; iY++)
			{
				for(int iX = 0, Max_X = I_ColorArray.GetLength(0); iX < Max_X; iX++)
				{
					Result.Append(Get_SEColorChar(I_ColorArray[iX, iY]));
				}
				Result.Append('\n');
			}
			return Result.ToString();
		}

		public Bitmap Get_FillRecWithColor(Color I_Color, Size I_Size)
		{
			Bitmap NewRec = new Bitmap(I_Size.Width, I_Size.Height);
			Graphics Layout = Graphics.FromImage((Image)NewRec);
			Layout.FillRectangle(new SolidBrush(I_Color), new Rectangle(0, 0, I_Size.Width, I_Size.Height));
			return NewRec;
		}

		public Bitmap Get_ResizedPic(Bitmap I_OriginBitmap, int I_NewWidth, int I_NewHeight)    //我知道重复了，别说了，我懒得改而已blablabla
		{
			return new Bitmap(I_OriginBitmap, new Size(I_NewWidth, I_NewHeight));
		}

		public Bitmap Get_ResizeUseRate(Bitmap I_OriginBitmap, float I_ResizeRate)              //Eg: 177 / Math.Max(image01.height, image02.width)
		{
			return new Bitmap(I_OriginBitmap, new Size((int)(I_OriginBitmap.Width * I_ResizeRate), (int)(I_OriginBitmap.Height * I_ResizeRate)));
		}

		public Bitmap Get_ResizeUseMaxSide(Bitmap I_OriginBitmap, int I_MaxLength)              //Eg: (200, 354), 177 --> (100,177)
		{
			if(I_OriginBitmap.Width >= I_OriginBitmap.Height)
			{
				float I_WidthVSML = I_MaxLength / (float)I_OriginBitmap.Width;
				return new Bitmap(I_OriginBitmap, new Size(I_MaxLength, (int)(I_OriginBitmap.Height * I_WidthVSML)));
			}
			else
			{
				float I_HeightVSML = (I_MaxLength / (float)I_OriginBitmap.Height);
				return new Bitmap(I_OriginBitmap, new Size((int)(I_OriginBitmap.Width * I_HeightVSML), I_MaxLength));
			}
		}


		/// <summary>
		/// 将源图片覆盖到新图片之上，图片将尽量居中，且将会加上背景颜色
		/// 可能会耗时的操作
		/// </summary>
		/// <param name="I_OriginBitmap">原始图片（要加入的）</param>
		/// <param name="I_NewSize">新的图片的大小</param>
		/// <param name="I_Background">背景色</param>
		/// <returns></returns>
		public Bitmap Get_PicMidFillBackground(Bitmap I_OriginBitmap, Size I_NewSize, Color I_Background)
		{
			if(I_OriginBitmap.Width > I_NewSize.Width || I_OriginBitmap.Height > I_NewSize.Height)
			{
				I_OriginBitmap = Get_ResizeUseMaxSide(I_OriginBitmap, Math.Min(I_NewSize.Height, I_NewSize.Width));
			}

			int XOffset = (int)((I_NewSize.Width - I_OriginBitmap.Width) / 2);
			int YOffset = (int)((I_NewSize.Height - I_OriginBitmap.Height) / 2);

			Bitmap Result = new Bitmap(I_NewSize.Width, I_NewSize.Height);

			//对了，可以直接用Graphic来绘制Image类，然后用它来绘制长方形填充背景色，哇我真是个天才
			Graphics GraphicsResult = Graphics.FromImage(Result);

			//把图片“拱”进去
			//GraphicsResult.DrawImage(I_OriginBitmap, new Point(XOffset,YOffset));

			//保留备用，这个是原来的老方法，已启用，因为Graphic奇葩到要读取Sys的分辨率。。
			for(int iY = 0, Max_Height = I_OriginBitmap.Height; iY < Max_Height; iY++)
			{
				for(int iX = 0, Max_Width = I_OriginBitmap.Width; iX < Max_Width; iX++)
				{
					Result.SetPixel(XOffset + iX, YOffset + iY, I_OriginBitmap.GetPixel(iX, iY));
				}
			}

			//填充背景色	-->	看右边
			GraphicsResult.FillRectangle(new SolidBrush(I_Background), new Rectangle(0, 0, XOffset, Result.Height));                        //左长方形
			GraphicsResult.FillRectangle(new SolidBrush(I_Background), new Rectangle(Result.Width - XOffset, 0, XOffset, Result.Height));   //右长方形
			GraphicsResult.FillRectangle(new SolidBrush(I_Background), new Rectangle(0, 0, Result.Width, YOffset));                         //上长方形
			GraphicsResult.FillRectangle(new SolidBrush(I_Background), new Rectangle(0, Result.Height - YOffset, Result.Width, YOffset));   //下长方形

			return Result;
		}

		public char Get_SEColorChar(Color I_InputColor)
		{
			return (char)((0xE100 + (I_InputColor.R / 32 << 6) + (I_InputColor.G / 32 << 3) + (I_InputColor.B / 32)));
		}

		public char Get_ColorChar(Color I_InputColor)
		{
			return (char)((0xE100 + (I_InputColor.R << 6) + (I_InputColor.G << 3) + I_InputColor.B));
		}

		public bool Save_StringFile(string I_InputString, string I_FileFinalPath)
		{
			try
			{
				StreamWriter StreamWriter_StringFile = new StreamWriter(I_FileFinalPath, false, Encoding.Unicode);
				StreamWriter_StringFile.Write(I_InputString);
				StreamWriter_StringFile.Close();
			}
			catch(Exception e)
			{
				MessageBox.Show(e.ToString());
				return false;
			}
			return true;
		}

		/// <summary>
		/// 在字符串中从第<paramref name="I_Index"/>个<paramref name="I_NumLeftChar"/>字符开始查找，直到<paramref name="I_NumRightChar"/>
		/// 返回其中包括的整数数字
		/// </summary>
		/// <param name="I_TargetString">指定的字符串</param>
		/// <param name="I_NumLeftChar">数字左端字符</param>
		/// <param name="I_NumRightChar">数字右端字符</param>
		/// <param name="I_Index">从指定数量的左端字符后开始识别</param>
		/// <returns>如果查找失败将返回-1</returns>
		public int Get_ValueFromStringWithChar(string I_TargetString, char I_NumLeftChar, char I_NumRightChar, int I_Index = 0)
		{
			string[] A_String = I_TargetString.Split(I_NumLeftChar);
			int Result = -1;
			if(A_String.Length > 0)
			{
				A_String = A_String[I_Index].Split(I_NumRightChar);
				if(A_String.Length > 0)
				{
					if(int.TryParse(A_String[0], out Result))
					{
						return Result;
					}
				}
				else
				{
					return -1;
				}
			}
			else
			{
				return -1;
			}
			return -1;
		}

		public string Get_FileNameFromFinalPath(string I_FinalPath)
		{
			string[] Result_1 = I_FinalPath.Split('\\');
			return Result_1[Result_1.Length - 1];
		}

		public string Get_FileFolderPathFromFinalPath(string I_FinalPath)
		{
			string[] Result_0 = I_FinalPath.Split('\\');
			return I_FinalPath.Substring(0, I_FinalPath.Length - (Result_0[Result_0.Count() - 1].Length + 1));
		}
		#endregion

		/// <summary>
		/// 单独收纳一组原始图片与转换后图片与转码结果的类
		/// </summary>
		public class ImageContainer
		{
			public ImageContainer(string I_PicFinalPath)
			{
				PicFileFinalPath = I_PicFinalPath;
				FillImageWithPath();
				string[] Result_1 = I_PicFinalPath.Split('\\');
				PicFileName = Result_1[Result_1.Length - 1];
			}

			public ImageContainer(string I_PicFinalPath, Bitmap I_GIFClip, int I_gifIndex)
			{
				PicFileFinalPath = I_PicFinalPath;
				Pic = (Image)I_GIFClip;
				string[] Result_1 = I_PicFinalPath.Split('\\');
				GifIndex = I_gifIndex;
				PicFileName = Result_1[Result_1.Length - 1] + string.Format("_{0}", I_gifIndex);
			}


			public Image Pic;
			public Image FinalPic;

			public string PicFileFinalPath = "";
			public string PicFileName = "";

			public string PicFinalString = "";
			public bool IfCompleted = false;

			public int GifIndex;

			public ListViewItem Get_ListViewItem()
			{
				ListViewItem Result = new ListViewItem();
				Result.SubItems.Add(PicFileName);
				Result.SubItems.Add(IfCompleted ? "o" : "x");
				return Result;
			}

			public void FillImageWithPath()
			{
				Pic = Image.FromFile(PicFileFinalPath);
			}


			public void ResetMe(bool I_IfRefillImage = false)
			{
				FinalPic = null;
				PicFinalString = "";
				IfCompleted = false;
				if(I_IfRefillImage)
				{
					FillImageWithPath();
				}
			}

			//2017-12-04大改-封-
			//public int AllStepsNum;		//我是不是改用Long?
			//public Thread TaskThread;
			//public Color[,] PicColorArray;
			//public Color PicBackground;
			//public Size CustomSize = new Size(177, 177);
			//public static ConvertSupport ICCS = new ConvertSupport();
			//public int CurrentStepNum;
			/*
			public ImageContainer(string I_PicFinalPath)
			{
				PicFileFinalPath = I_PicFinalPath;
				Update_PicName();
				Pic = Update_ImageFromPath();
				TaskThread = null;
				Update_TaskStepInfo(1);
			}
			*/
			/*
			/// <summary>
			/// 从指定路径获取图片，并赋值给FinalPic
			/// </summary>
			public void FillClass()
			{
				Update_ImageFromPath();
				FinalPic = new Bitmap(Pic, CustomSize);
			}

			public float Get_TaskStepRate()
			{
				if (AllStepsNum > 0)
				{
					return (float)CurrentStepNum / AllStepsNum;
				}
				else
				{
					AllStepsNum = 1;
					return 0f;
				}
			}

			public void Update_TaskStepInfo(int I_StepNum)
			{
				CurrentStepNum = 0;
				AllStepsNum = I_StepNum;
			}

			public void Update_InitialThread()
			{
				Update_TaskStepInfo(1);
				TaskThread = new Thread(new ThreadStart(Update_FinalString));
			}			

			public void Update_ColorArray()
			{
				Update_TaskStepInfo(1);
				PicColorArray = ICCS.Get_ColorArray((Bitmap)FinalPic);
			}

			public void Do_Resize()
			{
				Update_TaskStepInfo(1);
				FinalPic = ICCS.Get_ResizedPic((Bitmap)Pic, CustomSize.Width, CustomSize.Height);
			}

			public void Do_PicMidAndFill()
			{
				Update_TaskStepInfo(1);
				FinalPic = ICCS.Get_PicMidFillBackground((Bitmap)Pic, CustomSize, PicBackground);
			}

			public ListViewItem Get_ConvertToLVItem()
			{
				ListViewItem Result = new ListViewItem();
				Result.SubItems.Add(PicFileName);
				Result.SubItems.Add(Get_TaskStepRate().ToString("0%"));
				Result.SubItems.Add(TaskThread.ThreadState.ToString());
				return Result;
			}
			

			public void Update_FinalString()
			{
				Update_TaskStepInfo(PicColorArray.GetLength(1));
				//PicString = ICCS.Get_StringPic(PicColorArray);
				string Result = "";
				for (int iY = 0, Max_Y = PicColorArray.GetLength(1); iY < Max_Y; iY++)
				{
					for (int iX = 0, Max_X = PicColorArray.GetLength(0); iX < Max_X; iX++)
					{
						Result += ICCS.Get_ColorChar(PicColorArray[iX, iY]);
					}
					CurrentStepNum++;
					Result += '\n';
				}
				PicString = Result;
			}

			public Image Update_ImageFromPath()
			{
				Update_TaskStepInfo(1);
				try
				{
					Pic = Image.FromFile(PicFileFinalPath);
				}
				catch (FileNotFoundException e)
				{
					MessageBox.Show("Error: " + PicFileFinalPath + "\nIs not a Picture or not a File");
				}
				catch (Exception e)
				{
					MessageBox.Show("Error: " + e.ToString());
				}
				return Pic;
			}

			public string Update_PicName()
			{
				PicFileName = ICCS.Get_FileNameFromPath(PicFileFinalPath);
				return PicFileName;
			}

			//Debug
			public void Debug_ProjectFunctionCheck()
			{
				Do_PicMidAndFill();
				Update_ColorArray();
				Update_FinalString();
			}
			*/

		}

		/// <summary>
		/// 描述一次转换任务的类
		/// </summary>
		public class ImageConvertTaskModel
		{
			static ConvertSupport ICTMCS = new ConvertSupport();

			public Size TargetNewSize = new Size(177, 177);
			public Color TargetBackGround = Color.Black;
			public string TargetGapString = "*&";

			public int Task_ConvertStepLength = 1;
			public int Task_ConvertCurrentStep = 0;

			public int STask_ConvertCurrentStep = 0;

			public List<ConvertSupport.ImageContainer> L_ICs = new List<ConvertSupport.ImageContainer>();

			public Dictionary<int, ConvertSupport.ImageContainer> Dic_ICSorted = new Dictionary<int, ImageContainer>();
			public Dictionary<int, ConvertSupport.ImageContainer> Dic_ICUnsorted = new Dictionary<int, ImageContainer>();

			public string FinalStringResult = "";

			public Thread TaskThread_MainConvert;

			public ENUM_ICTM_TaskRequest ICTM_TaskRequest = ENUM_ICTM_TaskRequest.None;

			public bool IfSorted = false;

			public enum ENUM_ICTM_TaskRequest : int
			{
				None = 0,
				RequestPause = 1,
				RequestDestroy = 2,
			}

			public void Task()
			{
				Task_ConvertCurrentStep = 0;
				Task_ConvertStepLength = L_ICs.Count;
				for(int iPicStep = 0, iMaxPicStep = L_ICs.Count; iPicStep < iMaxPicStep; iPicStep++)
				{
					L_ICs[iPicStep] = Task_SingleTask(iPicStep);
					FinalStringResult += TargetGapString + "\n" + L_ICs[iPicStep].PicFinalString;
					Task_ConvertCurrentStep = iPicStep + 1;
				}
			}

			public List<ListViewItem> Get_LVItemList()
			{
				List<ListViewItem> Result = new List<ListViewItem>();
				foreach(ConvertSupport.ImageContainer SIC in L_ICs)
				{
					Result.Add(SIC.Get_ListViewItem());
				}
				return Result;
			}

			public ConvertSupport.ImageContainer Task_SingleTask(int I_Index)
			{
				ConvertSupport.ImageContainer Result = L_ICs[I_Index];
				Result.FinalPic = ICTMCS.Get_PicMidFillBackground((Bitmap)L_ICs[I_Index].Pic, TargetNewSize, TargetBackGround);
				Color[,] A_Color = ICTMCS.Get_ColorArray((Bitmap)Result.FinalPic);
				string A_FinalString = "";
				for(int iStepY = 0, iMax = iMax = A_Color.GetLength(1); iStepY < iMax; iStepY++)
				{
					for(int iStepX = 0, iMax2 = A_Color.GetLength(0); iStepX < iMax2; iStepX++)
					{
						A_FinalString += ICTMCS.Get_SEColorChar(A_Color[iStepX, iStepY]);
					}
					A_FinalString += "\n";
					STask_ConvertCurrentStep = iStepY;   //告诉类，这个图片的转换已经进行到第{iStep}步了
				}
				Result.PicFinalString = A_FinalString;
				L_ICs[I_Index].IfCompleted = true;
				return Result;
			}


			/// <summary>
			/// 从第<paramref name="I_NumLeftChar"/>直到<paramref name="I_NumRightChar"/>开始查找数字，用于排序
			/// </summary>
			/// <param name="I_NumLeftChar">数字左端的第<paramref name="I_Index"/>个字符被用于排序</param>
			/// <param name="I_NumRightChar">直到<paramref name="I_NumRightChar"/></param>
			/// <param name="I_Index">指经过多少个<paramref name="I_NumLeftChar"/>才开始查找</param>
			/// <returns>被成功排序的图片数量</returns>
			public int Update_SortByNum(char I_NumLeftChar, char I_NumRightChar, int I_Index = 0)
			{
				IfSorted = false;
				Dic_ICSorted.Clear();
				Dic_ICUnsorted.Clear();
				int Result = 0;
				int Result_Unsorted = 0;

				for(int i = 0, iMax = L_ICs.Count; i < iMax; i++)
				{
					int Temp_Var1 = ICTMCS.Get_ValueFromStringWithChar(L_ICs[i].PicFileName, I_NumLeftChar, I_NumRightChar, I_Index);
					if(Temp_Var1 == -1)
					{
						Dic_ICUnsorted.Add(Result_Unsorted, L_ICs[i]);
						Result_Unsorted++;
					}
					else
					{
						Dic_ICSorted.Add(Temp_Var1, L_ICs[i]);
						Result++;
					}
				}
				return Result;
			}

			public int Get_PicNameFirstNum(string I_PicName)
			{
				for(int i = 0, iMax = I_PicName.Length; i < iMax; i++)
				{
					if(char.IsNumber(I_PicName[i]))
					{
						return i;
					}
				}
				return -1;
			}

			public int Get_PicNameLastNum(string I_PicName, int I_StartIndex = 0)
			{
				bool IfHasNum = false;
				for(int i = I_StartIndex, iMax = I_PicName.Length; i < iMax; i++)
				{
					if(char.IsNumber(I_PicName[i]))
					{
						IfHasNum = true;
					}
					if(IfHasNum && !char.IsNumber(I_PicName[i]))
					{
						return i;
					}
				}
				return -1;
			}
		}
	}
}

//第二次大改：弃用了超多线程模式，改用主线程模式-2017-12-04-
//2017-12-04:请注意程序中的//Debug标识
//2017-12-18:没啥别的
//

//来源：SE_PicsConverter 20190318 转移至该项目