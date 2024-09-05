using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Runtime;
using IronMan.Abstract.Acad;
using IronMan.Acad.Demo;
using IronMan.Acad.Demo.Extensions;
using IronMan.Acad.Demo.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

[assembly: ExtensionApplication(typeof(App))]
namespace IronMan.Acad.Demo;

public class App : IExtensionApplication
{
    private readonly IServiceProvider provider;
    public App()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IAppUI, AppUI>();

        provider = services.BuildServiceProvider();


    }

    public void Initialize()
    {
        provider.GetService<IAppUI>().OnStartUp();
    }

    public void Terminate()
    {
        provider.GetService<IAppUI>().OnShutDown();
    }

}
