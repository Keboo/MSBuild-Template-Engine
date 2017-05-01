namespace MTE.Core
{
    public interface ITemplate
    {
        TemplateResult Execute(Config config);
    }
}