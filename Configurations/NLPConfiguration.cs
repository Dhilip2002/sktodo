using sktodo.Models;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace sktodo.Configurations
{

    public class NLPConfiguration
    {
        // Load the .env file
        string githubKey = Env.GetString("GITHUB_KEY");
        string model = Env.GetString("MODELID");
        public string MODELID { get; set; }
        public string GITHUB_KEY { get; set; }

        public NLPConfiguration()
        {
            GITHUB_KEY = githubKey;
            MODELID = model;
        }
    }
}
