using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using IronMan.Acad.Demo.BasicApi;
using IronMan.Acad.Demo.Command;
using IronMan.Acad.Demo.Extensions;
using Stark.Extensions.Acad;

[assembly: CommandClass(typeof(AnnotatedAreaCommand))]
namespace IronMan.Acad.Demo.Command
{
    internal class AnnotatedAreaCommand : CommandBase
    {
        [CommandMethod(nameof(AnnotatedAreaByLayer))]
        public void AnnotatedAreaByLayer()
        {
            var prompt = new PromptEntityOptions("\n选择一个线条以获取其图层");
            var result = Editor.GetEntity(prompt);
            if (result.Status != PromptStatus.OK)
            {
                return;
            }
            Database.NewTransaction(trans =>
            {
                var entity = (Entity)trans.GetObject(result.ObjectId, OpenMode.ForRead);
                if (entity == null)
                {
                    Editor.WriteMessage("\n选择的对象无效");
                    return;
                }

                var layerName = entity.Layer;

                var blockTable = (BlockTable)trans.GetObject(Database.BlockTableId, OpenMode.ForRead);
                var modelSpace = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                var units = Database.Insunits;
                var successCount = 0;
                foreach (var id in modelSpace)
                {
                    var currentEntity = (Entity)trans.GetObject(id, OpenMode.ForWrite);
                    if (currentEntity.LayerId == entity.LayerId)
                    {
                        if (currentEntity is Autodesk.AutoCAD.DatabaseServices.Polyline polyline && polyline.Closed)
                        {
                            var text = new MText();
                            text.TextHeight = 10;
                            var area = polyline.Area;
                            text.Contents = $"{area / 1e6:F2}㎡";
                            text.Location = polyline.GetBoxMidPoint();
                            modelSpace.AppendEntity(text);
                            trans.AddNewlyCreatedDBObject(text, true);
                            successCount++;
                        }
                    }
                }
                Editor.WriteMessage($"\n已成功标注{successCount}个闭合区域");
            });

        }


        [CommandMethod(nameof(AnnotatedAreaByPoint))]
        public void AnnotatedAreaByPoint()
        {
            var prompt = new PromptPointOptions("\n请选择封闭区域内的一个点");
            var result = Editor.GetPoint(prompt);
            if (result.Status != PromptStatus.OK)
            {
                return;
            }
            var clickPoint = result.Value;
            Database.NewTransaction(trans =>
            {
                var blockTable = (BlockTable)trans.GetObject(Database.BlockTableId, OpenMode.ForRead);
                var record = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                foreach (var objId in record)
                {
                    var entity = (Entity)trans.GetObject(objId, OpenMode.ForWrite);
                    if (entity is Autodesk.AutoCAD.DatabaseServices.Polyline polyline && polyline.Closed)
                    {
                        if (GeometryUtil.IsPointInsidePolygon(clickPoint, polyline))
                        {
                            var area = polyline.Area;
                            var units = Database.Insunits;
                            var text = new MText();
                            text.Contents = $"{area / 1e6:F2}㎡";
                            text.TextHeight = 10;
                            text.Location = clickPoint;
                            record.AppendEntity(text);
                            trans.AddNewlyCreatedDBObject(text, true);
                            clickPoint = clickPoint.Add(new Vector3d(0, 10, 0));
                        }
                    }
                }


            });
        }


        [CommandMethod(nameof(CircularAnnotatedArea))]
        public void CircularAnnotatedArea()
        {
            Database.NewTransaction(trans =>
            {
                var blockTable = (BlockTable)trans.GetObject(Database.BlockTableId, OpenMode.ForRead);
                var modelSpace = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                var layerTable = (LayerTable)trans.GetObject(Database.LayerTableId, OpenMode.ForRead);
                //创建一个图层用来放置边界线与面积
                if (!layerTable.Has("面积"))
                {
                    using var newLayer = new LayerTableRecord();
                    newLayer.Name = "面积";
                    newLayer.Color = Color.FromRgb(255, 255, 0);
                    layerTable.UpgradeOpen();
                    layerTable.Add(newLayer);
                    trans.AddNewlyCreatedDBObject(newLayer, true);
                }
            });

            //设置获取坐标选项
            var prompt = new PromptPointOptions("\n请选择闭合区域内部或[设置文字高度(H)]")
            {
                AllowNone = true
            };
            prompt.Keywords.Add("H");//加入关键字D
            var textHeight = 10.0;
            //循环放置
            while (true)
            {
                var pointResult = Editor.GetPoint(prompt);

                if (pointResult.Status == PromptStatus.Keyword)
                {
                    switch (pointResult.StringResult)
                    {
                        case "H":
                            var doubleResult = Editor.GetDouble("\n输入文字高度<" + textHeight + ">");//提示用户设置文字高度
                            if (doubleResult.Status == PromptStatus.OK && doubleResult.Value > 0)
                            {
                                textHeight = doubleResult.Value;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (pointResult.Status == PromptStatus.OK)
                {
                    var pt1 = pointResult.Value;
                    //根据坐标获取到边界线集合，(false为不进行孤岛检测)
                    //true表示如果当前闭合轮廓内还有闭合轮廓，也会统计
                    //false表示如果当前闭合轮廓内还有闭合轮廓，不会统计，即返回数量为1
                    //这个方法，返回数量不稳定！！！，时好时坏
                    var dbc = Editor.TraceBoundary(pt1, false);
                    if (dbc.Count == 1)//如果成功检测到边界线，会得到1条曲线，判断数量是否为1（因为不进行孤岛检测所以只能是1或者0）
                    {
                        if (dbc[0] is Curve cur && cur.Closed)//以防万一再判断一下dbc里的对象是不是闭合曲线
                        {
                            cur.Layer = "面积";
                            var area = cur.Area / 1e6;
                            var midpt = pt1;

                            var text1 = new DBText()//创建一个文字
                            {
                                TextString = "面积：" + area.ToString("0.00") + "m²",//设置文字内容
                                Height = textHeight,//文字高度
                                Position = midpt,//插入点
                                VerticalMode = TextVerticalMode.TextVerticalMid,//设置对齐方式为正中
                                HorizontalMode = TextHorizontalMode.TextLeft,
                                AlignmentPoint = midpt,//设置对齐点
                                //WidthFactor = 0.7,//设置宽度因子
                                Layer = "面积",
                            };

                            Database.NewTransaction(trans =>
                            {
                                var blockTable = (BlockTable)trans.GetObject(Database.BlockTableId, OpenMode.ForRead);
                                var modelSpace = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                                modelSpace.AppendEntity(text1);
                                trans.AddNewlyCreatedDBObject(text1, true);
                            });

                        }

                    }
                }
                else
                {
                    return;
                }
            }
        }
    }


}
