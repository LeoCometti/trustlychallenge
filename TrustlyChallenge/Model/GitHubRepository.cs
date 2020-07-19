using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TrustlyChallenge.Model
{
    public class GitHubRepository
    {
        public GitHubRepository()
        {
            FileExtensionInfo = new List<GroupedExtension>();
        }

        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "This filed cannot be empty")]
        public string Url { get; set; }
        public List<GroupedExtension> FileExtensionInfo { get; set; }
    }
}
