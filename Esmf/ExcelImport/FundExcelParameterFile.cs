// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace Esmf
{
    public class FundExcelParameterFile
    {
        public class Parameter
        {
            public string Name;
            public string Type;
        }

        public class NonDimensionalParameter : Parameter
        {
            public string Value;
        }

        public class OneDimensionalParameter : Parameter
        {
            private List<string> _data;

            public OneDimensionalParameter(List<string> data)
            {
                _data = data;
            }

            public int Length
            {
                get { return _data.Count; }
            }

            public string this[int index]
            {
                get { return _data[index]; }
            }
        }

        public class TwoDimensionalParameter : Parameter
        {
            private string[,] _data;

            public TwoDimensionalParameter(string[,] data)
            {
                _data = data;
            }

            public int Count0
            {
                get { return _data.GetLength(0); }
            }

            public int Count1
            {
                get { return _data.GetLength(1); }
            }

            public string this[int index1, int index2]
            {
                get { return _data[index1, index2]; }
            }
        }

        private string _filename;

        private XmlDocument _workbook;

        private XmlDocument _stringtable;

        private Dictionary<string, OpenXmlPart> _sheets = new Dictionary<string, OpenXmlPart>();

        private List<Parameter> _parameters = new List<Parameter>();

        public FundExcelParameterFile(string filename)
        {
            _filename = filename;
        }

        private IEnumerable<string> EnumerateExcelColRange(string start, string end)
        {
            StringBuilder current = new StringBuilder(start.ToUpper().Trim());

            while (current.ToString() != end)
            {
                yield return current.ToString();

                int currentStelle = 0;

                while (1 == 1)
                {
                    if (current[current.Length - 1 - currentStelle] == 'Z')
                    {
                        if (current.Length - 1 - currentStelle == 0)
                        {
                            current[current.Length - 1 - currentStelle] = 'A';
                            current.Insert(0, 'A');
                            break;
                        }
                        else
                        {
                            current[current.Length - 1 - currentStelle] = 'A';
                            currentStelle++;
                        }
                    }
                    else
                    {
                        current[current.Length - 1 - currentStelle] = Convert.ToChar(current[current.Length - 1 - currentStelle] + 1);
                        break;
                    }
                }
            }

            yield return current.ToString();
        }

        public void Load()
        {
            using (SpreadsheetDocument xlPackage = SpreadsheetDocument.Open(_filename, false))
            {
                WorkbookPart workbook = xlPackage.WorkbookPart;

                _workbook = LoadWorkbookDOM(workbook);

                Dictionary<string, string> allParameters = LoadDefinedFundParameterNames(_workbook);

                Dictionary<string, HashSet<string>> cellsByTable = new Dictionary<string, HashSet<string>>();

                foreach (string k in allParameters.Keys)
                {
                    string range = allParameters[k];

                    string[] rangeParts = range.Split('!');

                    string table = rangeParts[0];
                    string cellinfo = rangeParts[1];

                    if (!cellsByTable.ContainsKey(table))
                    {
                        cellsByTable.Add(table, new HashSet<string>());
                    }

                    HashSet<string> cellsToLookFor = cellsByTable[table];

                    if (cellinfo.Contains(":"))
                    {
                        string[] cells = cellinfo.Split(':');

                        string startcell = cells[0].Replace("$", "");
                        string endcell = cells[1].Replace("$", "");

                        int startcellrowindex = startcell.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                        string startcellcol = startcell.Substring(0, startcellrowindex);
                        string startcellrow = startcell.Substring(startcellrowindex);

                        int endcellrowindex = endcell.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                        string endcellcol = endcell.Substring(0, endcellrowindex);
                        string endcellrow = endcell.Substring(endcellrowindex);

                        if (startcellrow != endcellrow && startcellcol != endcellcol)
                        {
                            List<string> columns = new List<string>(EnumerateExcelColRange(startcellcol, endcellcol));

                            foreach (string column in columns)
                            {
                                for (int i = Convert.ToInt32(startcellrow); i <= Convert.ToInt32(endcellrow); i++)
                                {
                                    cellsToLookFor.Add(column + i.ToString());
                                }
                            }
                        }
                        else if (startcellrow == endcellrow)
                        {
                            foreach (string s in EnumerateExcelColRange(startcellcol, endcellcol))
                            {
                                cellsToLookFor.Add(s + startcellrow);
                            }

                        }
                        else if (startcellcol == endcellcol)
                        {
                            for (int i = Convert.ToInt32(startcellrow); i <= Convert.ToInt32(endcellrow); i++)
                            {
                                cellsToLookFor.Add(startcellcol + i.ToString());

                            }
                        }
                    }
                    else
                    {
                        cellsToLookFor.Add(cellinfo.Replace("$", ""));
                    }

                }

                const string worksheetSchema = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
                const string sharedStringSchema = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

                LoadSheets(workbook, allParameters);

                Dictionary<string, Dictionary<string, string>> allNeededValues = new Dictionary<string, Dictionary<string, string>>();

                Dictionary<int, List<KeyValuePair<string, string>>> thingsThatNeedToBeLoadedFromTheStringTable = new Dictionary<int, List<KeyValuePair<string, string>>>();

                foreach (string sheetname in _sheets.Keys)
                {
                    allNeededValues.Add(sheetname, new Dictionary<string, string>());
                    Dictionary<string, string> currentAllNeededValues = allNeededValues[sheetname];

                    HashSet<string> currentCellsToLookFor = cellsByTable[sheetname];

                    //  Return the value of the specified cell.

                    //  Create a namespace manager, so you can search.
                    //  Add a prefix (d) for the default namespace.
                    NameTable nt = new NameTable();
                    XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
                    nsManager.AddNamespace("d", worksheetSchema);

                    using (XmlTextReader r = new XmlTextReader(_sheets[sheetname].GetStream(FileMode.Open, FileAccess.Read), nt))
                    {
                        while (r.Read())
                        {
                            if (r.NodeType == XmlNodeType.Element && r.Name == "c")
                            {
                                string cellReference = r.GetAttribute("r");
                                if (currentCellsToLookFor.Contains(cellReference))
                                {
                                    string cellType = r.GetAttribute("t");

                                    if (r.ReadToDescendant("v"))
                                    {
                                        string cellValue = r.ReadElementContentAsString();

                                        if (cellType != "s")
                                        {
                                            currentAllNeededValues.Add(cellReference, cellValue);
                                        }
                                        else
                                        {
                                            int stringTableIndex = Convert.ToInt32(cellValue);
                                            if (!thingsThatNeedToBeLoadedFromTheStringTable.ContainsKey(stringTableIndex))
                                            {
                                                thingsThatNeedToBeLoadedFromTheStringTable.Add(stringTableIndex, new List<KeyValuePair<string, string>>());
                                            }
                                            thingsThatNeedToBeLoadedFromTheStringTable[stringTableIndex].Add(new KeyValuePair<string, string>(sheetname, cellReference));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //  Create a namespace manager, so you can search.
                //  Add a prefix (d) for the default namespace.
                NameTable nts = new NameTable();
                XmlNamespaceManager nsManager2 = new XmlNamespaceManager(nts);
                nsManager2.AddNamespace("s", sharedStringSchema);

                using (XmlTextReader r = new XmlTextReader(workbook.SharedStringTablePart.GetStream(FileMode.Open, FileAccess.Read), nts))
                {
                    int currentStringTableIndex = -1;
                    while (r.Read())
                    {
                        if (r.NodeType == XmlNodeType.Element && r.Name == "si")
                        {
                            currentStringTableIndex++;

                            if (thingsThatNeedToBeLoadedFromTheStringTable.ContainsKey(currentStringTableIndex))
                            {
                                if (r.ReadToDescendant("t"))
                                {
                                    string cellValue = r.ReadElementContentAsString();
                                    foreach (KeyValuePair<string, string> tt in thingsThatNeedToBeLoadedFromTheStringTable[currentStringTableIndex])
                                    {
                                        allNeededValues[tt.Key].Add(tt.Value, cellValue);
                                    }
                                }
                            }
                        }
                    }
                }


                foreach (string k in allParameters.Keys)
                {
                    string range = allParameters[k];

                    string[] rangeParts = range.Split('!');

                    string table = rangeParts[0];
                    string cellinfo = rangeParts[1];

                    if (cellinfo.Contains(":"))
                    {
                        string[] cells = cellinfo.Split(':');

                        string startcell = cells[0].Replace("$", "");
                        string endcell = cells[1].Replace("$", "");

                        int startcellrowindex = startcell.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                        string startcellcol = startcell.Substring(0, startcellrowindex);
                        string startcellrow = startcell.Substring(startcellrowindex);

                        int endcellrowindex = endcell.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                        string endcellcol = endcell.Substring(0, endcellrowindex);
                        string endcellrow = endcell.Substring(endcellrowindex);

                        if (startcellrow != endcellrow && startcellcol != endcellcol)
                        {
                            List<string> columns = new List<string>(EnumerateExcelColRange(startcellcol, endcellcol));

                            string[,] data = new string[Convert.ToInt32(endcellrow) - Convert.ToInt32(startcellrow) + 1, columns.Count];

                            foreach (string column in columns)
                            {
                                for (int i = Convert.ToInt32(startcellrow); i <= Convert.ToInt32(endcellrow); i++)
                                {
                                    data[i - Convert.ToInt32(startcellrow), columns.IndexOf(column)] = allNeededValues[table][column + i.ToString()];
                                }
                            }

                            TwoDimensionalParameter p = new TwoDimensionalParameter(data);

                            string nameAndType = k.Substring(5);

                            if (nameAndType.Contains("."))
                            {
                                int indexOfNameTypeSeperator = nameAndType.IndexOf('.');
                                p.Name = nameAndType.Substring(0, indexOfNameTypeSeperator);
                                p.Type = nameAndType.Substring(indexOfNameTypeSeperator + 1);
                            }
                            else
                            {
                                p.Name = nameAndType;
                                p.Type = null;
                            }

                            _parameters.Add(p);
                        }
                        else if (startcellrow == endcellrow)
                        {
                            List<string> data = new List<string>();

                            foreach (string s in EnumerateExcelColRange(startcellcol, endcellcol))
                            {
                                string v = allNeededValues[table][s + startcellrow];
                                data.Add(v);
                            }

                            OneDimensionalParameter p = new OneDimensionalParameter(data);

                            string nameAndType = k.Substring(5);

                            if (nameAndType.Contains("."))
                            {
                                int indexOfNameTypeSeperator = nameAndType.IndexOf('.');
                                p.Name = nameAndType.Substring(0, indexOfNameTypeSeperator);
                                p.Type = nameAndType.Substring(indexOfNameTypeSeperator + 1);
                            }
                            else
                            {
                                p.Name = nameAndType;
                                p.Type = null;
                            }

                            _parameters.Add(p);

                        }
                        else if (startcellcol == endcellcol)
                        {
                            List<string> data = new List<string>();

                            for (int i = Convert.ToInt32(startcellrow); i <= Convert.ToInt32(endcellrow); i++)
                            {
                                string v = allNeededValues[table][startcellcol + i.ToString()];
                                data.Add(v);
                            }

                            OneDimensionalParameter p = new OneDimensionalParameter(data);

                            string nameAndType = k.Substring(5);

                            if (nameAndType.Contains("."))
                            {
                                int indexOfNameTypeSeperator = nameAndType.IndexOf('.');
                                p.Name = nameAndType.Substring(0, indexOfNameTypeSeperator);
                                p.Type = nameAndType.Substring(indexOfNameTypeSeperator + 1);
                            }
                            else
                            {
                                p.Name = nameAndType;
                                p.Type = null;
                            }

                            _parameters.Add(p);
                        }
                    }
                    else
                    {
                        NonDimensionalParameter p = new NonDimensionalParameter();

                        string nameAndType = k.Substring(5);

                        if (nameAndType.Contains("."))
                        {
                            int indexOfNameTypeSeperator = nameAndType.IndexOf('.');
                            p.Name = nameAndType.Substring(0, indexOfNameTypeSeperator);
                            p.Type = nameAndType.Substring(indexOfNameTypeSeperator + 1);
                        }
                        else
                        {
                            p.Name = nameAndType;
                            p.Type = null;
                        }

                        p.Value = allNeededValues[table][cellinfo.Replace("$", "")];

                        _parameters.Add(p);
                    }
                }
            }
        }

        private void LoadStringtable(WorkbookPart workbook)
        {
            SharedStringTablePart part = workbook.SharedStringTablePart;
            using (Stream s = part.GetStream(FileMode.Open, FileAccess.Read))
            {

                _stringtable = new XmlDocument();
                _stringtable.Load(s);
            }
        }

        private string GetCellValue(string sheetName, string addressName)
        {
            //  Return the value of the specified cell.
            const string worksheetSchema = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            const string sharedStringSchema = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            string cellValue = null;

            //  Create a namespace manager, so you can search.
            //  Add a prefix (d) for the default namespace.
            NameTable nt = new NameTable();
            XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
            nsManager.AddNamespace("d", worksheetSchema);
            nsManager.AddNamespace("s", sharedStringSchema);

            XmlDocument sheetDoc = new XmlDocument();
            sheetDoc.Load(_sheets[sheetName].GetStream());

            XmlNode cellNode = sheetDoc.SelectSingleNode(string.Format("//d:sheetData/d:row/d:c[@r='{0}']", addressName), nsManager);
            if (cellNode != null)
            {

                //  Retrieve the value. The value may be stored within 
                //  this element. If the "t" attribute contains "s", then
                //  the cell contains a shared string, and you must look 
                //  up the value individually.
                XmlAttribute typeAttr = cellNode.Attributes["t"];
                string cellType = string.Empty;
                if (typeAttr != null)
                {
                    cellType = typeAttr.Value;
                }

                XmlNode valueNode = cellNode.SelectSingleNode("d:v", nsManager);
                if (valueNode != null)
                {
                    cellValue = valueNode.InnerText;
                }

                //  Check the cell type. At this point, this code only checks
                //  for booleans and strings individually.
                if (cellType == "b")
                {
                    if (cellValue == "1")
                    {
                        cellValue = "TRUE";
                    }
                    else
                    {
                        cellValue = "FALSE";
                    }
                }
                else if (cellType == "s")
                {


                    int requestedString = Convert.ToInt32(cellValue);
                    string strSearch = string.Format("//s:sst/s:si[{0}]", requestedString + 1);
                    XmlNode stringNode = _stringtable.SelectSingleNode(strSearch, nsManager);
                    if (stringNode != null)
                    {
                        cellValue = stringNode.InnerText;
                    }
                }
            }
            return cellValue;
        }


        private void LoadSheets(WorkbookPart workbook, Dictionary<string, string> allParameters)
        {
            foreach (string k in allParameters.Keys)
            {
                string range = allParameters[k];

                string[] rangeParts = range.Split('!');

                string table = rangeParts[0];

                if (!_sheets.ContainsKey(table))
                {

                    XmlNamespaceManager nsManager = new XmlNamespaceManager(_workbook.NameTable);
                    nsManager.AddNamespace("d", _workbook.DocumentElement.NamespaceURI);

                    XmlNodeList nodes = _workbook.SelectNodes("//d:sheets/d:sheet", nsManager);

                    foreach (XmlNode node in nodes)
                    {
                        if (node.Attributes["name"].Value == table)
                        {
                            OpenXmlPart part = workbook.GetPartById(node.Attributes["r:id"].Value);

                            _sheets.Add(table, part);
                        }
                    }

                }
            }
        }

        private static XmlDocument LoadWorkbookDOM(WorkbookPart workbook)
        {
            using (Stream workbookstr = workbook.GetStream(FileMode.Open, FileAccess.Read))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(workbookstr);

                return doc;
            }

        }

        private Dictionary<string, string> LoadDefinedFundParameterNames(XmlDocument workbook)
        {
            Dictionary<string, string> allParameters = new Dictionary<string, string>();

            XmlNamespaceManager nsManager = new XmlNamespaceManager(_workbook.NameTable);
            nsManager.AddNamespace("d", _workbook.DocumentElement.NamespaceURI);

            XmlNodeList nodes = _workbook.SelectNodes("//d:definedNames/d:definedName", nsManager);

            foreach (XmlNode node in nodes)
            {
                string name = node.Attributes["name"].Value;

                if (name.StartsWith("Fund.", StringComparison.OrdinalIgnoreCase))
                {
                    string range = node.InnerText.Replace("'", "");

                    allParameters.Add(name, range);
                }
            }

            return allParameters;
        }

        public List<Parameter> Parameters
        {
            get { return _parameters; }
        }
    }
}
