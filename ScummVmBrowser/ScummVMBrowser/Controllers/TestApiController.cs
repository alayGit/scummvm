using ScummVMBrowser.Models;
using ScummVMBrowser.StaticData;
using StartInstance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScummVMBrowser.Controllers
{
    public class TestApiController : Controller
    {
        public const string ScummVMLocation = "C:\\ScummVMNew\\ScummVMWeb\\GIT\\dists\\SignalRSelfHost\\bin\\Debug\\SignalRSelfHost.exe";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StartGame()
        {
            int? port = (int?) Session[SessionKeys.Port];
            bool gameRequiresStarting = false;
            if (port == null)
            {
                port = StartScummVM();
                Session[SessionKeys.Port] = port;

                gameRequiresStarting = true;
            }

            return Json(new TestGameInfo() { Port = port, GameRequiresStarting = gameRequiresStarting }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private int StartScummVM()
        {
            return 0; //new InstanceStarter(StartScummVM).StartInstance();
        }

        private bool StartScummVM(int port)
        {
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = ScummVMLocation;
                myProcess.StartInfo.CreateNoWindow = false;
                myProcess.StartInfo.Arguments = port.ToString();
                
                return myProcess.Start();
            }
        }
    }
}