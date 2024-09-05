using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Customization;
using IronMan.Abstract.Acad;
using IronMan.Acad.Demo.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.Acad.Demo
{
    /// <summary>
    /// 不使用aswindow或acwindow方式创建ui，code方式？
    /// 自定义菜单的两个优点：1.不是缓存方式（切换界面菜单会丢失）；2.同时支持菜单栏和侧边栏两种方式；
    /// </summary>
    internal class AppUI:IAppUI
    {
        private string menuGroup = "IronMan";
        private string customCuix;
        private string root;
        private CustomizationSection mainSection;

        public MacroGroup ParentGroup { get; set; }
        public MenuMacro WelcomeMenuMacro { get; set; }
        public MenuMacro CreateLine { get; set; }
        public MenuMacro AnnotatedAraeByPoint { get; set; }
        public MenuMacro AnnotatedAreaByLayer { get; set; }
        public MenuMacro CircularAnnotatedArea { get; set; }

        public void OnStartUp()
        {
            mainSection = GetMainCustomizationSection();
            //create custom cuix
            root = GetRootPath();
            customCuix = GetCustomCuix();
            Autodesk.AutoCAD.ApplicationServices.Core.Application.BeginQuit += Application_BeginQuit;
            DeleteCuix();

            var customSection = new CustomizationSection()
            {
                MenuGroupName = menuGroup
            };

            //创建宏
            var macroGroup = new MacroGroup("IronMan", customSection.MenuGroup);
            ParentGroup = macroGroup;

            WelcomeMenuMacro = macroGroup.CreateMenuMacro("WelcomeCommand", $"{root}\\Assets\\PhoneNumber.png");
            CreateLine = macroGroup.CreateMenuMacro("CreateLine", $"{root}\\Assets\\PhoneNumber.png");
            AnnotatedAraeByPoint = macroGroup.CreateMenuMacro("AnnotatedAraeByPoint", $"{root}\\Assets\\PhoneNumber.png");
            AnnotatedAreaByLayer = macroGroup.CreateMenuMacro("AnnotatedAreaByLayer", $"{root}\\Assets\\PhoneNumber.png");
            CircularAnnotatedArea = macroGroup.CreateMenuMacro("CircularAnnotatedArea", $"{root}\\Assets\\PhoneNumber.png");

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
            var tabSource = section.CreateTab("IronMan");
            var panelSource = tabSource.CreatePanel("BasicApi");
            var basicApiRow = panelSource.CreateRibbonRow();

            //创建按钮
            basicApiRow.CreatePushButton(x =>
            {
                x.Text = "Welcome";
                x.ButtonStyle = RibbonButtonStyle.LargeWithText;
                x.MacroID = WelcomeMenuMacro.ElementID;
            });
            basicApiRow.CreatePushButton(x =>
            {
                x.Text = "CreateLine";
                x.MacroID = CreateLine.ElementID;
            });


            basicApiRow.CreateRibbonSplitButton(btn =>
            {

            },
            btn1 =>
            {
                btn1.Text = "ByPoint";
                btn1.MacroID = ParentGroup.CreateMenuMacro("AnnotatedAreaByPoint", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            },
            btn2 =>
            {
                btn2.Text = "ByLayer";
                btn2.MacroID = ParentGroup.CreateMenuMacro("AnnotatedAreaByLayer", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            },
            btn3 =>
            {
                btn3.Text = "Circular";
                btn3.MacroID = ParentGroup.CreateMenuMacro("CircularAnnotatedArea", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            });


            ////创建分割线
            basicApiRow.CreateRibbonSeparator();

            basicApiRow.CreatePushButton(x =>
            {
                x.Text = "ConnectToAcad";
                x.MacroID = ParentGroup.CreateMenuMacro("ConnectToAcad", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            });
            basicApiRow.CreatePushButton(x =>
            {
                x.Text = "ReadDoc";
                x.MacroID = ParentGroup.CreateMenuMacro("ReadDoc", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            });

            basicApiRow.CreateRibbonSplitButton(null,
            btn1 =>
            {
                btn1.Text = "BlockTable";
                btn1.MacroID = ParentGroup.CreateMenuMacro("ReadBlockTable", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            },
            btn2 =>
            {
                btn2.Text = "LayerTable";
                btn2.MacroID = ParentGroup.CreateMenuMacro("ReadLayerTable", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            },
            btn3 =>
            {
                btn3.Text = "DimTable";
                btn3.MacroID = ParentGroup.CreateMenuMacro("ReadLayerTable", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            }
            );

            basicApiRow.CreateRibbonSplitButton(null,
            btn1 =>
            {
                btn1.Text = "LayoutDic";
                btn1.MacroID = ParentGroup.CreateMenuMacro("ReadLayoutDic", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            },
            btn2 =>
            {
                btn2.Text = "LayerTable";
                btn2.MacroID = ParentGroup.CreateMenuMacro("ReadLayerTable", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            },
            btn3 =>
            {
                btn3.Text = "DimTable";
                btn3.MacroID = ParentGroup.CreateMenuMacro("ReadLayerTable", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            }
            );

            basicApiRow.CreateRibbonSplitButton(null,
            btn1 =>
            {
                btn1.Text = "AppendData";
                btn1.MacroID = ParentGroup.CreateMenuMacro("AppendData", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            },
            btn11 =>
            {
                btn11.Text = "AppendData2";
                btn11.MacroID = ParentGroup.CreateMenuMacro("AppendData2", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            },
            btn2 =>
            {
                btn2.Text = "ReadData";
                btn2.MacroID = ParentGroup.CreateMenuMacro("ReadData", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            },
            btn3 =>
            {
                btn3.Text = "DeleteData";
                btn3.MacroID = ParentGroup.CreateMenuMacro("DeleteData", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            },
            btn4 =>
            {
                btn4.Text = "UpdateData";
                btn4.MacroID = ParentGroup.CreateMenuMacro("UpdateData", $"{root}\\Assets\\PhoneNumber.png").ElementID;
            }
            );


            var funcRow = tabSource.CreatePanel("Func").CreateRibbonRow();

            ////创建堆叠式按钮
            funcRow.CreateStackPanelButton(btn1 =>
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
            funcRow.CreateRibbonSplitButton(x =>
            {
                x.Text = "建筑";
                x.KeyTip = "keyTip";
                x.TooltipTitle = "Tooltip Title";
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
            //查询主文件中的局部自定义文件是否包含当前自定义文件
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

        private void Application_BeginQuit(object sender, BeginQuitEventArgs e)
        {
            DeleteCuix();
            Autodesk.AutoCAD.ApplicationServices.Core.Application.BeginQuit -= Application_BeginQuit;
        }

        /// <summary>
        /// 有时候CAD关闭太快，终结方法不会执行
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void OnShutDown()
        {
            DeleteCuix();
        }
        private string GetCustomCuix()
        {
            return $"{root}\\{menuGroup}.cuix";
        }

        private string GetRootPath()
        {
            var location = GetType().Assembly.Location;
            var root = Directory.GetParent(location).FullName;
            return root;
        }

        private CustomizationSection GetMainCustomizationSection()
        {
            var mainCuix = $"{Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable("MENUNAME")}.cuix";
            var section = new CustomizationSection(mainCuix);
            return section;
        }
    }
}
