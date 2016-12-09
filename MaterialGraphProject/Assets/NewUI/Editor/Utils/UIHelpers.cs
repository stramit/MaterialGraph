using System.Linq;
using System.Reflection;
using UnityEditor;

namespace RMGUI.GraphView
{
	internal class UIHelpers
	{
		static MethodInfo s_ApplyWireMaterialMi;

		public static void ApplyWireMaterial()
		{
			if (s_ApplyWireMaterialMi == null)
			{
				s_ApplyWireMaterialMi = typeof(HandleUtility)
					.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
					.Where(m => m.Name == "ApplyWireMaterial" && m.GetParameters().Count() == 0).First();
			}

			if (s_ApplyWireMaterialMi != null)
			{
				s_ApplyWireMaterialMi.Invoke(null, null);
			}
		}
	}
}
