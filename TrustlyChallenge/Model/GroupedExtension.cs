using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TrustlyChallenge.Model
{
    public class GroupedExtension
    {
        [Key]
        public int Id { get; set; }
        public string Extension { get; set; }
        public int NumberOfFiles { get; set; }
        public int NumberOfLines { get; set; }
        public int NumberOfBytes { get; set; }
    }
}
