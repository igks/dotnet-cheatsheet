//inside Configure method in Start up file
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
    {
        context.Request.Path = "/index.html";
    }
});

app.UseDefaultFiles();
app.UseStaticFiles();
