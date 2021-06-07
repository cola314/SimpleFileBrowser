using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleFileBrowser.Models
{
    public class FileInformation
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public long Size { get; set; }

        public string LastModifiedDate { get; set; }
    }
}
