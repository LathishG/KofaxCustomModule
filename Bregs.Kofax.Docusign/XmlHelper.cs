using Kofax.Capture.SDK.CustomModule;
using Kofax.MSXML.Interop;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bregs.Kofax.Docusign
{
    internal class XmlHelper
    {
        // *** Enumerations
        public enum acxXmlType
        {
            acxRuntimeType,
            acxSetupType,
            acxDocumentType
        }

        // *** Class members
        private DOMDocument m_oXSetupDocument;
        private DOMDocument m_oXRuntimeDocument;
        private DOMDocument m_oXDocDocument;
        private string m_strImagesPath; // *** Images directory for the batch.
                                        // *** Cached for performance.

        // **************************************************************************
        // *** Property:  XmlSetupFile
        // *** Purpose:   Set the path to the XML file that represents the batch
        // **************************************************************************
        public string XmlSetupFile
        {
            set
            {
                m_oXSetupDocument = OpenXmlFile(value);
            }
        }

        // **************************************************************************
        // *** Property:  XmlSetup
        // *** Purpose:   Returns the DOM document.
        // *** Notes:     The XmlSetupFile must be set first.
        // **************************************************************************
        public DOMDocument XmlSetup
        {
            get
            {
                DOMDocument XmlSetupRet = default;
                XmlSetupRet = m_oXSetupDocument;
                return XmlSetupRet;
            }
        }

        // **************************************************************************
        // *** Property:  XmlRuntimeFile
        // *** Purpose:   Set the path to the XML file that represents the batch
        // **************************************************************************
        public string XmlRuntimeFile
        {
            set
            {
                m_oXRuntimeDocument = OpenXmlFile(value);
            }
        }

        // **************************************************************************
        // *** Property:  XmlRuntime
        // *** Purpose:   Returns the DOM document.
        // *** Notes:     The XmlRuntimeFile must be set first.
        // **************************************************************************
        public DOMDocument XmlRuntime
        {
            get
            {
                DOMDocument XmlRuntimeRet = default;
                XmlRuntimeRet = m_oXRuntimeDocument;
                return XmlRuntimeRet;
            }
        }
        // **************************************************************************
        // *** Property:  XmlRuntimeDocumentsFile
        // *** Purpose:   Set the path to the XML file that represents the documents.
        // **************************************************************************
        public string XmlRuntimeDocumentsFile
        {
            set
            {
                m_oXDocDocument = OpenXmlFile(value);
            }
        }

        // **************************************************************************
        // *** Property:  XmlDocument
        // *** Purpose:   Returns the DOM document.
        // *** Notes:     The XmlDocumentFile must be set first.
        // **************************************************************************
        public DOMDocument XmlRuntimeDocuments
        {
            get
            {
                DOMDocument XmlRuntimeDocumentsRet = default;
                XmlRuntimeDocumentsRet = m_oXDocDocument;
                return XmlRuntimeDocumentsRet;
            }
        }

        // **************************************************************************
        // *** Function:  WriteXmlRuntimeFile
        // *** Purpose:   Write the batch XML file, based on changes to the DOM
        // **************************************************************************
        public void WriteXmlRuntimeFile(string strFile)
        {

            SaveXmlFile(strFile, ref m_oXRuntimeDocument);

        }

        // **************************************************************************
        // *** Function:  GetXml
        // *** Purpose:   Returns a DOM document absed on acxXmlType.
        // *** Notes:     The XmlDocumentFile must be set first.
        // **************************************************************************
        public DOMDocument GetXml(acxXmlType XmlType)
        {
            DOMDocument GetXmlRet = default;
            switch (XmlType)
            {
                case acxXmlType.acxRuntimeType:
                    {
                        GetXmlRet = XmlRuntime;
                        break;
                    }
                case acxXmlType.acxSetupType:
                    {
                        GetXmlRet = XmlSetup;
                        break;
                    }
                case acxXmlType.acxDocumentType:
                    {
                        GetXmlRet = XmlRuntimeDocuments;
                        break;
                    }

                default:
                    {
                        throw new ApplicationException("Unknown XmlType");
                    }
            }

            return GetXmlRet;
        }

        // **************************************************************************
        // *** Function:  OpenXmlFile
        // *** Purpose:   Open an Xml file into a DOM object
        // *** Input:     strFileName - Path of XML file
        // *** Output:    Returns the DOMDocument
        // *************************************************************************
        private DOMDocument OpenXmlFile(string strFileName)
        {
            DOMDocument OpenXmlFileRet = default;

            var oXDocument = new DOMDocument();

            // *** VERY IMPORTANT to make reading synchronous to insure
            // *** that the file is not changing
            oXDocument.async = false;

            // *** Load the XML file
            oXDocument.load(strFileName);
            // *** Throw an error if the file has not been read

            IXMLDOMParseError oXParseError;
            string strParseError;
            if (!oXDocument.hasChildNodes())
            {
                oXParseError = oXDocument.parseError;
                strParseError = " (" + oXParseError.reason + " " + oXParseError.line + ")";
                throw new ApplicationException("Parse Error" + strParseError);
            }

            OpenXmlFileRet = oXDocument;
            return OpenXmlFileRet;
        }

        // **************************************************************************
        // *** Function:  SaveXmlFile
        // *** Purpose:   Write the Xml file for a DOM object
        // *** Input:     strFileName - Path of XML file
        // ***            oDocument - the DOM representing the XML to write
        // **************************************************************************
        private void SaveXmlFile(string strFileName, ref DOMDocument oXDocument)
        {
            oXDocument.save(strFileName);
        }

        // **************************************************************************
        // *** Function:  GetDocuments
        // *** Purpose:   Get the collection of documents from the Xml Batch object
        // *** Input:     None
        // *** Output:    Returns a node list of all "Document" objects
        // **************************************************************************
        public IXMLDOMNodeList GetDocuments()
        {
            IXMLDOMNodeList GetDocumentsRet = default;
            if (XmlRuntimeDocuments is null)
            {
                GetDocumentsRet = GetBatch().selectNodes("Documents/Document");
            }
            else
            {
                GetDocumentsRet = XmlRuntimeDocuments.selectNodes("//Documents/Document");
            }

            return GetDocumentsRet;
        }

        // **************************************************************************
        // *** Function:  GetBatch
        // *** Purpose:   Get the collection of documents from the Xml Batch object
        // *** Input:     None
        // *** Output:    Returns the batch node
        // **************************************************************************
        public IXMLDOMNode GetBatch()
        {
            IXMLDOMNode GetBatchRet = default;
            GetBatchRet = XmlRuntime.selectSingleNode("//Batch");
            return GetBatchRet;
        }

        // **************************************************************************
        // *** Function:  GetLoosePages
        // *** Purpose:   Get the collection of loose pages from the Xml Batch object
        // *** Input:     None
        // *** Output:    Returns a node list of all loose "Page" objects
        // **************************************************************************
        public IXMLDOMNodeList GetLoosePages()
        {
            IXMLDOMNodeList GetLoosePagesRet = default;
            GetLoosePagesRet = GetBatch().selectNodes("Pages/Page");
            return GetLoosePagesRet;
        }

        // **************************************************************************
        // *** Function:  GetLoosePagesNode
        // *** Purpose:   Get the loose pages node from the Xml Batch object
        // *** Input:     None
        // *** Output:    Returns the loose pages
        // **************************************************************************
        public IXMLDOMNode GetLoosePagesNode()
        {
            IXMLDOMNode GetLoosePagesNodeRet = default;
            IXMLDOMNode oNode;
            oNode = XmlRuntime.selectSingleNode("//Batch/Pages");
            IXMLDOMNode oBatchNode;

            if (oNode is null)
            {
                oBatchNode = GetBatch();
                oNode = CreateElement("Pages");
                oBatchNode.appendChild(oNode);
            }
            GetLoosePagesNodeRet = oNode;
            return GetLoosePagesNodeRet;
        }

        // **************************************************************************
        // *** Function:  GetDocumentsNode
        // *** Purpose:   Get the document node from the Xml Batch object
        // *** Input:     None
        // *** Output:    Returns the documents
        // **************************************************************************
        public IXMLDOMNode GetDocumentsNode()
        {
            IXMLDOMNode GetDocumentsNodeRet = default;
            IXMLDOMNode oNode;
            oNode = XmlRuntime.selectSingleNode("//Batch/Documents");
            IXMLDOMNode oBatchNode;
            IXMLDOMNode oPages;

            if (oNode is null)
            {
                oBatchNode = GetBatch();
                oNode = CreateElement("Documents");
                oPages = GetLoosePagesNode();
                oBatchNode.insertBefore(oNode, oPages);
            }
            GetDocumentsNodeRet = oNode;
            return GetDocumentsNodeRet;
        }

        // **************************************************************************
        // *** Function:  SetAttribute
        // *** Purpose:   Sets an attribute in an element node
        // *** Input:     oXDocumentNode is the document node
        // ***            strName - Name of the attribute to set
        // ***            strValue - Value to set
        // ***            DocType - indicates which Xml doc to modify
        // *** Output:    None
        // **************************************************************************
        public void SetAttribute(ref IXMLDOMNode oXElement, string strName, string strValue, [Optional, DefaultParameterValue(acxXmlType.acxRuntimeType)] ref acxXmlType XmlType)

        {

            // *** Get the attribute map		
            IXMLDOMNamedNodeMap oXAttrList;
            oXAttrList = oXElement.attributes;

            // *** Create the new attribute node and set the value		
            IXMLDOMNode oXAttribute;
            oXAttribute = GetXml(XmlType).createAttribute(strName);
            oXAttribute.nodeValue = strValue;
            oXAttrList.setNamedItem(oXAttribute);
        }

        // **************************************************************************
        // *** Function:  CreateElement
        // *** Purpose:   Creates the specified XML element.
        // *** Input:     strElement - Element to create
        // *** Output:    The element that was created.
        // **************************************************************************
        public IXMLDOMElement CreateElement(string strElement)
        {
            IXMLDOMElement CreateElementRet = default;
            CreateElementRet = m_oXRuntimeDocument.createElement(strElement);
            return CreateElementRet;
        }

        // **************************************************************************
        // *** Function:  GetImageFile
        // *** Purpose:   Returns an image file path based on the image path and 
        // ***            image id
        // *** Input:     oXPage - Page node
        // *** Output:    Outputs the actual file path
        // **************************************************************************
        public string GetImageFile(ref IXMLDOMNode oXPage, ref IBatch oBatch)
        {
            string GetImageFileRet = default;

            // *** Check to see if this file has an extension, then use it
            // *** to get the image ID from the page
            IXMLDOMNode oExtNode;
            oExtNode = oXPage.attributes.getNamedItem("Extension");

            if (oExtNode != null)
            {
                GetImageFileRet = oBatch.get_ImageFileWithExt(Convert.ToInt32(oXPage.attributes.getNamedItem("ImageID").nodeValue), oExtNode.text);

            }
            else
            {
                GetImageFileRet = oBatch.get_ImageFile(Convert.ToInt32(oXPage.attributes.getNamedItem("ImageID").nodeValue));
            }

            return GetImageFileRet;
        }
    }
}
