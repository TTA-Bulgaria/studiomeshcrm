namespace Crm.Application.Interfaces;

public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync<T>(string templateName, T model) where T : notnull;
}
