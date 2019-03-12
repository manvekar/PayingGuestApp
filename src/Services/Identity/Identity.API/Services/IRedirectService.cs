using System;
using System.Collections.Generic;
using System.Text;

namespace FoodApp.Services.Identity.API.Services
{
    public interface IRedirectService
    {
        string ExtractRedirectUriFromReturnUrl(string url);
    }
}
