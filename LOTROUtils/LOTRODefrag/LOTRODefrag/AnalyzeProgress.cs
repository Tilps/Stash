using System;
using System.Collections.Generic;
using System.Text;

namespace LOTRODefrag
{
    class AnalyzeProgress
    {
        private bool shouldDefrag;

        public bool ShouldDefrag
        {
            get { return shouldDefrag; }
        }

        private string name;

        public string Name
        {
            get { return name; }
        }

        private int numFrags;

        public int NumFrags
        {
            get { return numFrags; }
        }

        private int numFiles;

        public int NumFiles
        {
            get { return numFiles; }
        }

        private bool recommendDefrag;

        public bool RecommendDefrag
        {
            get { return recommendDefrag; }
        }

        private long externalFrags;

        public long ExternalFrags
        {
            get { return externalFrags; }
        }

        private string status;

        public string Status
        {
            get { return status; }
        }

        private bool shouldExtDefrag;

        public bool ShouldExtDefrag
        {
            get { return shouldExtDefrag; }
        }

        public AnalyzeProgress(bool shouldDefrag, string name, int numFrags, int numFiles, bool recommendDefrag, long externalFrags, bool shouldExtDefrag, string status)
        {
            this.shouldDefrag = shouldDefrag;
            this.name = name;
            this.numFrags = numFrags;
            this.numFiles = numFiles;
            this.recommendDefrag = recommendDefrag;
            this.externalFrags = externalFrags;
            this.status = status;
            this.shouldExtDefrag = shouldExtDefrag;
        }
    }
}
