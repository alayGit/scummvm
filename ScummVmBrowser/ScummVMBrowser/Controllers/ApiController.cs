using ScummVMBrowser.Clients;
using ScummVMBrowser.Data;
using ScummVMBrowser.Models;
using ScummVMBrowser.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ScummVMBrowser.Utilities;
using SignalRHostWithUnity;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;

namespace ScummVMBrowser.Controllers
{
    public class ApiController : Controller
    {
        private IGameClientStore<IGameInfo> HubStore { get; set; }

       public ApiController(IGameClientStore<IGameInfo> hubStore)
        {
            HubStore = hubStore;
        }

        public ActionResult GetControlKeysJson()
        {
            return Content(EnumExtensions.EnumToString<ControlKeys>());   
        }
    }
}