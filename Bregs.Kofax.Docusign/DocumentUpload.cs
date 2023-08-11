using Kofax.MSXML.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace Bregs.Kofax.Docusign
{
    public partial class DocumentUpload : Form
    {
        public DocumentUpload()
        {
            InitializeComponent();
        }

        private void btnOpenBatch_Click(object sender, EventArgs e)
        {
            lblBatchId.Text = "0";
            lblDocumentCount.Text = "0";
            int batchId=0;
            BatchManager batchManager = new BatchManager("Kofax.docusign");
            batchManager.LoginToRuntimeSession();

            clsCmdLine   clsCmdLine = new clsCmdLine();

            if (clsCmdLine.IsOption("B") == true)
                batchId = Convert.ToInt32(clsCmdLine.GetOptionParameter("B"));

            lblBatchId.Text = batchId.ToString();

            batchManager.XmlRuntimeExportFile = "RtExport.xml";
            batchManager.XmlRuntimeImportFile = "RtImport.xml";
            batchManager.XmlDocumentExportFile = "DcExport.xml"; 
            batchManager.XmlDocumentImportFile = "DcImport.xml";

            if (batchId>0)
            {
                //Open Batch
                batchManager.BatchOpen(batchId, ProcessMode.ProcessByDoc);
                if (batchManager.ActiveBatch !=null)
                {
                    lblDocumentCount.Text = batchManager.ActiveBatch.DocumentCount > 0 ? Convert.ToString(batchManager.ActiveBatch.DocumentCount) : "0";
                    //Open Documnets
                    if (batchManager.ActiveBatch.DocumentCount > 0)
                    {
                        batchManager.DocumentOpen(1, (short)batchManager.ActiveBatch.DocumentCount);

                        XmlHelper xmlHelper = new XmlHelper();
                        xmlHelper.XmlRuntimeDocumentsFile = batchManager.XmlDocumentExportFile;
                        int cnt = 0;
                        string indexFieldName = "";
                        string indexFieldValue = "";
                        foreach (IXMLDOMNode docNode in xmlHelper.GetDocuments())
                        {                            
                            lbldocumentName.Text = docNode.attributes.getNamedItem("PDFGenerationFileName").nodeValue;
                            MessageBox.Show(docNode.attributes.getNamedItem("PDFGenerationFileName").nodeValue);
                            foreach (IXMLDOMNode indexFieldNode in docNode.selectNodes("IndexFields/IndexField"))
                            {
                                indexFieldName= indexFieldNode.attributes.getNamedItem("Name").nodeValue;
                                indexFieldValue = indexFieldNode.attributes.getNamedItem("Value").nodeValue;
                                MessageBox.Show(string.Format("{0}    ______   {1}", indexFieldName, indexFieldValue));
                                lblIndexField.Text = string.Format("{0}__{1}", indexFieldName, indexFieldValue);
                                lblIndexField.Update();
                            }
                        }

                        lblIndexField.Text = cnt.ToString();
                    }
                }
                
            }

        }

        private void lblBatchId_Click(object sender, EventArgs e)
        {

        }
    }
}
