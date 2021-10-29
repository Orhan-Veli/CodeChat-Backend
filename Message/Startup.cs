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
using Message.Dal.SignalRHub;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
namespace Message
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                              builder =>
                              {
                                  builder.WithOrigins("http://localhost:8081")
                                  .AllowAnyMethod()
                                  .AllowCredentials()
                                  .AllowAnyHeader()
                                  .SetIsOriginAllowed((host) => true);
                              });
        });    
            services.AddControllers();
            services.AddSingleton(Configuration);            
            services.AddHttpContextAccessor();         
            services.AddSignalR();
            services.AddSingleton<ChatHub>();   
            services.AddSingleton<IRabbitMqRepository, RabbitMqRepository>();                               
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddSingleton<IElasticRepository<MessageModel>, ElasticRepository<MessageModel>>();
            services.AddSingleton<IElasticRepository<OnlineUserModel>, ElasticRepository<OnlineUserModel>>();
            services.AddSingleton<IElasticRepository<ReportedMessageModel>, ElasticRepository<ReportedMessageModel>>();
            services.AddSingleton<IElasticService, ElasticService>();
            services.AddSingleton<IElasticClient>(
            p =>
            {
                var elastic = new ElasticClient(new ConnectionSettings(new Uri(Configuration["elasticsearchserver:Host"]))
                .BasicAuthentication(Configuration["elasticsearchserver:Username"], Configuration["elasticsearchserver:Password"]));
                var messageIndex = elastic.Indices.Exists(Configuration["elasticsearchserver:Message"].ToString());
                var reportedIndex = elastic.Indices.Exists(Configuration["elasticsearchserver:ReportedUser"].ToString());
                if (!messageIndex.Exists)
                {
                    elastic.Indices.Create(Configuration["elasticsearchserver:Message"].ToString(), ci => ci.Index(Configuration["elasticsearchserver:Message"].ToString()).MessageMapping().Settings(s => s.NumberOfShards(3).NumberOfReplicas(1)));
                }                
                if(!reportedIndex.Exists)
                {
                    elastic.Indices.Create(Configuration["elasticsearchserver:ReportedUser"].ToString(), ci => ci.Index(Configuration["elasticsearchserver:ReportedUser"].ToString()).ReportedUserMapping().Settings(s => s.NumberOfShards(1).NumberOfReplicas(1)));
                }
                return elastic;
            });
            services.AddTransient<IValidator<MessageModel>, MessageModelValidation>();
            services.AddMvc();

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
                app.UseHsts();
            }
            //var context = app.ApplicationServices.GetService<UserDbContext>();
            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);        

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/ChatHub");
                endpoints.MapHub<UserHub>("/UserHub");
            });
        }
    }
}

