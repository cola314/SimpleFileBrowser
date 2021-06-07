using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleFileBrowser.Models
{
    public class FolderInformation
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string LastModifiedDate { get; set; }
    }
}
