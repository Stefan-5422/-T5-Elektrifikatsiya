using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Elektrifikatsiya;
using Blazorise;

namespace Elektrifikatsiya.Pages
{
    public partial class Dashboard
    {
        string bgcolor { get; set; } = "00f"; // (starting value)
        void SetColor()
        {
            bgcolor = "#fd7";
            StateHasChanged(); // may not be required, but I'm at work right now, so can't check
        }
    }
}