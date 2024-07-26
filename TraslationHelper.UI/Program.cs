using TraslationHelper.BLL.Services;
using TraslationHelper.DAL.GoogleDocument.Repositories;
using TraslationHelper.Domain.Abstract.Repositories;
using TraslationHelper.Domain.Abstract.Services;
using TraslationHelper.Domain.Models.Settings;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<GoogleDocumentSettings>(configuration.GetSection("GoogleDocumentSettings"));

builder.Services.AddTransient<IGoogleDocsRepository, GoogleDocsRepository>();
builder.Services.AddTransient<IGoogleDocTranslationService, GoogleDocTranslationService>();
builder.Services.AddTransient<ITranslationUpdaterService, TranslationUpdaterService>();
builder.Services.AddTransient<IStreamService, StreamService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
