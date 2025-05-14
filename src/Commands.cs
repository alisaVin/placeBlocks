using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using placing_block.src;
using System.Collections.Generic;
using System.Windows.Forms;
namespace placing_block
{
    public class Commands
    {
        Control _ctrl;
        ExcelReader exReader = new ExcelReader();
        //Reporter _reporter;

        [CommandMethod("PLACEBLOCK", CommandFlags.Session)]
        //public void Demo()
        //{
        //    FormDialog formDlg = new FormDialog { TopMost = false };
        //    formDlg.Show();
        //}

        //public void PlaceBlocks(string blockPath, string coordPath, object sender, DoWorkEventArgs e)
        public void PlaceBlocks()
        {
            //_ctrl = e.Argument as Control;
            //BackgroundWorker bw = sender as BackgroundWorker;
            //if (!File.Exists(coordPath) || !File.Exists(blockPath)) return;

            //if (bw.CancellationPending)
            //{
            //    e.Cancel = true;
            //    return;
            //}

            var dm = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
            var targetDoc = dm.CurrentDocument;
            var targetDb = dm.MdiActiveDocument.Database;
            var ed = dm.MdiActiveDocument.Editor;
            //Input Feld ///////////////
            string blockPath = "C:\\Projects\\Bundesverwaltungsgericht\\placing_block\\Blöcke-techn Anlagen\\FSR.dwg";
            string blockName = "FSR";

            using (targetDoc.LockDocument(DocumentLockMode.Write, "PLACEBLOCK", "PLACEBLOCK", true))
            {
                var blockPoints = exReader.ReadInputData();

                try
                {
                    //Invoker.Invoke(() =>
                    //{
                    //mal fragen, ob man DWG-Datei öffnen soll oder nicht (bzw. ohne Öffnung, wie bei copy_file_async)

                    //Database sourceDb = AcadUtils.OpenDb(blockPath.StringResult, _reporter);
                    using (var sourceDb = new Database(false, true))
                    {
                        sourceDb.ReadDwgFile(blockPath, FileOpenMode.OpenForReadAndReadShare, true, string.Empty);
                        if (sourceDb == null) return;
                        //Input Feld (block name) ///////////////
                        var blockDefId = AcadUtils.GetBlockDef(sourceDb, blockName);
                        // Reporter ''''''''''''''''
                        if (blockDefId == null)
                        {
                            ed.WriteMessage("The block doesn't exist in this drawing");
                            return;
                        }

                        using (Transaction tr = targetDb.TransactionManager.StartTransaction())
                        {
                            var bt = tr.GetObject(targetDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                            var ms = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                            var blIds = new ObjectIdCollection();

                            if (blockDefId != null)
                                blIds.Add(blockDefId);

                            var idMapping = new IdMapping();
                            sourceDb.WblockCloneObjects(blIds, targetDb.BlockTableId, idMapping, DuplicateRecordCloning.Replace, false);

                            //reporter ####################################
                            ed.WriteMessage($"\nCopied {blIds.Count.ToString()} block definitions from {blockPath} to the current drawing.");

                            List<Point3d> insertPoints = new List<Point3d>(); //Einfhügepunkt - Zentrum

                            foreach (var blPoint in blockPoints)
                            {
                                insertPoints.Add(new Point3d(blPoint.X, blPoint.Y, 0));
                            }


                            var blBtr = AcadUtils.GetBlockDef(targetDb, blockName);
                            if (blBtr.IsNull) return;

                            for (int i = 0; i < insertPoints.Count; i++)
                            {
                                var newBr = new BlockReference(insertPoints[i], blBtr);
                                ms.AppendEntity(newBr);
                                tr.AddNewlyCreatedDBObject(newBr, true);

                                using (var blDef = tr.GetObject(blBtr, OpenMode.ForRead) as BlockTableRecord)
                                {
                                    if (blDef == null || !blDef.HasAttributeDefinitions)
                                        return;

                                    //attribute auslesen
                                    if (newBr != null)
                                    {
                                        AttributeCollection attrColl = newBr.AttributeCollection;
                                        foreach (ObjectId adId in blDef)
                                        {
                                            var adObj = tr.GetObject(adId, OpenMode.ForRead);
                                            AttributeDefinition ad = adObj as AttributeDefinition;
                                            if (ad != null)
                                            {
                                                using (var attrRef = new AttributeReference())
                                                {
                                                    attrRef.SetAttributeFromBlock(ad, newBr.BlockTransform);
                                                    attrRef.Tag = "Punktkoordinaten";
                                                    attrRef.TextString = $"{blockPoints[i].X}, {blockPoints[i].Y}";
                                                    newBr.AttributeCollection.AppendAttribute(attrRef);
                                                    tr.AddNewlyCreatedDBObject(attrRef, true);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            //SwapBlockIds - soll schauen, wie man die Ids von FAMOS zu den Blöcken zuweist

                            ms.Dispose();
                            tr.Commit();
                        }
                    }
                    //}, _ctrl);
                }
                catch (Exception ex)
                {
                    //_reporter.ReportExeption(ex);
                    ed.WriteMessage($"\n Error during copy: {ex.Message} \n {ex.StackTrace}");
                }
            }

        }
    }
}
