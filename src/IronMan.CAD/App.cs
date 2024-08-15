using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Runtime;
using IronMan.CAD;
using IronMan.CAD.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

[assembly: ExtensionApplication(typeof(App))]
namespace IronMan.CAD
{
    /// <summary>
    /// 不使用aswindow或acwindow方式创建ui，code方式？
    /// 自定义菜单的两个优点：1.不是缓存方式（切换界面菜单会丢失）；2.同时支持菜单栏和侧边栏两种方式；
    /// </summary>
    public class App : IExtensionApplication
    {
        private string menuGroup = "IronMan";
        private string customCuix;
        private string root;
        private CustomizationSection mainSection;

        private MenuMacro home;

        public App()
        {
            mainSection = GetMainCustomizationSection();
            //create custom cuix
            root = GetRootPath();
            customCuix = GetCustomCuix();
        }

        public MacroGroup Parent { get; set; }

        public MenuMacro WelcomeMenuMacro
        {
            get
            {
                if (home == null)
                {
                    home = new MenuMacro(Parent, "home", "WelcomeCommand", "IronMan_tag");
                    home.macro.LargeImage = $@"{root}\Assets\PhoneNumber.png";
                    home.macro.SmallImage = $@"{root}\Assets\PhoneNumber.png";
                }
                return home;
            }
        }

        public void Initialize()
        {
            Autodesk.AutoCAD.ApplicationServices.Core.Application.BeginQuit += Application_BeginQuit;
            DeleteCuix();

            var customSection = new CustomizationSection()
            {
                MenuGroupName = menuGroup
            };
            Parent = new MacroGroup("IronMan", customSection.MenuGroup);

            CreateRibbon(customSection);
            //CreatePopMenu(customSection);

            //另存cuix文件
            if (customSection.SaveAs(customCuix))
            {
                mainSection.AddPartialMenu(customSection.CUIFileName);
                mainSection.Save();
                Application.LoadPartialMenu(customSection.CUIFileName);
            }
        }

        private void CreateRibbon(CustomizationSection section)
        {
            var ribbonRoot = section.MenuGroup.RibbonRoot;

            ////创建Tab
            //var tabSource = new RibbonTabSource(ribbonRoot)
            //{
            //    Name = "IronMan",
            //    Text = "IronMan"
            //};
            //ribbonRoot.RibbonTabSources.Add(tabSource);

            var tabSource = section.CreateTab("IronMan");


            //创建Panel
            //var panelSource = new RibbonPanelSource(ribbonRoot)
            //{
            //    Name = "userpanel",
            //    Text = "一个Panel"
            //};
            //ribbonRoot.RibbonPanelSources.Add(panelSource);

            var panelSource = tabSource.CreatePanel("UserPanel");


            ////创建行
            //var ribbonRow = new RibbonRow(panelSource);
            //panelSource.Items.Add(ribbonRow);

            //var ribbonPanelSourceReference = new RibbonPanelSourceReference(tabSource)
            //{
            //    PanelId = panelSource.ElementID,
            //};
            //tabSource.Items.Add(ribbonPanelSourceReference);

            var ribbonRow = panelSource.CreateRibbonRow();


            //创建按钮
            ribbonRow.CreatePushButton(x =>
            {
                x.Text = "Welcome";
                x.ButtonStyle = RibbonButtonStyle.LargeWithText;
                x.MacroID = WelcomeMenuMacro.ElementID;
            });

            ////创建分割线
            ribbonRow.CreateRibbonSeparator();

            ////创建堆叠式按钮
            ribbonRow.CreateStackPanelButton(btn1 =>
            {
                btn1.Text = "堆叠按钮1";
                btn1.MacroID = WelcomeMenuMacro.ElementID;
            },
            btn2 =>
            {
                btn2.Text = "堆叠按钮2";
                btn2.MacroID = WelcomeMenuMacro.ElementID;
            },
            btn3 =>
            {
                btn3.Text = "堆叠按钮3";
                btn3.MacroID = WelcomeMenuMacro.ElementID;
            });

            ////创建下拉式按钮
            ribbonRow.CreateRibbonSplitButton(x =>
            {
                x.Text = "建筑";
            },
            btn1 =>
            {
                btn1.Text = "墙";
                btn1.MacroID = WelcomeMenuMacro.ElementID;
            },
            btn1 =>
            {
                btn1.Text = "梁";
                btn1.MacroID = WelcomeMenuMacro.ElementID;
            },
            btn1 =>
            {
                btn1.Text = "柱";
                btn1.MacroID = WelcomeMenuMacro.ElementID;
            },
            btn1 =>
            {
                btn1.Text = "板";
                btn1.MacroID = WelcomeMenuMacro.ElementID;
            }
            );
        }

        //private void CreatePopMenu(CustomizationSection section)
        //{
        //    var strings = new StringCollection()
        //    {
        //        "SuperMan"
        //    };

        //    var popMenu = new PopMenu("IronMan", strings, "IronManTag", section.MenuGroup);
        //    new PopMenuItem(WelcomeMenuMacro, "建筑", popMenu, 0);
        //    new PopMenuItem(WelcomeMenuMacro, "结构", popMenu, 1);
        //    new PopMenuItem(WelcomeMenuMacro, "机电", popMenu, 2);

        //    //创建子菜单
        //    var subMenu = new PopMenu("Other", new StringCollection(), "Other", section.MenuGroup);
        //    new PopMenuRef(subMenu, popMenu, 3);
        //    new PopMenuItem(WelcomeMenuMacro, "景观", subMenu, 0);
        //    new PopMenuItem(WelcomeMenuMacro, "市政", subMenu, 1);
        //    new PopMenuItem(WelcomeMenuMacro, "规划", subMenu, 2);
        //    section.MenuGroup.PopMenus.Add(popMenu);
        //}

        /// <summary>
        /// 删除旧的Cuix
        /// </summary>
        private void DeleteCuix()
        {
            if (!mainSection.PartialCuiFiles.Contains(customCuix))
            {
                return;
            }
            if (!mainSection.RemovePartialMenu(customCuix, menuGroup))
            {
                return;
            }
            if (!mainSection.Save())
            {
                return;
            }
            if (!File.Exists(customCuix))
            {
                return;
            }
            File.Delete(customCuix);
        }

        private void Application_BeginQuit(object sender, Autodesk.AutoCAD.ApplicationServices.BeginQuitEventArgs e)
        {
            DeleteCuix();
            Autodesk.AutoCAD.ApplicationServices.Core.Application.BeginQuit -= Application_BeginQuit;
        }

        /// <summary>
        /// 有时候CAD关闭太快，终结方法不会执行
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Terminate()
        {
            throw new NotImplementedException();
        }
        private string GetCustomCuix()
        {
            return $"{root}\\{menuGroup}.cuix";
        }

        private string GetRootPath()
        {
            var location = this.GetType().Assembly.Location;
            var root = Directory.GetParent(location).FullName;
            return root;
        }

        private CustomizationSection GetMainCustomizationSection()
        {
            //get main cuix
            var mainCuix = $"{Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable("MENUNAME")}.cuix";
            var fileInfo = new FileInfo(mainCuix);
            var section = new CustomizationSection(mainCuix);
            return section;
        }

    }
}
