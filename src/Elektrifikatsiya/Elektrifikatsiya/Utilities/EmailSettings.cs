using HandlebarsDotNet;

namespace Elektrifikatsiya.Utilities;

public class EmailSettings
{
    public string User { get; set; }
    public string DefaultEmail { get; set; }
    public string Key { get; set; }
    public Dictionary<string, string> Templates { get; set; }
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }

    public Dictionary<string, HandlebarsTemplate<object, object>> CompiledTemplates { get; } = new();

    public void CompileTemplates()
    {
        foreach ((string templateName, string templatePath) in Templates)
        {
            CompiledTemplates.Add(templateName, Handlebars.Compile(File.ReadAllText(templatePath)));
        }
    }
}