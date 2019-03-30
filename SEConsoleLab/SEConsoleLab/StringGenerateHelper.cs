using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using System.Drawing.Imaging;
using System.Drawing;

using System.Windows.Forms;


using Color = System.Drawing.Color;

namespace SEConsoleLab
{
	public class StringGenerateHelper
	{
		public class PicGenerator
		{
			public Image targetImage;
			
			string targetImagePath = @"D:\Code-Half\SpaceengineerProjects\Labs\SEConsoleLab\SEConsoleLab\ForkPotato.gif";

			public Size TargetImageSize = new Size(64, 64);			//图片最大值：254x255 因为null取反为FF
			public Color TargetImageBackgroundColor = Color.Black;
			public int GIFLength = 1;

			public const int PixelDifferenceThreshold = 1;				//Recommand val => 1
			//颜色差在2的色块会被加入色变动表中，取值：[1, 7] .取值越大，gif字符串越短，图像越浑浊
			//Color1 - Color0 = PixelDifference;
			//PixelDifference / (binary)1001001 （不行，算了一下，如果delta只在蓝色上的话根本不能计算）
			//还是老老实实用 位与 计算吧。。。
			

			//0x01FF ->   0000 000R RRGG GBBB
			//0xE100 -> + 1110 0001 0000 0000
			//0xE2FF -> = 1110 00R0 RRGG GBBB

			//0x0049 ---> 0000 0000 0100 1001
			//                    R RRGG GBBB

			public ConvertSupport CSInstance = new ConvertSupport();


			public void GetImage()
			{
				try
				{
					targetImage = Image.FromFile(targetImagePath);
				}catch(Exception ex)
				{
					//MessageBox.Show(ex.ToString());
					throw ex;
				}
			}

			public StringBuilder GIF_Style1()	//底板+点差
			{
				StringBuilder SB_Result = new StringBuilder(50_000);
				
				FrameDimension fdm = new FrameDimension(targetImage.FrameDimensionsList[0]);
				GIFLength = targetImage.GetFrameCount(fdm);
				

				var picsize = targetImage.Size;
				targetImage.SelectActiveFrame(fdm, GIFLength - 1);
				Bitmap bmp0 = new Bitmap(targetImage);
				Bitmap bmp1 = new Bitmap(targetImage);

				//转换图片为字符串，耗时工作
				StringBuilder bmpt0str = new StringBuilder(TargetImageSize.Height * (TargetImageSize.Width + 2));
				StringBuilder bmpt1str = new StringBuilder(TargetImageSize.Height * (TargetImageSize.Width + 2));

				bmpt0str.Append(
					CSInstance.Get_StringPic(
						CSInstance.Get_ColorArray(
							CSInstance.Get_PicMidFillBackground(
								bmp0, TargetImageSize, TargetImageBackgroundColor))));
				for(int currentindex = 0; currentindex < GIFLength; currentindex++)
				{
					targetImage.SelectActiveFrame(fdm, currentindex);
					bmp1 = new Bitmap(targetImage);
					bmpt1str.Append(
						CSInstance.Get_StringPic(
							CSInstance.Get_ColorArray(
								CSInstance.Get_PicMidFillBackground(
									bmp1, TargetImageSize, TargetImageBackgroundColor))));
					//色色色色\n <-'\n'是第[4]个字符
					//色色色色\n
					List<PixelChange2> L_pixelChanges = GetDifference(bmpt0str, bmpt1str, currentindex);
					foreach(var x in L_pixelChanges)
					{
						SB_Result.Append(x.ToString());
					}
					bmpt0str.Clear();
					bmpt0str.Append(bmpt1str);
					bmpt1str.Clear();
				}



				targetImage.SelectActiveFrame(fdm, GIFLength - 1);
				return SB_Result;
			}

			public List<PixelChange2> GetDifference(StringBuilder I_SB_before, StringBuilder I_SB_after, int I_currentPicIndex)
			{
				List<PixelChange2> L_Result = new List<PixelChange2>();
				int currentdelta = 0;
				for(int i = 0, iMax = I_SB_before.Length; i < iMax; i++)
				{
					currentdelta = I_SB_after[i] - I_SB_before[i];
					if(CheckIfOverThreshold(currentdelta))
					{
						L_Result.Add(new PixelChange2()
						{
							//Pos = new Vector2I(i % (TargetImageSize.Width + 1), i / (TargetImageSize.Width + 1)),
							Pos = i,
							deltaColor = currentdelta,
							index = I_currentPicIndex,
						});
					}
					else
					{
						I_SB_after[i] = I_SB_before[i];			//若不用此行，出现渐变时变化可能始终小于Threshold导致GIF没变化
					}
				}
				return L_Result;
			}

