using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Message.Dal.Abstract;
using Message.Dal.Concrete;
using Message.Dal.Model;
using Message.Business.Concrete;
using Message.Business.Abstract;
using Nest;
using Message.Extensions;
using Message.Validation;
using FluentValidation;
namespace Message
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSingleton(Configuration);
            services.AddSingleton<IEntityRepository<MessageModel>, ElasticRepository>();
            services.AddSingleton<IElasticService, ElasticService>();
            services.AddSingleton<IElasticClient>(
            p =>
            {
                var elastic = new ElasticClient(new ConnectionSettings(new Uri(Configuration["elasticsearchserver:Host"]))
                .BasicAuthentication(Configuration["elasticsearchserver:Username"], Configuration["elasticsearchserver:Password"]));
                var any = elastic.Indices.Exists(Configuration["elasticsearchserver:indexName"].ToString());
                if (!any.Exists)
                {
                    elastic.Indices.Create(Configuration["elasticsearchserver:indexName"].ToString(), ci => ci.Index(Configuration["elasticsearchserver:indexName"].ToString()).MessageMapping().Settings(s => s.NumberOfShards(3).NumberOfReplicas(1)));
                }
                return elastic;
            });
            services.AddTransient<IValidator<MessageModel>, MessageModelValidation>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
