// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.IO;

namespace NodaTime.TzValidate.TrimNames
{
    /// <summary>
    /// Tiny app to trim the names off tzvalidate output. This could probably be done fairly
    /// easily in a shell script with sed etc, but it's simple enough to write for Windows...
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            // We might want to make this use the command line for files at some point,
            // but not yet.
            RewriteLines(Console.In, Console.Out);
        }

        static void RewriteLines(TextReader input, TextWriter output)
        {
            bool inHeaders = true;
            string line;
            while ((line = input.ReadLine()) != null)
            {
                string outputLine;
                if (inHeaders && (line == "" || line.Contains(":")))
                {
                    outputLine = line;
                }
                else
                {
                    inHeaders = false;
                    outputLine = TrimLine(line);
                }
                // Keep using \n as the line ending...
                output.Write("{0}\n", outputLine);
            }
        }

        // This shows what we want to keep
        const string SampleContent = "1918-01-01 00:00:52Z +00:00:00 standard";

        static string TrimLine(string line)
        {
            if (line.Length < SampleContent.Length + 1 || line[SampleContent.Length] != ' ')
            {
                return line;
            }
            return line.Substring(0, SampleContent.Length);
        }
    }
}
