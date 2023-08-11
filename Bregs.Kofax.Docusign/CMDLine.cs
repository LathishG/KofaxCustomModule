using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bregs.Kofax.Docusign
{
    internal class clsCmdLine
    {

        // *** Member variables
        public string OptSeparators; // *** Option separators
        public string OptParamSeparators; // *** Option parameter separators

        private string m_strCmdLine; // *** Command line string
        private Collection m_collOptions; // *** Collection of options.
        private Collection m_collParams; // *** Collection of option parameters.

        // *** Constants
        private const string DEFAULT_OPT_SEPARATORS = "-/";
        private const string DEFAULT_OPT_PARAM_SEPARATORS = "";

        // **************************************************************************
        // *** Property:  CmdLine
        // *** Purpose:   Parses the command line. Initializes the object variables.
        // **************************************************************************	
        public string CmdLine
        {
            get
            {
                string CmdLineRet = default;
                CmdLineRet = m_strCmdLine;
                return CmdLineRet;
            }
            set
            {
                // *** The string to parse. This is modified
                string strCmd;

                // *** Position within the string
                int lOptionStart;

                // *** Starting location of option parameter string
                var lParamStart = default(int);

                // *** Next option
                string strOption;

                // *** Parameter to the option
                string strParam;

                // *** Initialize class members
                m_collOptions = null;
                m_collOptions = new Collection();
                m_collParams = null;
                m_collParams = new Collection();
                m_strCmdLine = value;

                // *** Parse the command line.
                // *** Look for option separators.
                strCmd = value;
                lOptionStart = find_OneOf(strCmd, OptSeparators);

                // *** If no option separators are found, then use the whole command
                // *** line as an option so that it isn't lost.
                if (lOptionStart == 0 & Strings.Len(strCmd) > 0)
                {

                    // *** Add the whole command line as a parameter
                    m_collOptions.Add(strCmd, strCmd);
                    m_collParams.Add("", strCmd);
                }

                // *** Loop through the options and add them to the collection
                while (lOptionStart > 0)
                {

                    // *** Look for characters that separate options from 
                    // *** their parameters
                    strOption = Conversions.ToString(parse_String(ref strCmd, lOptionStart, OptParamSeparators, ref lParamStart));

                    // *** Look for the next option
                    strParam = Conversions.ToString(parse_String(ref strCmd, lParamStart, OptSeparators, ref lOptionStart));

                    // *** Add the parameter to the collection
                    Debug.Assert(Strings.Len(strOption) > 0, "");
                    if (Strings.Len(strOption) > 0)
                    {
                        m_collOptions.Add(strOption, strOption);
                        m_collParams.Add(strParam, strOption);
                    }
                }
            }
        }

        // **************************************************************************
        // *** Function:  IsOption
        // *** Purpose:   Determine if the specified option has been set
        // *** Inputs:    strOption - A possible option
        // *** Outputs:   True if the option was specified on the command line
        // **************************************************************************
        public bool IsOption(string strOption)
        {
            bool IsOptionRet = default;
            bool bRet;
            try
            {
                string strTempString = Conversions.ToString(m_collOptions[Strings.UCase(strOption)]);
                bRet = true;
            }
            catch (Exception ex)
            {
                bRet = false;
            }
            IsOptionRet = bRet;
            return IsOptionRet;
        }

        // **************************************************************************
        // *** Function:  GetOptionParameter
        // *** Purpose:   Returns the option parameter for a particular option
        // *** Inputs:    strOption - The option
        // *** Outputs:   Option parameter
        // *** Notes:     Returns blank if the parameter has not been specified.
        // ***            Raises if the option has not been specified.
        // ***            Options are not case sensitive.
        // **************************************************************************
        public string GetOptionParameter(string strOption)
        {
            string GetOptionParameterRet = default;
            GetOptionParameterRet = Conversions.ToString(m_collParams[Strings.UCase(strOption)]);
            return GetOptionParameterRet;
        }

        // **************************************************************************
        // *** Function:  New
        // *** Purpose:   Initialize the class members from the command line 
        // ***            arguments.
        // *** Inputs:    Command()
        // *** Outputs:   None
        // **************************************************************************
        public clsCmdLine() : base()
        {

            OptSeparators = DEFAULT_OPT_SEPARATORS;
            OptParamSeparators = DEFAULT_OPT_PARAM_SEPARATORS;
            CmdLine = Interaction.Command();
        }

        // **************************************************************************
        // *** Function:  find_OneOf
        // *** Purpose:   Searches for the first character in a string that matches 
        // ***            any character contained in another string.
        // *** Inputs:    strSrc - The string to search
        // ***            strCharSet - The characters to search for
        // *** Outputs:   Returns the position of the first character in strSrc that 
        // ***            is also in strCharSet; 0 if there is no match.
        // **************************************************************************
        private int find_OneOf(string strSrc, string strCharSet)
        {
            int find_OneOfRet = default;
            int lPos; // *** Used to walk through the source string

            var loopTo = Strings.Len(strSrc);
            for (lPos = 1; lPos <= loopTo; lPos++)
            {
                if (Strings.InStr(strCharSet, Strings.Mid(strSrc, lPos, 1)) > 0)
                {

                    // *** The character is one we were looking for
                    find_OneOfRet = lPos;
                    break;
                }
            }

            return find_OneOfRet;
        }

        // **************************************************************************
        // *** Function:  parse_String
        // *** Purpose:   Returns a parameter from the string and removes that string
        // ***            from the source string.
        // *** Inputs:    strSrc - Source string
        // ***            lStart - Position that current substring starts at
        // ***            strCharSet - Characters that identify the next substring
        // *** Outputs:   Returns the next string as separated by strCharSet
        // ***            lNextStart - Position that current substring starts at
        // **************************************************************************
        private object parse_String(ref string strSrc, int lStart, string strCharSet, ref int lNextStart)

        {
            object parse_StringRet = default;
            int lLen;

            // *** Trim the string as each part is processed
            if (lStart > 0)
            {
                strSrc = Strings.Mid(strSrc, 1 + lStart);
            }
            else
            {
                strSrc = "";
            }

            // *** Find starting location of next string
            if (Strings.Len(strCharSet) == 0)
            {
                // *** If no separators are specified, ASSUME a single
                // *** character string is desired.
                if (Strings.Len(strSrc) > 0)
                {
                    lNextStart = 1;
                }
                else
                {
                    lNextStart = 0;
                }
                lLen = lNextStart;
            }
            else
            {
                lNextStart = find_OneOf(strSrc, strCharSet);
                if (lNextStart > 0)
                {
                    lLen = lNextStart - 1;
                }
                else
                {
                    lLen = Strings.Len(strSrc);
                }
            }

            // *** Trim the result
            parse_StringRet = Strings.Trim(Strings.Mid(strSrc, 1, lLen));
            return parse_StringRet;
        }
    }
}
