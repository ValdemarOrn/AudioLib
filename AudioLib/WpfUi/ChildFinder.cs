using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AudioLib.WpfUi
{
    public class ChildFinder
    {
        public static IEnumerable<T> GetChildrenOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if (child is T)
                    yield return child as T;
                else
                {
                    var result = GetChildrenOfType<T>(child);
                    if (result != null)
                        foreach (var r in result)
                            yield return r;
                }
            }
        }
    }
}
