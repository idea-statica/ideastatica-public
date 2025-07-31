using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace IdeaConWpfApp
{

	public partial class CustomConversionWindow : Window
	{
		public Dictionary<string, string> SteelMapping { get; private set; } = new();
		public Dictionary<string, string> WeldsMapping { get; private set; } = new();
		public Dictionary<string, string> BoltsMapping { get; private set; } = new();
		public Dictionary<string, string> BoltGradesMapping { get; private set; } = new();
		public Dictionary<string, string> ConcreteMapping { get; private set; } = new();
		public Dictionary<string, string> CrossSectionsMapping { get; private set; } = new();

		public CustomConversionWindow(Dictionary<string, string> steel,
									  Dictionary<string, string> welds,
									  Dictionary<string, string> bolts,
									  Dictionary<string, string> boltGrades,
									  Dictionary<string, string> concrete,
									  Dictionary<string, string> crossSections)
		{
			InitializeComponent();

			PopulateStack(SteelStack, steel);
			PopulateStack(WeldsStack, welds);
			PopulateStack(BoltsStack, bolts);
			PopulateStack(BoltGradesStack, boltGrades);
			PopulateStack(ConcreteStack, concrete);
			PopulateStack(CrossSectionsStack, crossSections);
		}

		private void PopulateStack(StackPanel stack, Dictionary<string, string> data)
		{
			foreach (var kvp in data)
			{
				var panel = new StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 5) };

				var label = new TextBlock
				{
					Text = kvp.Key,
					Width = 200,
					VerticalAlignment = VerticalAlignment.Center
				};

				var textBox = new System.Windows.Controls.TextBox
				{
					Text = kvp.Value,
					Width = 300,
					Tag = kvp.Key // Store key for retrieval
				};

				panel.Children.Add(label);
				panel.Children.Add(textBox);

				stack.Children.Add(panel);
			}
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			SteelMapping = ReadValuesFromStack(SteelStack);
			WeldsMapping = ReadValuesFromStack(WeldsStack);
			BoltsMapping = ReadValuesFromStack(BoltsStack);
			BoltGradesMapping = ReadValuesFromStack(BoltGradesStack);
			ConcreteMapping = ReadValuesFromStack(ConcreteStack);
			CrossSectionsMapping = ReadValuesFromStack(CrossSectionsStack);

			DialogResult = true;
			Close();
		}

		private Dictionary<string, string> ReadValuesFromStack(StackPanel stack)
		{
			var result = new Dictionary<string, string>();
			foreach (StackPanel row in stack.Children)
			{
				var textBox = row.Children[1] as System.Windows.Controls.TextBox;
				var key = textBox?.Tag.ToString();
				var value = textBox?.Text;
				result[key!] = value!;
			}
			return result;
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
