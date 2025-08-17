using System.Reflection;
using System.Windows.Markup;

namespace MineSweeper.Factory
{
    public class AssemblyInfoExtension : MarkupExtension
    {
        public string? Property { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return Property?.ToLower() switch
            {
                "version" => assembly.GetName().Version?.ToString() ?? "N/A",
                "fileversion" => assembly
                    .GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "N/A",
                "informationalversion" => assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                    .InformationalVersion?.Split('+')[0] ?? "N/A",
                _ => "N/A"
            };
        }
    }
}
