using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.FoxCommon.update
{


    public class PublishVersion
    {
        public string RawVersion { get; set; }
        public Version ParsedVersion { get; set; }
        public bool IsPreRelease { get; set; }
        public string ReleaseMessage { get; set; }
        public string PublishPage { get; set; }
    }
}
