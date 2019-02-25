using BionicExtensions.Attributes;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

namespace RTM_Blazor.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            InjectableAttribute.RegisterInjectables(services);
            services
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app
            .UseBootstrapProviders()
            .UseFontAwesomeIcons();
            app.AddComponent<App>("app");
        }
    }
}
