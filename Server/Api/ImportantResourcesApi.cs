using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Server.Models;
using System.Security.Claims;

namespace Server.Api
{
    public class ImportantResourcesApi : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapGet("/api/Resources/Important",GetImportantResource);
            //app.MapGet("api/Resources/Tmp1",Tmp1);
            //app.MapGet("api/Resources/Tmp2",Tmp2);
            //app.MapGet("api/Resources/Tmp3",Tmp3);
            //app.MapGet("api/Resources/GLTF",GLTF);
        }
		private IResult GLTF(HttpResponse response,IWebHostEnvironment environment)
		{
			var contentRoot = environment.ContentRootPath;
			var path = Path.Combine(contentRoot, "products/TMP/GLTF");
			var fpath = Directory.EnumerateFiles(path, "untitled.glb").SingleOrDefault();
            response.Headers.CacheControl = new[] { "public", "max-age=12" };
            return Results.File(fpath);
		}
		private IResult Tmp3(IWebHostEnvironment environment)
		{
			var contentRoot = environment.ContentRootPath;
			var path = Path.Combine(contentRoot, "products/TMP/4");
			var fpath = Directory.EnumerateFiles(path, "model.zip").SingleOrDefault();
			return Results.File(fpath);
		}

		private IResult Tmp2(IWebHostEnvironment environment)
		{
			var contentRoot = environment.ContentRootPath;
			var path = Path.Combine(contentRoot, "products/TMP/1");
			var fpath = Directory.EnumerateFiles(path, "untitled.mtl").SingleOrDefault();
			return Results.File(fpath);
		}

		private IResult Tmp1(IWebHostEnvironment environment)
        {
            var contentRoot = environment.ContentRootPath;
			var path = Path.Combine(contentRoot, "products/TMP/1");
			var fpath = Directory.EnumerateFiles(path, "untitled.obj").SingleOrDefault();
			return Results.File(fpath);
		}

		[Authorize(Policy = "Auth",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IResult GetImportantResource(HttpContext httpContext)
        {
            var user = httpContext.User;
            var x = user.FindFirstValue("Id");
            return Results.Ok("This is a very important resource. This token belongs to user with id: " + x);
        } 

    }
}
