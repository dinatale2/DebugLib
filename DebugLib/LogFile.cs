using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace DebugLib
{
    public class LogFile
    {
        private string m_strFilePath; // path to the log
        private bool m_bIncludeProcessID; // include process and thread ID?

        public LogFile(string FilePath, bool IncludeProcessID = false)
        {
            m_strFilePath = FilePath;
            m_bIncludeProcessID = IncludeProcessID;
        }

        public void Log(string strMessage, params object[] parms)
        {
            // only one thread at a time
            lock (this)
            {
                // basically write a line, get the time stamp first
                using (StreamWriter ToFile = new StreamWriter(m_strFilePath, true))
                {
                    ToFile.WriteLine(GetLinePreface() + " : " + string.Format(strMessage, parms));
                }
            }
        }

        private string GetLinePreface()
        {
            if (m_bIncludeProcessID)
            {
                // if we want a time stamp, we want it down to the seconds and in this case we want to print out the process and thread id
                return string.Format("{0:G}-({1}:{2})", DateTime.Now, Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId);
            }
            else
            {
                // in some cases (such as always in one thread) we only want a timestamp
                return DateTime.Now.ToString("G");
            }
        }

        public void Truncate(long nSizeKB)
        {
            // there is nothing to truncate if there is no file
            if (!File.Exists(m_strFilePath))
                return;

            // our new size in bytes (KB * 1024)bytes
            long nNewSize = 1024 * nSizeKB;
            long nCurrSize = -1;

            {
                // grab the current file's size
                FileInfo Info = new FileInfo(m_strFilePath);
                nCurrSize = Info.Length;
            }

            // we dont want to truncate the file if its already below
            if (nCurrSize > nNewSize)
            {
                // always practice safe programming when using threads...
                lock (this)
                {
                    // ok, byte array to hold the data we want to have in the new file
                    byte[] Data = new byte[nNewSize];
                    using (FileStream ToTrunc = new FileStream(m_strFilePath, FileMode.Open, FileAccess.Read))
                    {
                        // seek to the end - the new size, then read that amount into our buffer
                        ToTrunc.Seek(-nNewSize, SeekOrigin.End);
                        ToTrunc.Read(Data, 0, Data.Length);
                        ToTrunc.Close();
                    }

                    // new we need to overwrite our existing file, simply write the buffer to it and close
                    using (FileStream NewFile = new FileStream(m_strFilePath, FileMode.Truncate, FileAccess.Write))
                    {
                        NewFile.Write(Data, 0, Data.Length);
                        NewFile.Close();
                    }
                }
            }
        }
    }
}
