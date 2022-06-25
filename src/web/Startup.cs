using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using core.interfaces;
using core.models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using web.Controllers;

namespace dynata.web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;

        }
        public IConfiguration Configuration { get; }

        private IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IContentSource>(x => new MemoryContentProvider(GetContent()));
            services.AddSingleton<IStorageProvider>(x => new FileStorageProvider(Path.Combine(_env.ContentRootPath, "fs")));
            services.AddSingleton<IContentProvider, FileSystemContentProvider>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFolderService, FolderService>();
            services.AddControllersWithViews();

        }

        readonly ConcurrentDictionary<Guid, IFsEntity> _fsContent = new ConcurrentDictionary<Guid, IFsEntity>();

        private IDictionary<Guid, IFsEntity> GetContent()
        {
            if (!_fsContent.Any())
            {
                var id = Guid.NewGuid();
                _fsContent.TryAdd(id, new folder()
                {
                    Id = id,
                    Name = "fs"
                });
            }

            return _fsContent;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
