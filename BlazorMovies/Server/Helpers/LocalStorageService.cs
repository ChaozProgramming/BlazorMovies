namespace BlazorMovies.Server.Helpers
{
    public class LocalStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment env;   //Microsoft Hosting
        private readonly IHttpContextAccessor httpContextAccessor; //read the requested url

        public LocalStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) 
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task DeleteFile(string fileRoute, string containerName)
        {
            var fileName = Path.GetFileName(fileRoute);
            string fileDirectory = Path.Combine(env.WebRootPath, containerName, fileName);
            if (File.Exists(fileDirectory))
                File.Delete(fileDirectory);

            return Task.FromResult(0);
        }

        public async Task<string> EditFile(byte[] content, string extension, string containerName, string fileRoute)
        {
            if(!string.IsNullOrEmpty(fileRoute))
                await DeleteFile(fileRoute, containerName);

            return await SaveFile(content, extension, containerName);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string containerName)
        {
            var filename = $"{Guid.NewGuid()}.{extension}";
            //WebRootPath will be null if you not created the wwwroot folder in your project
            string folder = Path.Combine(env.WebRootPath, containerName);

            if(!Directory.Exists(folder))   
                Directory.CreateDirectory(folder);

            string savingPath = Path.Combine(folder, filename);
            await File.WriteAllBytesAsync(savingPath, content);

            var currentUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var pathForDatabase = Path.Combine(currentUrl, containerName, filename);
            return pathForDatabase;
        }
    }
}
