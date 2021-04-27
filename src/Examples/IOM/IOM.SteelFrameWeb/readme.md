#### The example of creating IDEA Connection project by webservice


It allows to generate idea connection project without having Idea StatiCa on your PC. IOM and IOM results are sent to webservice which generates and returns Idea Connection project.

```C#
		public static readonly string viewerURL = "https://viewer.ideastatica.com";

		public static void CreateOnServer(OpenModel model, OpenModelResult openModelResult, string path)
		{
			IdeaRS.OpenModel.OpenModelContainer openModelContainer = new OpenModelContainer()
			{
				OpenModel = model,
				OpenModelResult = openModelResult,
			};

			// serialize IOM to XML
			var stringwriter = new System.IO.StringWriter();
			var serializer = new XmlSerializer(typeof(OpenModelContainer));
			serializer.Serialize(stringwriter, openModelContainer);

			var serviceUrl = viewerURL + "/ConnectionViewer/CreateFromIOM";

			Console.WriteLine("Posting iom in xml to the service {0}", serviceUrl);
			var resultMessage = Helpers.PostXMLData(serviceUrl, stringwriter.ToString());

			ResponseMessage responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(resultMessage);
			Console.WriteLine("Service response is : '{0}'", responseMessage.status);
			if (responseMessage.status == "OK")
			{
				byte[] dataBuffer = Convert.FromBase64String(responseMessage.fileContent);
				Console.WriteLine("Writing {0} bytes to file '{1}'", dataBuffer.Length, path);
				if (dataBuffer.Length > 0)
				{
					using (FileStream fileStream = new FileStream(path
				, FileMode.Create
				, FileAccess.Write))
					{
						fileStream.Write(dataBuffer, 0, dataBuffer.Length);
					}
				}
				else
				{
					Console.WriteLine("The service returned no data");
				}
			}
		}
	}

```