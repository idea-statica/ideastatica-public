# BIM API Breaking Changes

## 23.1.2

### Depreciation of WCF Communication for Checkbot Links

Communication between BIM Links plugins and IdeaStatiCa Checkbot [BIMPluginHosting](../../api-plugin/IdeaStatiCa.Plugin.BIMPluginHosting.yml) communication class is no longer supported. It uses WCF communication which is now deprecated in our applications.

Communication with IDEA should migrate in BIM plugins to gRpc communication [BIMPluginHostingGrpc](../../api-plugin/IdeaStatiCa.Plugin.BIMPluginHostingGrpc.yml)
Examples of usage can be found in the latest here:

* [BimApiFeaLinkExample](https://github.com/idea-statica/ideastatica-public/tree/main/src/Examples/CheckbotBimLink)

Sample code is provided below to show changes in the Run method of the Plugin. 

```csharp
 
public void Run(object param)
{
    Logger.LogInformation($"Run param = '{param?.ToString()}'");
    
    var factory = new PluginFactory(this);
    
    Logger.LogDebug("Run - calling GrpcBimHostingFactory");
    
    //Using new Grpc Communication Classes 
    var bimHostingFactory = new GrpcBimHostingFactory();
    var pluginHostingGrpc = bimHostingFactory.Create(factory, Logger);
    
    FeaAppHosting = pluginHostingGrpc;
    
    this.IdeaStatica = ((ApplicationBIM)FeaAppHosting.Service).IdeaStaticaApp;
    
    FeaAppHosting.AppStatusChanged += new ISEventHandler(IdeaStaticAppStatusChanged);
    
    var id = Process.GetCurrentProcess().Id.ToString();
    
    ProjectDir = Path.Combine(WorkingDirectory, ProjectName);
    
    if (!Directory.Exists(ProjectDir))
    {
        Logger.LogDebug($"Run - creating new project dir '{ProjectDir}'");
        Directory.CreateDirectory(ProjectDir);
    }
    else
    {
        Logger.LogDebug($"Run - using existing dir '{ProjectDir}'");
    }
}

```
