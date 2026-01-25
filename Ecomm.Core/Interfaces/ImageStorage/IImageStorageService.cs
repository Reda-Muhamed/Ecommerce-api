using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces.ImageStorage
{
   
        public interface IImageStorageService
        {
            Task<(string Url, string PublicId)> UploadAsync(
                IFormFile file,
                string folder,
                CancellationToken ct);

            Task DeleteAsync(string publicId, CancellationToken ct);
        }


    

}
