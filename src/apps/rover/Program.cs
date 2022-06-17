// See https://aka.ms/new-console-template for more information
IHost host = Host.CreateDefaultBuilder(args)
.ConfigureServices((hostcontext, services) => {
    var configurationRoot = hostcontext.Configuration;
    services.AddOptions();
    //services.Configure<RoverSettings>(configurationRoot.GetSection(nameof(RoverSettings)));
    //services.Configure<IntegrationSettings>(configurationRoot.GetSection(nameof(IntegrationSettings)));
    //services.Configure<MarsSettings>(configurationRoot.GetSection(nameof(MarsSettings)));
    services.AddScoped<InitMarsService>();
})
.Build();

await host.Services.GetService<InitMarsService>()?.InitMarsAsync(CancellationToken.None);
await host.RunAsync();
