using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System.Reflection;
using VRage.Game;

using System.Windows.Forms;

namespace SEConsoleLab
{
	public class ProjectReflectionTestHelper
	{
		public Assembly SE;
		public string Path =
			@"D:\SteamLibrary\steamapps\common\SpaceEngineers\Bin64\Sandbox.Common.dll";

		public void Ini()
		{
			SE = Assembly.ReflectionOnlyLoadFrom(Path);
			//Assembly.LoadFrom(@"D:\SteamLibrary\steamapps\common\SpaceEngineers\Bin64\VRage.Game.dll");
			var l = SE.GetReferencedAssemblies();

			foreach(var sl in l)
			{
				//MessageBox.Show(sl.FullName);
			}




			foreach(var sti in SE.DefinedTypes)
			{
				Console.WriteLine(sti.Name);
			}
		}
	}
}