using Autodesk.AutoCAD.Customization;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IronMan.CAD.Demo.Extensions
{
    public static class UIExtension
    {
        private static string _root;
        public static CustomizationSection MainSection
        {
            get
            {
                var mainCuix = $"{Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable("MENUNAME")}.cuix";
                var mainSection = new CustomizationSection(mainCuix);
                return mainSection;
            }
        }

        public static MacroGroup CreateMacroGroup(string groupName)
        {
            var customSection = new CustomizationSection()
            {
                MenuGroupName = groupName
            };
            var group = new MacroGroup($"{groupName}", customSection.MenuGroup);
            return group;
        }

        /// <summary>
        /// 创建宏
        /// </summary>
        /// <param name="group"></param>
        /// <param name="commandName"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static MenuMacro CreateMenuMacro(this MacroGroup group, string commandName, string imagePath)
        {
            var menuMacro = new MenuMacro(group, $"{commandName}", commandName, $"{commandName}");
            menuMacro.macro.LargeImage = imagePath;
            return menuMacro;
        }

        /// <summary>
        /// 创建菜单栏
        /// </summary>
        /// <param name="section"></param>
        /// <param name="tabName"></param>
        /// <returns></returns>
        public static RibbonTabSource CreateTab(this CustomizationSection section, string tabName)
        {
            var ribbonRoot = section.MenuGroup.RibbonRoot;
            var tab = new RibbonTabSource(ribbonRoot)
            {
                Name = $"{tabName}_ironman",
                Text = tabName
            };
            ribbonRoot.RibbonTabSources.Add(tab);
            return tab;
        }

        /// <summary>
        /// 创建面板
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="panelName"></param>
        /// <returns></returns>
        public static RibbonPanelSource CreatePanel(this RibbonTabSource tab, string panelName)
        {
            var section = tab.Parent.CustomizationSection;
            var ribbonRoot = section.MenuGroup.RibbonRoot;

            var panel = new RibbonPanelSource(ribbonRoot)
            {
                Text = panelName,
                Name = $"{panelName}_ironman"
            };
            ribbonRoot.RibbonPanelSources.Add(panel);
            return panel;
        }

        /// <summary>
        /// 创建<see cref="RibbonRow"/>
        /// </summary>
        /// <param name="panelSource"></param>
        /// <returns></returns>
        public static RibbonRow CreateRibbonRow(this RibbonPanelSource panelSource)
        {
            var row = new RibbonRow(panelSource);
            panelSource.Items.Add(row);

            var parent = panelSource.Parent as RibbonRoot;

            var tabSource = parent.RibbonTabSources[0];
            var ribbonPanelSourceReference = new RibbonPanelSourceReference(tabSource)
            {
                PanelId = panelSource.ElementID,
            };
            tabSource.Items.Add(ribbonPanelSourceReference);
            return row;

            //var ribbonRow = new RibbonRow(panelSource);
            //panelSource.Items.Add(ribbonRow);

            //var ribbonPanelSourceReference = new RibbonPanelSourceReference(tabSource)
            //{
            //    PanelId = panelSource.ElementID,
            //};
            //tabSource.Items.Add(ribbonPanelSourceReference);
        }

        /// <summary>
        /// 创建普通按钮
        /// </summary>
        /// <param name="row"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static RibbonRow CreatePushButton(this RibbonRow row, Action<RibbonCommandButton> action)
        {
            var button = new RibbonCommandButton(row)
            {
                ButtonStyle = RibbonButtonStyle.LargeWithText
            };
            action(button);
            row.Items.Add(button);
            return row;
        }

        /// <summary>
        /// 创建面板分隔符
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static RibbonRow CreateRibbonSeparator(this RibbonRow row)
        {
            row.Items.Add(new RibbonSeparator(row));
            return row;
        }

        /// <summary>
        /// 创建堆叠式按钮
        /// </summary>
        /// <param name="row"></param>
        /// <param name="action1"></param>
        /// <param name="action2"></param>
        /// <param name="action3"></param>
        /// <returns></returns>
        public static RibbonRow CreateStackPanelButton(this RibbonRow row, params Action<RibbonCommandButton>[] actions)
        {
            var panel = new RibbonRowPanel(row);
            row.Items.Add(panel);

            foreach (var item in actions)
            {
                panel.CreateStackPanelButtonItem(item);
            }
            return row;
        }

        /// <summary>
        /// 创建一个堆叠按钮
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private static RibbonRow CreateStackPanelButtonItem(this RibbonRowPanel panel, Action<RibbonCommandButton> action)
        {
            var row = new RibbonRow(panel);
            panel.Items.Add(row);
            var btn = new RibbonCommandButton(row)
            {
                ButtonStyle = RibbonButtonStyle.SmallWithText
            };
            action.Invoke(btn);
            row.Items.Add(btn);
            return row;
        }

        /// <summary>
        /// 创建分隔按钮
        /// </summary>
        /// <param name="row"></param>
        /// <param name="action"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public static RibbonRow CreateRibbonSplitButton(this RibbonRow row, Action<RibbonSplitButton> action, params Action<RibbonCommandButton>[] actions)
        {
            var ribbonSplitButton = new RibbonSplitButton(row)
            {
                ButtonStyle = RibbonButtonStyle.LargeWithText,
            };
            action.Invoke(ribbonSplitButton);

            foreach (var item in actions)
            {
                ribbonSplitButton.CreateRibbonSplitButtonItem(item);
            }
            row.Items.Add(ribbonSplitButton);
            return row;

        }

        /// <summary>
        /// 创建分割按钮条目
        /// </summary>
        /// <param name="ribbonSplitButton"></param>
        /// <param name="action"></param>
        private static void CreateRibbonSplitButtonItem(this RibbonSplitButton ribbonSplitButton, Action<RibbonCommandButton> action)
        {
            var button = new RibbonCommandButton(ribbonSplitButton)
            {
                ButtonStyle = RibbonButtonStyle.LargeWithText,
            };
            action(button);
            ribbonSplitButton.Items.Add(button);
        }

        public static RibbonPanelSource CreatePushButton(this RibbonPanelSource panel, MecroData data)
        {
            //处理创建前数据
            var location = Assembly.GetCallingAssembly().Location;
            var root = Directory.GetParent(location).FullName;

            //创建宏
            var macroGroup = CreateMacroGroup(data.GroupName);
            var commandMenuMacro = new MenuMacro(data.ParentMacroGroup, data.Name, data.CommandName, data.Tag);
            //var fullImagePath = Path.Combine(root, data.ImagePath);
            var fullImagePath = $"{root}{data.ImagePath}";
            commandMenuMacro.macro.LargeImage = fullImagePath;

            //创建按钮
            var button = new RibbonCommandButton(panel)
            {
                ButtonStyle = RibbonButtonStyle.LargeWithText,
                Text = data.CommandName,
                MacroID = commandMenuMacro.ElementID
            };
            //添加按钮
            panel.Items.Add(button);
            return panel;
        }





    }
}
