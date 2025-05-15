using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Microsoft.Win32;
using placing_block.src;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;


namespace placing_block
{
    public class Commands
    {
        Control _ctrl;
        ExcelReader exReader = new ExcelReader();
        Reporter _reporter;

        [CommandMethod("PLACEBLOCK", CommandFlags.Session)]
        public void Demo()
        {
            FormDialog formDlg = new FormDialog { TopMost = false };
            formDlg.Show();
        }

        //public void PlaceBlocks()
        public void PlaceBlocks(string blockPath, string coordPath, object sender, DoWorkEventArgs e)
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

                try
                {  //Exception repeted DWG readed
                    Invoker.Invoke(() =>
                    {
                        Database sourceDb = AcadUtils.OpenDb(blockPath, _reporter);
                        using (sourceDb)
                        {
                            //sourceDb.ReadDwgFile(blockPath, FileOpenMode.OpenForReadAndReadShare, true, string.Empty);
                            if (sourceDb == null) return;
                            //Input Feld (block name) ///////////////
                            var blockDefId = AcadUtils.GetBlockDef(sourceDb, blockName);
                            // Reporter ''''''''''''''''
                            if (blockDefId == null)
                            {
                                _reporter.WriteText("The block doesn't exist in this drawing");
                                ed.WriteMessage("The block doesn't exist in this drawing");
                                return;
                            }

                            using (Transaction tr = targetDb.TransactionManager.StartTransaction())
                            {
                                #region copy block into dwg
                                var bt = tr.GetObject(targetDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                                var ms = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                                var blIds = new ObjectIdCollection();

                                if (blockDefId != null)
                                    blIds.Add(blockDefId);
                                var idMapping = new IdMapping();
                                sourceDb.WblockCloneObjects(blIds, targetDb.BlockTableId, idMapping, DuplicateRecordCloning.Replace, false);

                                //reporter ####################################

                                ed.WriteMessage($"\nCopied {blIds.Count.ToString()} block definitions from {blockPath} to the current drawing.");
                                #endregion

                                #region set attributes to copied blocks
                                List<Point3d> insertPoints = new List<Point3d>(); //Einfhügepunkt - Zentrum
                                foreach (var blPoint in blockData)
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
                                            Autodesk.AutoCAD.DatabaseServices.AttributeCollection attrColl = newBr.AttributeCollection;
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
                                                        attrRef.TextString = $"{blockData[i].X}, {blockData[i].Y}";
                                                        newBr.AttributeCollection.AppendAttribute(attrRef);
                                                        tr.AddNewlyCreatedDBObject(attrRef, true);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                #endregion

                                //SwapBlockIds - soll schauen, wie man die Ids von FAMOS zu den Blöcken zuweist
                                ms.Dispose();
                                tr.Commit();
                            }
                        }
                    }, _ctrl);
                }
                catch (Exception ex)
                {
                    _reporter.ReportExeption(ex);
                    ed.WriteMessage($"\n Error during copy: {ex.Message} \n {ex.StackTrace}");
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
