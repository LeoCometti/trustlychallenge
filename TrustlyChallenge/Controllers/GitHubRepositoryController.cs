using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TrustlyChallenge.Data;
using TrustlyChallenge.Model;

namespace TrustlyChallenge.Controllers
{
    [Route("api/GitHubRepository")]
    [ApiController]
    public class GitHubRepositoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<GitHubRepository>>> Get([FromServices] DataContext context)
        {
            var gitHubRepositories = await context.GitHubRepo
                .Include(x => x.GroupedExtensionInfo)
                .AsNoTracking()
                .ToListAsync();
            return gitHubRepositories;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<GitHubRepository>> Post(
            [FromServices] DataContext context,
            [FromBody] GitHubRepository model)
        {
            if (ModelState.IsValid)
            {
                var gitHubAddress = new GitHubAddress(model.Url);

                if (gitHubAddress.IsValidUrl)
                {
                    var httpClientResults = await ListAllContents(gitHubAddress);

                    model.GroupedExtensionInfo.AddRange(GetResultGroupedByFileExtension(httpClientResults));

                    //model.FileExtensionInfo.AddRange(GetResultOfEachFile(httpClientResults)); // DEBUG -> Check the number of lines and size length of each file
                }

                context.GitHubRepo.Add(model);
                await context.SaveChangesAsync();
                return model;
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        static List<GroupedExtension> GetResultGroupedByFileExtension(List<FileInformation> results)
        {
            var groupedExtensions = new List<GroupedExtension>();

            foreach (var file in results)
            {
                if (groupedExtensions.Count(i => i.Extension == file.Extension) == 0)
                {
                    groupedExtensions.Add(new GroupedExtension()
                    {
                        Extension = file.Extension,
                        NumberOfFiles = 1,
                        NumberOfLines = file.Lines,
                        NumberOfBytes = file.Size,
                    });
                }
                else
                {
                    var reference = groupedExtensions.Where(i => i.Extension == file.Extension).FirstOrDefault();

                    reference.NumberOfFiles++;
                    reference.NumberOfLines += file.Lines;
                    reference.NumberOfBytes += file.Size;
                }
            }

            return groupedExtensions;
        }

        /// <summary>
        /// This method was created for debug purposes
        /// </summary>
        static List<GroupedExtension> GetResultOfEachFile(List<FileInformation> results)
        {
            var groupedExtensions = new List<GroupedExtension>();

            foreach (var file in results)
            {
                groupedExtensions.Add(new GroupedExtension()
                {
                    Extension = file.Extension,
                    NumberOfLines = file.Lines,
                    NumberOfBytes = file.Size,
                });
            }

            return groupedExtensions;
        }

        static async Task<List<FileInformation>> ListAllContents(GitHubAddress gitHubAddress)
        {
            var allFileContent = new List<FileInformation>();

            using (var client = GetGithubHttpClient())
            {
                var bodyJson = await GetJTokenFromRepository(client, gitHubAddress);

                var nameFiles = bodyJson.SelectTokens("$.[?(@.type == 'file')].name").Select(token => token.Value<string>());

                var fileInformationList = await GetFileInformationFromJson(client, gitHubAddress, nameFiles);

                foreach (var info in fileInformationList)
                {
                    allFileContent.Add(new FileInformation()
                    {
                        Extension = info.Extension,
                        Lines = info.Lines,
                        Size = info.Size,
                    });
                }

                var directories = bodyJson.SelectTokens("$.[?(@.type == 'dir')].name").Select(token => token.Value<string>());
                var currentPath = gitHubAddress.PathContent;

                foreach (var dir in directories)
                {
                    gitHubAddress.PathContent = $"{ currentPath }/{ dir }";

                    var subContent = await ListAllContents(gitHubAddress);

                    allFileContent.AddRange(subContent);
                }
            }

            return allFileContent;
        }

        static HttpClient GetGithubHttpClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri("https://api.github.com")
                {

                },
                DefaultRequestHeaders =
                {
                    {"User-Agent", "Github-API-Test" },
                },
            };
        }

        static async Task<JToken> GetJTokenFromRepository(HttpClient client, GitHubAddress gitHubContent)
        {
            var resp = await client.GetAsync($"repos/{gitHubContent.RepositoryOwner}/{gitHubContent.RepositoryName}/contents/{gitHubContent.PathContent}");
            var bodyString = await resp.Content.ReadAsStringAsync();
            return JToken.Parse(bodyString);
        }

        static async Task<List<FileInformation>> GetFileInformationFromJson(HttpClient client, GitHubAddress gitHubAddress, IEnumerable<string> nameFiles)
        {
            var fileInformation = new List<FileInformation>();

            foreach (var name in nameFiles)
            {
                var nameRefMaster = $"{ name }?ref=master";

                var subAddress = new GitHubAddress()
                {
                    RepositoryOwner = gitHubAddress.RepositoryOwner,
                    RepositoryName = gitHubAddress.RepositoryName,
                    PathContent = gitHubAddress.PathContent.Equals(string.Empty) ? $"{ nameRefMaster }" : $"{ gitHubAddress.PathContent }/{ nameRefMaster }",
                };

                var bodyJson2 = await GetJTokenFromRepository(client, subAddress);

                var contentFiles = bodyJson2.SelectTokens("$.content").Select(token => token.Value<string>());
                var sizeFiles = bodyJson2.SelectTokens("$.size").Select(token => token.Value<int>());

                var extension = GetExtension(name);
                var lines = GetLines(contentFiles.FirstOrDefault());
                var size = sizeFiles.FirstOrDefault();

                fileInformation.Add(new FileInformation()
                {
                    Extension = extension,
                    Lines = lines,
                    Size = size,
                });
            }

            return fileInformation;
        }

        static string GetExtension(string fileName)
        {
            return fileName.Substring(fileName.LastIndexOf('.'));
        }

        static int GetLines(string content)
        {
            if (content == null) return 0;

            var encoding = Encoding.UTF8;
            var bytes = Convert.FromBase64String(content);
            var sValid = encoding.GetString(bytes).TrimEnd('\n');

            return sValid.Split('\n').Length;
        }
    }
}
