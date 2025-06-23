using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Teigha.DatabaseServices;
using Teigha.Geometry;
using Teigha.Runtime;
using ViewModels;
using Views;


namespace Commands
{
    public class ARESCommands
    {
        Control _ctrl;
        readonly ExcelReader exReader = new ExcelReader();
        readonly IReporter _reporter;

        #region Place block command
        [CommandMethod("PLACEBLOCK", CommandFlags.Session)]
        public void Demo()
        {
            FormDialog formDlg = new FormDialog { TopMost = false };
            formDlg.Show();
        }

        public void PlaceBlocks(string coordPath, string blockPath, string blockName, string etageInput, object sender, DoWorkEventArgs e)
        {
            _ctrl = e.Argument as Control;
            BackgroundWorker bw = sender as BackgroundWorker;
            if (!File.Exists(coordPath) || !File.Exists(blockPath)) return;

            if (bw.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            var targetDoc = Teigha.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            using (targetDoc.LockDocument())
            {
                var targetDb = targetDoc.Database;
                var ed = targetDoc.Editor;
                var blockData = exReader.ReadInputData(coordPath, blockName, etageInput);
                var validBlocks = blockData.Where(b => b.X > 0 && b.Y > 0 && b.Etage == etageInput)
                                          .ToList();
                try
                {
                    bool success = false;
                    bw.ReportProgress(0);
                    System.Windows.Forms.Application.DoEvents();
                    Invoker.Invoke(() =>
                    {
                        Database sourceDb = AcadUtils.OpenDb(blockPath, _reporter);
                        if (sourceDb == null) return;
                        success = InsertProcess(blockName, targetDb, sourceDb, validBlocks);
                    }, _ctrl);
                    bw.ReportProgress(50);
                    Thread.Sleep(50);
                    bw.ReportProgress(100);
                    if (bw.CancellationPending == true || success == false)
                        e.Cancel = true;
                }
                catch (System.Exception ex)
                {
                    _reporter?.ReportExeption(ex);
                    ed.WriteMessage($"\n Error during copy: {ex.Message} \n {ex.StackTrace}");
                }
            }
        }

        private bool InsertProcess(string blockName, Database targetDb, Database sourceDb, List<BlockDataModel> validBlocks)
        {
            using (sourceDb)
            {
                string layerName = "techAnlage_" + blockName;

                #region copy block into dwg

                using (var trans = sourceDb.TransactionManager.StartOpenCloseTransaction())
                {
                    var layerTable = trans.GetObject(sourceDb.LayerTableId, OpenMode.ForRead) as LayerTable;
                    if (!layerTable.Has(layerName)) return false;
                    var layIds = new ObjectIdCollection();
                    var blLayId = layerTable[layerName];
                    layIds.Add(blLayId);

                    ObjectId blDefId = AcadUtils.GetBlockDef(sourceDb, blockName);
                    var blIds = new ObjectIdCollection();
                    if (!blDefId.IsNull)
                        blIds.Add(blDefId);

                    if (blIds.Count != 0 && layIds.Count != 0)
                    {
                        var mapping = new IdMapping();
                        sourceDb.WblockCloneObjects(layIds, targetDb.LayerTableId, mapping, DuplicateRecordCloning.Replace, false);

                        var idMapping = new IdMapping();
                        sourceDb.WblockCloneObjects(blIds, targetDb.BlockTableId, idMapping, DuplicateRecordCloning.Replace, false);
                    }
                    else
                    {
                        _reporter?.ClearText();
                        _reporter?.WriteText("\nNo block definition found.");
                        return false;
                    }
                }

                #endregion

                #region set attributes to copied blocks
                List<Point3d> insertPoints = new List<Point3d>();
                List<AttributesModel> lstAttrData = new List<AttributesModel>();
                foreach (var b in validBlocks)
                {
                    insertPoints.Add(new Point3d(b.X, b.Y, 0));
                    lstAttrData.Add(

                        //new AttributesModel { Name = "PUNKTNUMMER", Value = b.PunktNum },
                        //new AttributesModel { Name = "TA_ID", Value = b.TAId },
                        new AttributesModel { Name = "TA_BEZEICHNUNG", Value = b.TABezeichnung }
                        //new AttributesModel { Name = "TA_GRUPPE", Value = b.TAGruppe }
                        //new AttributesModel { Name = "Geschoss", Value = b.Etage }
                    );
                }

                List<Point3d> transformPoints = TransformCoordinates(insertPoints);
                using (Transaction tr = targetDb.TransactionManager.StartTransaction())
                {
                    var blBtrID = AcadUtils.GetBlockDef(targetDb, blockName);
                    var bt = tr.GetObject(targetDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                    var ms = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    try
                    {
                        if (blBtrID.IsNull) return false;
                        for (int i = 0; i < insertPoints.Count; i++)
                        {
                            var newBr = new BlockReference(transformPoints[i], blBtrID);
                            newBr.Layer = layerName;
                            newBr.Visible = true;
                            ms.AppendEntity(newBr);
                            tr.AddNewlyCreatedDBObject(newBr, true);

                            using (var blDef = tr.GetObject(blBtrID, OpenMode.ForRead) as BlockTableRecord)
                            {
                                if (blDef == null || !blDef.HasAttributeDefinitions)
                                    return false;

                                SetAttributeData(tr, blDef, newBr, lstAttrData);
                            }
                        }
                        #endregion

                        ms.Dispose();
                        tr.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        _reporter?.ReportExeption(ex);
                    }
                }
            }
            return true;
        }

        private void SetAttributeData(Transaction tr, BlockTableRecord bd, BlockReference bRef, List<AttributesModel> lstAttrData)
        {
            if ((bd == null) || !bd.HasAttributeDefinitions)
                return;

            if (bRef != null)
            {
                Teigha.DatabaseServices.AttributeCollection attrColl = bRef.AttributeCollection;
                foreach (ObjectId adId in bd)
                {
                    var adObj = tr.GetObject(adId, OpenMode.ForWrite); //!!!
                    AttributeDefinition ad = adObj as AttributeDefinition;
                    if (ad != null)
                    {
                        using (var attrRef = new AttributeReference())
                        {
                            attrRef.SetAttributeFromBlock(ad, bRef.BlockTransform);
                            var modelEntity = lstAttrData.FirstOrDefault(b => b.Name == "TA_BEZEICHNUNG");
                            if (modelEntity != null && attrRef.Tag == "ATTR1")
                            {
                                attrRef.Tag = modelEntity.Name;
                                attrRef.TextString = modelEntity.Value;
                                lstAttrData.Remove(modelEntity);
                            }
                            else
                            {
                                continue;
                            }
                            bRef.AttributeCollection.AppendAttribute(attrRef);
                            tr.AddNewlyCreatedDBObject(attrRef, true);
                        }
                    }
                }
            }
        }

        public List<Point3d> TransformCoordinates(List<Point3d> originalCoords)
        {
            List<Point3d> rotatedCoords = new List<Point3d>();

            // -90° Rotation im Uhrzeigersinn: (x,y) -> (-y, x)
            foreach (Point3d coord in originalCoords)
            {
                double newX = -coord.Y;
                double newY = coord.X;
                rotatedCoords.Add(new Point3d(newX, newY, 0));
            }
            // Finden minimalen X-Wert für Offset-Berechnung
            double minX = double.MaxValue;
            // Offset der X berechnen
            double xOffset = Math.Abs(minX);

            List<Point3d> finalCoords = new List<Point3d>();
            foreach (Point3d coord in rotatedCoords)
            {
                finalCoords.Add(new Point3d(coord.X + xOffset, coord.Y, 0));
            }
            return finalCoords;
        }
        #endregion

        #region Register commands
        [CommandMethod("RegApp", CommandFlags.Session)]
        public void RegisterAppOnDemand()
        {
            DemandLoading.RegisterForDemandLoading();
        }

        [CommandMethod("UnregApp", CommandFlags.Session)]
        public void UnregisterApp()
        {
            DemandLoading.UnregisterForDemandLoading();
        }
        #endregion
    }
}
