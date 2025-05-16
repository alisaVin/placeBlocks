using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Microsoft.Win32;
using placing_block.src;
using placing_block.src.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;


namespace placing_block
{
    public class Commands
    {
        Control _ctrl;
        ExcelReader exReader = new ExcelReader();
        IReporter _reporter;

        [CommandMethod("PLACEBLOCK", CommandFlags.Session)]
        public void Demo()
        {
            FormDialog formDlg = new FormDialog { TopMost = false };
            formDlg.Show();
        }

        //public void PlaceBlocks()
        public void PlaceBlocks(string blockPath, string coordPath, string etageInput, object sender, DoWorkEventArgs e)
        {
            _ctrl = e.Argument as Control;
            BackgroundWorker bw = sender as BackgroundWorker;
            if (!File.Exists(coordPath) || !File.Exists(blockPath)) return;

            if (bw.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            var dm = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
            var targetDoc = dm.CurrentDocument;
            var targetDb = dm.MdiActiveDocument.Database;
            var ed = dm.MdiActiveDocument.Editor;

            string blockName = "FSR";

            using (targetDoc.LockDocument(DocumentLockMode.Write, "PLACEBLOCK", "PLACEBLOCK", true))
            {
                var blockData = exReader.ReadInputData(coordPath);
                var validBlocks = blockData.Where(b => b.X >= 0 && b.Y >= 0 && b.Etage == etageInput)
                                          .ToList();
                var firstBlocks = new List<BlockDataModel>();

                for (int i = 0; i < 50; i++)
                {
                    if (validBlocks[i] != null)
                    {
                        firstBlocks.Add(validBlocks[i]);
                    }
                }

                try
                {
                    bool success = false;
                    for (int i = 0; i < firstBlocks.Count; i++)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        Thread.Sleep(50);
                        Invoker.Invoke(() =>
                        {
                            Database sourceDb = AcadUtils.OpenDb(blockPath, _reporter);
                            if (sourceDb == null) return;
                            success = InsertProcess(blockName, targetDb, sourceDb, firstBlocks);
                        }, _ctrl);

                        if (bw.CancellationPending == true || success == false)
                        {
                            e.Cancel = true;
                            break;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    _reporter.ReportExeption(ex);
                    ed.WriteMessage($"\n Error during copy: {ex.Message} \n {ex.StackTrace}");
                }
            }
        }

        private bool InsertProcess(string blockName, Database targetDb, Database sourceDb, List<BlockDataModel> validBlocks)
        {
            using (sourceDb)
            {
                //sourceDb.ReadDwgFile(blockPath, FileOpenMode.OpenForReadAndReadShare, true, string.Empty);

                //Input Feld (block name) ///////////////
                var blockDefId = AcadUtils.GetBlockDef(sourceDb, blockName);
                // Reporter ''''''''''''''''
                if (blockDefId == null)
                {
                    _reporter.WriteText("The block doesn't exist in this drawing");
                    return false;
                }

                using (Transaction tr = targetDb.TransactionManager.StartTransaction())
                {
                    #region copy block into dwg
                    var bt = tr.GetObject(targetDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                    var ms = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    var blIds = new ObjectIdCollection();

                    if (blockDefId != null)
                        blIds.Add(blockDefId);
                    if (blIds.Count != 0)
                    {
                        var idMapping = new IdMapping();
                        sourceDb.WblockCloneObjects(blIds, targetDb.BlockTableId, idMapping, DuplicateRecordCloning.Replace, false);
                    }
                    else
                    {
                        _reporter?.ClearText();
                        _reporter?.WriteText("\nNo block definition found.");
                        return false;
                    }
                    #endregion

                    #region set attributes to copied blocks
                    List<Point3d> insertPoints = new List<Point3d>(); //Einfhügepunkt - Zentrum
                    List<AttributesModel> lstAttrData = new List<AttributesModel>();
                    foreach (var b in validBlocks)
                    {
                        insertPoints.Add(new Point3d(b.X, b.Y, 0));
                        lstAttrData.AddRange(new List<AttributesModel>
                        {
                            //new AttributesModel { Name = "PUNKTNUMMER", Value = b.PunktNum },
                            //new AttributesModel { Name = "TA_ID", Value = b.TAId },
                            new AttributesModel { Name = "TA_BEZEICHNUNG", Value = b.TABezeichnung },
                            //new AttributesModel { Name = "TA_GRUPPE", Value = b.TAGruppe }
                            new AttributesModel { Name = "Geschoss", Value = b.Etage }
                        });
                    }

                    var blBtrID = AcadUtils.GetBlockDef(targetDb, blockName);
                    if (blBtrID.IsNull) return false;
                    for (int i = 0; i < insertPoints.Count; i++)
                    {
                        var newBr = new BlockReference(insertPoints[i], blBtrID);
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

                    //SwapBlockIds - soll schauen, wie man die Ids von FAMOS zu den Blöcken zuweist
                    ms.Dispose();
                    tr.Commit();
                }
            }
            return true;
        }

        private void SetAttributeData(Transaction tr, BlockTableRecord bd, BlockReference bRef, List<AttributesModel> lstAttrData)
        {
            if ((bd == null) || !bd.HasAttributeDefinitions)
                return;

            //attribute auslesen
            if (bRef != null)
            {
                Autodesk.AutoCAD.DatabaseServices.AttributeCollection attrColl = bRef.AttributeCollection;
                foreach (ObjectId adId in bd)
                {
                    var adObj = tr.GetObject(adId, OpenMode.ForRead);
                    AttributeDefinition ad = adObj as AttributeDefinition;
                    if (ad != null)
                    {
                        using (var attrRef = new AttributeReference())
                        {
                            attrRef.SetAttributeFromBlock(ad, bRef.BlockTransform);
                            var modelEntity = lstAttrData.FirstOrDefault(b => b.Name == "TA_BEZEICHNUNG");
                            if (modelEntity != null)
                            {
                                attrRef.Tag = modelEntity.Name;
                                attrRef.TextString = modelEntity.Value;
                            }
                            bRef.AttributeCollection.AppendAttribute(attrRef);
                            tr.AddNewlyCreatedDBObject(attrRef, true);
                        }
                    }
                }
            }
        }

        [CommandMethod("RegisterApp", CommandFlags.Session)]
        public void RegisterApp()
        {
            try
            {
                string sAppName = "PlacingBlock";

                string sProdKey = HostApplicationServices.Current.UserRegistryProductRootKey; //"Software\\Autodesk\\AutoCAD\\R24.1\\ACAD-5101:407"	
                Microsoft.Win32.RegistryKey regAcadProdKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(sProdKey);
                Microsoft.Win32.RegistryKey regAcadAppKey = regAcadProdKey.OpenSubKey("Applications", true); //{HKEY_CURRENT_USER\Software\Autodesk\AutoCAD\R24.1\ACAD-5101:407}

                using (regAcadAppKey)
                {
                    //var regAppAddInKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(regAcadAppKey.ToString()) ?? Microsoft.Win32.Registry.CurrentUser.CreateSubKey(sAppName);
                    string[] subKeys = regAcadAppKey.GetSubKeyNames();
                    foreach (string subKey in subKeys)
                    {
                        if (subKey.Equals(sAppName))
                            return;
                    }
                    string sAssemblyPath = Assembly.GetExecutingAssembly().Location;

                    Microsoft.Win32.RegistryKey regAppAddInKey = regAcadAppKey.CreateSubKey(sAppName);
                    regAppAddInKey.SetValue("DESCRIPTION", sAppName, RegistryValueKind.String);
                    regAppAddInKey.SetValue("LOADCTRLS", 2, RegistryValueKind.DWord);
                    regAppAddInKey.SetValue("LOADER", sAssemblyPath, RegistryValueKind.String);
                    regAppAddInKey.SetValue("MANAGED", 1, RegistryValueKind.DWord);
                }
            }
            catch (System.Exception ex)
            {
                _reporter.ReportExeption(ex);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
