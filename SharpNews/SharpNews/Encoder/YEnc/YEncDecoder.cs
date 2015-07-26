using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SharpNews.Encoder.YEnc
{
    /// <summary>
    /// Provides decoding facilities for yEnc posts.
    /// </summary>
    public class YEncDecoder
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="output">
        /// Stream to receive decoded post contents.</param>
        public YEncDecoder(Stream output)
        {
            this.output = output;
        }

        private Stream output;

        /// <summary>
        /// Processes one articles contents.
        /// </summary>
        /// <param name="lines">
        /// Raw body contents of an article.
        /// </param>
        public void AddPost(List<byte[]> lines)
        {
            bool started = false;
            foreach (byte[] line in lines)
            {
                if (!started)
                {
                    if (line.Length >= 8 && line[0] == (byte)'='
                        && line[1] == (byte)'y'
                        && line[2] == (byte)'b'
                        && line[3] == (byte)'e'
                        && line[4] == (byte)'g'
                        && line[5] == (byte)'i'
                        && line[6] == (byte)'n'
                        && line[7] == (byte)' ')
                    {
                        string lineStr = Encoding.ASCII.GetString(line);

                        started = true;
                    }
                }
                if (started)
                {
                    if (line.Length >= 6 && line[0] == (byte)'='
                       && line[1] == (byte)'y'
                       && line[2] == (byte)'e'
                       && line[3] == (byte)'n'
                       && line[4] == (byte)'d'
                       && line[5] == (byte)' ')
                    {
                        started = false;
                    }
               }
               if (line.Length >= 2 && line[0] == (byte)'='
                   && line[1] == (byte)'y')
                   continue;
               if (started)
               {
                   bool first = true;
                   bool escape = false;
                   foreach (byte b in line)
                   {
                       if (first && b == (byte)'.')
                       {
                           first = false;
                           continue;
                       }
                       else if (first)
                           first = false;
                       if (b != (byte)'=')
                       {
                           if (b == '\r' || b == '\n')
                               continue;
                           if (!escape)
                           {
                               unchecked
                               {
                                   output.WriteByte((byte)(b - 42));
                               }
                           }
                           else
                           {
                               unchecked
                               {
                                   output.WriteByte((byte)(b - 106));
                               }
                               escape = false;
                           }
                       }
                       else
                           escape = true;
                   }
               }
            }
        }
    }
}
