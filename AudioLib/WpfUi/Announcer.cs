using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Audiolib.WpfUi
{
	public static class Announcer
	{
		private static Dictionary<DependencyObject, Action<string, string, bool>> Announcers = new Dictionary<DependencyObject, Action<string, string, bool>>();
		private static Dictionary<DependencyObject, DependencyObject> Children = new Dictionary<DependencyObject, DependencyObject>();


		public static void RegisterAnnouncer(DependencyObject parentElement, Action<string, string, bool> handler)
		{
			Announcers[parentElement] = handler;
		}

		public static void SendAnnouncement(this DependencyObject element, string parameterName, string formattedValue, bool timeout)
		{
			var parent = FindParentObject(element);
			if (parent != null)
				Announcers[parent](parameterName, formattedValue, timeout);
		}

		private static DependencyObject FindParentObject(DependencyObject child)
		{
			if (Children.ContainsKey(child))
				return Children[child];

			DependencyObject current = child;

			while(current != null)
			{
				if(Announcers.ContainsKey(current))
				{
					Children[child] = current;
					return current;
				}

				current = System.Windows.Media.VisualTreeHelper.GetParent(current);
			}

			return null;
		}
	}
}
