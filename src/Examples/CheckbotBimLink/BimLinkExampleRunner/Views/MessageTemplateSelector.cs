using BimLinkExampleRunner.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BimLinkExampleRunner.Views
{
	internal class MessageTemplateSelector : DataTemplateSelector
	{
		public DataTemplate StandardTemplate { get; set; }

		public DataTemplate ExceptionTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is MessageViewModel vm && StandardTemplate != null && ExceptionTemplate != null)
			{
				return vm.Exception == null
					? StandardTemplate
					: ExceptionTemplate;
			}

			return base.SelectTemplate(item, container);
		}
	}
}
