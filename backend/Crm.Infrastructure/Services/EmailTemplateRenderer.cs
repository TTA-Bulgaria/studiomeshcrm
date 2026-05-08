using Crm.Application.Interfaces;
using Scriban;
using Scriban.Runtime;

namespace Crm.Infrastructure.Services;

public class EmailTemplateRenderer : IEmailTemplateRenderer
{
    private static readonly string TemplatesPath = Path.Combine(
        AppContext.BaseDirectory, "EmailTemplates");

    public async Task<string> RenderAsync<T>(string templateName, T model) where T : notnull
    {
        var basePath = Path.Combine(TemplatesPath, "_base.html");
        var contentPath = Path.Combine(TemplatesPath, $"{templateName}.html");

        if (!File.Exists(contentPath))
            throw new FileNotFoundException($"Email template '{templateName}.html' not found.", contentPath);

        var baseSource = await File.ReadAllTextAsync(basePath);
        var contentSource = await File.ReadAllTextAsync(contentPath);

        var contentTemplate = Template.Parse(contentSource);
        var scriptObject = new ScriptObject();
        scriptObject.Import(model, renamer: member => member.Name
            .Replace("_", "")
            .ToLower());
        var context = new TemplateContext { MemberRenamer = member => member.Name.ToLower() };
        context.PushGlobal(scriptObject);
        var renderedContent = await contentTemplate.RenderAsync(context);

        var baseTemplate = Template.Parse(baseSource);
        var baseScriptObject = new ScriptObject();
        baseScriptObject.Import(model, renamer: member => member.Name.ToLower());
        baseScriptObject["content"] = renderedContent;
        var baseContext = new TemplateContext { MemberRenamer = member => member.Name.ToLower() };
        baseContext.PushGlobal(baseScriptObject);

        return await baseTemplate.RenderAsync(baseContext);
    }
}
