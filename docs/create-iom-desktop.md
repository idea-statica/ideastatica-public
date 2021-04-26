#### The example of creating IDEA Connection project IOM locally is in the project [IOM.SteelFrameDesktop](https://github.com/idea-statica/iom-examples/tree/master/IOM_SteelFrame1/IOM.SteelFrameDesktop)

The axample how to generate IOM for a steel frame and including geometry of connections is in IOM.GeneratorExample. Running this example requires IDEA StatiCa v 20.0 (or higher) on an user's PC. Free trial version version can be obtained [here](https://www.ideastatica.com/free-trial).

[IdeaStatiCa.Plugin](https://github.com/idea-statica/ideastatica-plugin) includes classes which allows communication and controlling IDEA StatiCa applications and its provides services to other applications. [IdeaStatiCa.Plugin](https://github.com/idea-statica/ideastatica-plugin) is also distributed as [nuget package](https://www.nuget.org/packages/IdeaStatiCa.Plugin/)

```C#
			// create IOM and results
			OpenModel example = SteelFrameExample.CreateIOM();
			OpenModelResult result = Helpers.GetResults();

			string iomFileName = "example.xml";
			string iomResFileName = "example.xmlR";

			// save to the files
			example.SaveToXmlFile(iomFileName);
			result.SaveToXmlFile(iomResFileName);
```

The instance of the class *ConnHiddenClientFactory* is responsible for creating the instance of *ConnectionHiddenCheckClient* whic communicates with the local installation of Idea StatiCa. The path to the installation directory is passed in the constructor of *ConnHiddenClientFactory*

```C#
			IdeaInstallDir = IOM.SteelFrameDesktop.Properties.Settings.Default.IdeaInstallDir;
			Console.WriteLine("IDEA StatiCa installation directory is '{0}'", IdeaInstallDir);

			ConnectionHiddenCheckClient calcFactory = new ConnHiddenClientFactory(IdeaInstallDir);
```

The installation directory of Idea StatiCa v 20.0 (or higher) is set in the project setting of *IOM.SteelFrameDesktop*. Idea Connection project is created by calling method *CreateConProjFromIOM*

```C#
				client.CreateConProjFromIOM(iomFileName, iomResFileName, fileConnFileNameFromLocal);
				Console.WriteLine("Generated project was saved to the file '{0}'", fileConnFileNameFromLocal);
```




