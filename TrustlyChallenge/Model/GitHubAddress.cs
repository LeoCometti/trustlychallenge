using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrustlyChallenge.Model
{
    public class GitHubAddress
    {
        public GitHubAddress()
        {

        }

        public GitHubAddress(string rootUrl)
        {
            var split = rootUrl.Split('/');

            if (split != null)
            {
                var proceed = false;

                for (int i = 0; i < split.Length; i++)
                {
                    if (split[i].Equals("github.com") || split[i].Equals("api.github.com"))
                    {
                        proceed = true;
                    }

                    if (proceed)
                    {
                        var index = !split[i + 1].Equals("repos") ? i + 1 : i + 2;

                        if (split.Length >= index + 1)
                        {
                            RepositoryOwner = split[index];
                            RepositoryName = split[index + 1];
                            PathContent = "";
                            IsValidUrl = true;
                        }
                        break;
                    }
                }
            }
        }

        public string RepositoryOwner { get; set; }
        public string RepositoryName { get; set; }
        public string PathContent { get; set; }
        public bool IsValidUrl { get; private set; }
    }
}