			public bool CheckIfOverThreshold(int I_delta)
			{
				if(I_delta == 0)
				{
					return false;
				}
				else if(I_delta < 0)
				{
					I_delta = -I_delta;
				}
				if((I_delta & 0x7) >= PixelDifferenceThreshold)
				{
					return true;
				}
				I_delta >>= 3;
				if((I_delta & 0x7) >= PixelDifferenceThreshold)
				{
					return true;
				}
				I_delta >>= 3;
				if((I_delta & 0x7) >= PixelDifferenceThreshold)
				{
					return true;
				}
				return false;
			}

			[Obsolete]
			public class PixelChange
			{
				public PixelChange()
				{

				}

				public PixelChange(char I_pos, char I_char)
				{
					pos = new Vector2I((I_pos & 0xFF) - 1, (I_pos >>= 8) & 0xFF);
					deltaColor = ((I_char & 0x3FF) - 0x1FF);
					index = (I_char >>= 10);
				}


				private Vector2I pos;
				public Vector2I Pos {
					get
					{
						return pos;
					}
					set
					{
						if(value.X <= 254 && value.X >= 0 && value.Y <= 255 && value.Y >= 0)
						{
							pos = value;
						}
						else
						{
							throw new Exception("Warning! X must in [0, 254], Y must in [0,255]\n" +
								"Current Input:" + value.ToString());
						}
					}
					}							//注意，此处X值域：[0,254]，Y值域：[0,255]
				public int deltaColor { get; set; }	//deltaColor取值[-511, 511]
				public int index { get; set; }	//

				public override string ToString()
				{
					char pos = (char)(((Pos.Y & 0xFF) << 8) + ((Pos.X + 1) & 0xFF));        //X + 1以预防null，但这也导致横轴缺少一像素
					char delta = (char)(((deltaColor + 0x1FF) & 0x3FF) + (index << 10));				//C#的char好渣啊... ...
					return new string(new char[] { pos, delta });									
					//YYYYYYYY XXXXXXXX  IIIIIICC CCCCCCCC
					//Y最小值0 X最小值1  I最小值1 C最小值0
					//想要遍历的时候直接把pos++就可以依次遍历
				}
			}

			[Obsolete]
			public class PixelChange2
			{


				public PixelChange2()
				{

				}

				public PixelChange2(char I_pos, char I_char)
				{
					Pos = (int)I_pos - 1;
					deltaColor = ((I_char & 0x3FF) - 0x200);
					index = (I_char >>= 10);
				}


				public int Pos;                     //注意，Pos大小取值[0, 65534] ([0, FFFE])
				public int deltaColor { get; set; } //deltaColor取值[-511, 511]
				public int index { get; set; }      //GIF页码

				
				public override string ToString()
				{
					char pos = (char)(Pos + 1);                                         //P + 1以预防null
					char delta = (char)(((deltaColor + 0x200) & 0x3FF) + (index << 10));//C#的char好渣啊... ...
					return new string(new char[] { pos, delta });
					//PPPPPPPP PPPPPPPP  IIIIIICC CCCCCCCC
					//P is [1, FFFF]     I最小值1 C最小值0
					//想要遍历的时候直接把pos++就可以依次遍历
				}
			}

			public class PixelChange3
			{

				public PixelChange3()
				{

				}

				public PixelChange3(char I_pos, char I_char)
				{
					Pos = (int)I_pos - 1;
					DeltaColor = ((I_char & 0x3FF) - 0x200);
					Info = (I_char >>= 10);
				}


				public int Pos;                     //注意，Pos大小取值[0, 65534] ([0, FFFE])
				public int DeltaColor { get; set; } //deltaColor取值[-511, 511]
				public int Info { get; set; }      //GIF页码


				public override string ToString()
				{
					char pos = (char)(Pos + 1);                                         //P + 1以预防null
					char delta = (char)(((DeltaColor + 0x200) & 0x3FF) + (Info << 10));//C#的char好渣啊... ...
					return new string(new char[] { pos, delta });
					//PPPPPPPP PPPPPPPP  IIIIIICC CCCCCCCC
					//P is [1, FFFF]     I最小值1 C最小值0
					//想要遍历的时候直接把pos++就可以依次遍历
				}
			}
		}
	}
}
