using Kofax.Capture.DBLite;
using Kofax.Capture.SDK.CustomModule;
using Kofax.Capture.SDK.Data;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bregs.Kofax.Docusign
{
    public enum ProcessMode
    {
        ProcessByBatch,
        ProcessByDoc
    }
    internal class BatchManager
    {
        private Login _login;
        private IBatch _activeBatch;
        private string _uniqueId;
        private int _processId;
        private IRuntimeSession _runTimeSession;
        private string _strXmlRtExp;   // *** XML Runtime Export file
        private string _strXmlRtImp;   // *** XML Runtime Import file
        private string _strXmlSuExp;   // *** XML Setup Export file
        private string _strXmlSuImp;   // *** XML Setup Import file
        private string _strXmlDocExp; // *** XML Document Export file
        private string _strXmlDocImp; // *** XML Document Export file
        private string _strXmlPath; // *** Path for XML files

        private ProcessMode _processMode;
        private bool _isDocOpen;
        private short _FirstDoc; // *** start of the range of open documents
        private short _LastDoc;   // *** end of the range of documents
        private const String absAscentRegistryLocation = @"Software\Kofax Image Products\Ascent Capture\3.0";

        // **************************************************************************
        // *** Property:  UniqueID
        // *** Purpose:   Unique identifier that specifies the queue.
        // **************************************************************************
        public string UniqueID
        {
            get
            {
                return _uniqueId;
            }
        }

        // **************************************************************************
        // *** Property:  XmlRuntimeExportFile
        // *** Purpose:   XML file of batch data created when batch is opened.
        // *** Notes:     If a relative file name is passed, a path in the
        // ***            Ascent Local directory will be used.
        // **************************************************************************
        public string XmlRuntimeExportFile
        {
            get
            {
                return _strXmlRtExp;
            }
            set
            {
                if (Strings.Len(value) > 0 & Strings.InStr(value, @"\") == 0)
                {
                    value = xml_Path + value;
                }
                _strXmlRtExp = value;
            }
        }

        // **************************************************************************
        // *** Property:  XmlRuntimeImportFile
        // *** Purpose:   XML file of batch data used to populate the database when
        // ***            batch is closed.
        // **************************************************************************
        public string XmlRuntimeImportFile
        {
            get
            {
                return _strXmlRtImp;
            }
            set
            {
                if (Strings.Len(value) > 0 & Strings.InStr(value, @"\") == 0)
                {
                    value = xml_Path + value;
                }
                _strXmlRtImp = value;
            }
        }

        // **************************************************************************
        // *** Property:  XmlSetupExportFile
        // *** Purpose:   XML file of batch class data created when batch is opened.
        // *** Notes:     No setup data is exported if this value is blank.
        // **************************************************************************
        public string XmlSetupExportFile
        {
            get
            {
                return _strXmlSuExp;
            }
            set
            {
                if (Strings.Len(value) > 0 & Strings.InStr(value, @"\") == 0)
                {
                    value = xml_Path + value;
                }
                _strXmlSuExp = value;
            }
        }

        // **************************************************************************
        // *** Property:  XmlSetupImportFile
        // *** Purpose:   XML file of batch class data used to populate the database
        // ***            when batch is closed.
        // *** Notes:     No setup data is imported if this value is blank.
        // **************************************************************************
        public string XmlSetupImportFile
        {
            get
            {
                return _strXmlSuImp;
            }
            set
            {
                if (Strings.Len(value) > 0 & Strings.InStr(value, @"\") == 0)
                {
                    value = xml_Path + value;
                }
                _strXmlSuImp = value;
            }
        }

        // **************************************************************************
        // *** Property:  XmlDocumentExportFile
        // *** Purpose:   XML file of document data created when docs are opened.
        // *** Notes:     If a relative file name is passed, a path in the
        // ***            Ascent Local directory will be used.
        // **************************************************************************
        public string XmlDocumentExportFile
        {
            get
            {
                return _strXmlDocExp;
            }
            set
            {
                if (Strings.Len(value) > 0 & Strings.InStr(value, @"\") == 0)
                {
                    value = xml_Path + value;
                }
                _strXmlDocExp = value;
            }
        }

        // **************************************************************************
        // *** Property:  XmlDocumentImportFile
        // *** Purpose:   XML file of document data used to populate database when
        // ***            the documents are closed.
        // **************************************************************************
        public string XmlDocumentImportFile
        {
            get
            {
                return _strXmlDocImp;
            }
            set
            {
                if (Strings.Len(value) > 0 & Strings.InStr(value, @"\") == 0)
                {
                    value = xml_Path + value;
                }
                _strXmlDocImp = value;
            }
        }

        // **************************************************************************
        // *** Property:  ActiveBatch
        // *** Purpose:   Returns currently selected batch
        // **************************************************************************
        public IBatch ActiveBatch
        {
            get
            {
                return _activeBatch;
            }
        }

        // **************************************************************************
        // *** Property:  DocumentCount
        // *** Purpose:   Number of documents in the currently selected batch
        // **************************************************************************
        public int DocumentCount
        {
            get
            {
                if (_activeBatch is null)
                {
                    return 0;
                }
                else
                {
                    return _activeBatch.DocumentCount;
                }
            }
        }

        // **************************************************************************
        // *** Property:  IsDocumentOpen
        // *** Purpose:   Returns True if docs are currently exported
        // **************************************************************************
        public bool IsDocumentOpen
        {
            get
            {
                return _isDocOpen;
            }
        }

        // **************************************************************************
        // *** Function:  CustomStorageString
        // *** Purpose:   Retrieves the named storage string from the specified batch
        // ***            the batch is not opened
        // *** Inputs:    lBatchId - the batch of the string
        // ***            strName - the name of the string
        // *** Outputs:   None.
        // **************************************************************************
        public string get_CustomStorageString(int lBatchId, string strName)
        {
            IBatchCollection oBatchColl = null;
            IBatch oBatch = null;
            try
            {
                try
                {
                    // *** Get the collection of all batches for this queue.
                    oBatchColl = _runTimeSession.get_BatchCollection(KfxDbFilter.KfxDbFilterOnProcess, _processId, 0);


                    // *** Iterate through the batch collection to find the 
                    // *** requested(batch)
                    foreach (IBatch currentOBatch in oBatchColl)
                    {
                        oBatch = currentOBatch;
                        if (oBatch.ExternalBatchID == lBatchId)
                        {
                            break;
                        }
                    }
                }

                catch (KfxException ex)
                {
                    int argnErrCode1 = ex.ErrorCode;
                    int argnErrCode2 = 0;
                    int argnErrCode3 = 0;
                    string argstrSource =  "BatchManager.CustomStorageString";
                    int argnLineNum = 0;
                    string argstrDesc = ex.Message;
                    bool argbRaise = true;
                    _login.LogError(ref argnErrCode1, ref argnErrCode2, ref argnErrCode3, ref argstrSource, ref argnLineNum, ref argstrDesc, ref argbRaise);
                    throw;
                }

                // *** Disable local error handling as errors are "expected" here
                if (!(oBatch == null))
                {
                    return oBatch.get_CustomStorageString(strName);
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                if (oBatch!=null)
                {
                    using (oBatch)
                    {
                    }
                    oBatch = null;
                }
                if (oBatchColl != null)
                {
                    using (oBatchColl)
                    {
                    }
                    oBatchColl = null;
                }
            }
        }

        // **************************************************************************
        // *** Function:  xml_Path
        // *** Purpose:   Determines the path where XML files are created
        // *** Inputs:    None
        // *** Outputs:   Path where XML files are created
        // **************************************************************************
        private string xml_Path
        {
            get
            {

                // *** If the path is already determined, return it.
                if (Strings.Len(_strXmlPath) > 0)
                {
                    return _strXmlPath;
                    return default;
                }

                // *** Read the registry to determine the local path
                string strLocalPath;
                RegistryKey oACRegKey;
                bool bNotWritable = false;
                oACRegKey = Registry.LocalMachine.OpenSubKey(absAscentRegistryLocation, bNotWritable);
                try
                {
                    strLocalPath = oACRegKey.GetValue("LocalPath").ToString();
                }
                finally
                {
                    oACRegKey.Close();
                }

                // *** Append the queue ID in order to create a unique location
                // *** Be sure the UniqueID is set first
                Debug.Assert(Strings.Len(UniqueID) > 0, "");
                strLocalPath = Path.Combine(strLocalPath, UniqueID) + Convert.ToString(Path.DirectorySeparatorChar);

                // *** Create the folder if necessary
                if (!Directory.Exists(strLocalPath))
                {
                    Directory.CreateDirectory(strLocalPath);
                }
                _strXmlPath = strLocalPath;

                return _strXmlPath;
            }
        }

        public BatchManager(string uniqueId)
        {
                _uniqueId = uniqueId;
        }

        public void LoginToRuntimeSession()
        {
            if (_login == null) { 
                
                _login = new Login(); 

                _login.EnableSecurityBoost = true;

                _login.Login("", "");
                _login.ApplicationName = "BregsDocusign";
                _login.Version = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileMajorPart + "." + FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileMinorPart;
                _login.ValidateUser(_uniqueId);
                _runTimeSession = (RuntimeSession)_login.RuntimeSession;
                _processId = _login.ProcessID;
               
            }
        }

        public bool BatchOpen(int lBatchId, ProcessMode OpenMode = ProcessMode.ProcessByBatch)

        {
            bool BatchOpenRet = default;
            // *** Return value
            bool bRet;

            try
            {
                // *** Check to see if a batch is already selected
                //Debug.Assert(_activeBatch is null, "");

                // *** Be sure the runtime session has been set.
                // *** Set the UniqueID
                //Debug.Assert(_runTimeSession is not null, "");

                // *** Set import/export mode
                _processMode = OpenMode;

                // *** Get the collection of all batches for this queue.
                using (var oBatchColl = _runTimeSession.get_BatchCollection(KfxDbFilter.KfxDbFilterOnProcess, _processId, 0))
                {

                    // *** Iterate through the batch collection to find the
                    // *** requested batch
                    foreach (IBatch oBatch in oBatchColl)
                    {
                        if (oBatch.ExternalBatchID == lBatchId)
                        {
                            _activeBatch = oBatch;
                            break;
                        }
                    }
                    // *** Return false if no batches are available
                    if (_activeBatch is null)
                    {
                        bRet = false;
                    }
                    else
                    {
                        // *** Lock the batch
                        batch_Lock();
                        bRet = true;
                    }
                }
                BatchOpenRet = bRet;
            }
            catch (KfxException ex)
            {
                int argnErrCode1 = Information.Err().Number;
                int argnErrCode2 = 0;
                int argnErrCode3 = 0;
                string argstrSource =  "BatchManager.BatchOpen";
                int argnLineNum = 0;
                string argstrDesc = Information.Err().Description;
                bool argbRaise = true;
                _login.LogError(ref argnErrCode1, ref argnErrCode2, ref argnErrCode3, ref argstrSource, ref argnLineNum, ref argstrDesc, ref argbRaise);
                Information.Err().Number = argnErrCode1;
                Information.Err().Description = argstrDesc;
            }

            return BatchOpenRet;
        }

        public bool BatchOpenNext(ProcessMode OpenMode = ProcessMode.ProcessByBatch, bool bExportXML = true)

        {

            try
            {

                // *** Check to see if a batch is already selected
                //Debug.Assert(m_oActiveBatch is null, "");

                // *** Set import/export mode
                _processMode = OpenMode;

                // *** Get the next available batch for this queue               
                _activeBatch = _runTimeSession.NextBatchGet(_processId, KfxDbFilter.KfxDbFilterOnProcess | KfxDbFilter.KfxDbFilterOnStates | KfxDbFilter.KfxDbSortOnPriorityDescending, KfxDbState.KfxDbBatchReady | KfxDbState.KfxDbBatchSuspended);





                if (_activeBatch is null)
                {

                    // *** No batch available. Return False.
                    return false;
                }
                else
                {
                    if (bExportXML)
                    {
                        // *** Export the XML
                        batch_Export();
                    }
                    return true;
                }
            }
            catch
            {
                int argnErrCode1 = Information.Err().Number;
                int argnErrCode2 = 0;
                int argnErrCode3 = 0;
                string argstrSource =  "BatchManger.BatchOpenNext";
                int argnLineNum = Information.Erl();
                string argstrDesc = Information.Err().Description;
                bool argbRaise = true;
                _login.LogError(ref argnErrCode1, ref argnErrCode2, ref argnErrCode3, ref argstrSource, ref argnLineNum, ref argstrDesc, ref argbRaise);
                Information.Err().Number = argnErrCode1;
                Information.Err().Description = argstrDesc;
            }

            return default;
        }

        public void BatchClose(KfxDbState eNewState = KfxDbState.KfxDbBatchReady, int lException = 0, string strException = "", bool bImportXML = true)


        {
            KfxDbQueue eQueue;

            try
            {

                // *** Determine what queue to move to based on state.
                switch (eNewState)
                {
                    case KfxDbState.KfxDbBatchError:
                        {
                            eQueue = KfxDbQueue.KfxDbQueueException;
                            break;
                        }
                    case KfxDbState.KfxDbBatchReady:
                        {
                            eQueue = KfxDbQueue.KfxDbQueueNext;
                            break;
                        }
                    case KfxDbState.KfxDbBatchCompleted:
                        {
                            eQueue = KfxDbQueue.KfxDbQueueSame;
                            break;
                        }

                    default:
                        {
                            eQueue = KfxDbQueue.KfxDbQueueSame;
                            break;
                        }
                }

                // *** Import the XML
                if (bImportXML)
                {
                    //batch_Import();
                }

                // *** Close the batch               
                _activeBatch.BatchClose(eNewState, eQueue, lException, strException);
                batch_Clear();
            }
            catch
            {
                int lError;
                string strError;
                string strSource;
                lError = Information.Err().Number;
                strError = Information.Err().Description;
                strSource = Information.Err().Source;
                batch_Clear();
                int argnErrCode2 = 0;
                int argnErrCode3 = 0;
                int argnLineNum = Information.Erl();
                bool argbRaise = true;
                _login.LogError(ref lError, ref argnErrCode2, ref argnErrCode3, ref strSource, ref argnLineNum, ref strError, ref argbRaise);
            }
        }

        private void batch_Clear()
        {
            _activeBatch = null;

            // *** Clear document as well
            DocumentClear();

            // *** Delete the XML files
            //file_Delete(XmlSetupExportFile);
            //file_Delete(XmlSetupImportFile);
            //file_Delete(XmlRuntimeExportFile);
            //file_Delete(XmlRuntimeImportFile);
        }

        private void batch_Lock()
        {
            // *** Lock the batch           
            _activeBatch.BatchOpen(_processId);

            // *** Export the XML
            batch_Export();
        }

        private void batch_Export()
        {
            // *** Export the XML
            switch (_processMode)
            {
                case ProcessMode.ProcessByBatch:
                    {
                        _activeBatch.XMLExport(XmlRuntimeExportFile, XmlSetupExportFile);
                        break;
                    }
                case ProcessMode.ProcessByDoc:
                    {
                        _activeBatch.XMLExportBatchOnly(XmlRuntimeExportFile, XmlSetupExportFile);
                        break;
                    }
            }

            _isDocOpen = false;
        }

        public void DocumentOpen(short iFirst, short iLast = -1)
        {
            try
            {
                // *** Be sure a batch is already open
                //Debug.Assert(m_oActiveBatch is not null, "");

                // *** Export documents
                if (iLast == -1)
                {
                    _activeBatch.XMLExportDocuments(XmlDocumentExportFile, iFirst);
                    
                }
                else
                {
                    _activeBatch.XMLExportDocuments(XmlDocumentExportFile, iFirst, iLast);
                }

                _isDocOpen = true;
                _FirstDoc = iFirst;
                _LastDoc = iLast;
            }
            catch
            {
                int argnErrCode1 = Information.Err().Number;
                int argnErrCode2 = 0;
                int argnErrCode3 = 0;
                string argstrSource =  "BatchManager.DocumentOpen";
                int argnLineNum = 0;
                string argstrDesc = Information.Err().Description;
                bool argbRaise = true;
                _login.LogError(ref argnErrCode1, ref argnErrCode2, ref argnErrCode3, ref argstrSource, ref argnLineNum, ref argstrDesc, ref argbRaise);
                Information.Err().Number = argnErrCode1;
                Information.Err().Description = argstrDesc;
            }
        }

        public void DocumentClose()
        {
            try
            {

                // *** Be sure a batch is open
               //Debug.Assert(m_oActiveBatch is not null, "");

                // *** Import documents
                if (_LastDoc == -1)
                {
                    _activeBatch.XMLImportDocuments(XmlDocumentImportFile, _FirstDoc);
                }
                else
                {
                    _activeBatch.XMLImportDocuments(XmlDocumentImportFile, _FirstDoc, _LastDoc);
                }
                DocumentClear();
            }
            catch
            {
                int argnErrCode1 = Information.Err().Number;
                int argnErrCode2 = 0;
                int argnErrCode3 = 0;
                string argstrSource =  "BatchManager.DocumentClose";
                int argnLineNum = Information.Erl();
                string argstrDesc = Information.Err().Description;
                bool argbRaise = true;
                _login.LogError(ref argnErrCode1, ref argnErrCode2, ref argnErrCode3, ref argstrSource, ref argnLineNum, ref argstrDesc, ref argbRaise);
                Information.Err().Number = argnErrCode1;
                Information.Err().Description = argstrDesc;
            }
        }

        private void DocumentClear()
        {
            _isDocOpen = false;
            _FirstDoc = 0;
            _LastDoc = 0;

            // *** Delete the XML files
            //file_Delete(XmlDocumentExportFile);
            //file_Delete(XmlDocumentImportFile);
        }

    }
}
