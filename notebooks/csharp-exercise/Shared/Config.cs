using DotNetEnv;

namespace Lab532.Shared;

/// <summary>
/// Loads .env from the repo root and exposes typed config used by every Part.
/// Mirrors the env vars referenced in the Python notebooks.
/// </summary>
public static class Config
{
    private static bool _loaded;

    public static void Load()
    {
        if (_loaded) return;
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, ".env");
            if (File.Exists(candidate)) { Env.Load(candidate); _loaded = true; return; }
            dir = dir.Parent;
        }
        Env.TraversePath().Load();
        _loaded = true;
    }

    private static string Require(string name) =>
        Environment.GetEnvironmentVariable(name)
        ?? throw new InvalidOperationException($"Set {name} in your .env file (see .env.sample).");

    public static string SearchEndpoint  => Require("AZURE_SEARCH_SERVICE_ENDPOINT").TrimEnd('/');
    public static string SearchApiKey    => Require("AZURE_SEARCH_ADMIN_KEY");
    public static string OpenAIEndpoint  => Require("AZURE_OPENAI_ENDPOINT").TrimEnd('/');
    public static string OpenAIKey       => Require("AZURE_OPENAI_KEY");
    public static string ChatDeployment  => Require("AZURE_OPENAI_CHATGPT_DEPLOYMENT");
    public static string ChatModel       => Require("AZURE_OPENAI_CHATGPT_MODEL_NAME");
    public static string? TenantId       => Environment.GetEnvironmentVariable("AZURE_TENANT_ID");

    public static string? WebIqKey => Environment.GetEnvironmentVariable("WEB_IQ_KEY");
    public static string? FabricWorkspaceId => Environment.GetEnvironmentVariable("FABRIC_WORKSPACE_ID");
    public static string? FabricOntologyId  => Environment.GetEnvironmentVariable("FABRIC_ONTOLOGY_ID");
}
