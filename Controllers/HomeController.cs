using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net.Mail;
using System.Xml.Linq;

namespace TesteTecnico.Controllers
{
    public class HomeController : Controller
    {
        private string xmlPath;
        private XDocument xDocument;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Mapa()
        {
            SelectList selectListEstados = MakeSelectListState();

            ViewBag.SelectListEstados = selectListEstados;

            return View();
        }

        private SelectList MakeSelectListState()
        {
            xmlPath = Server.MapPath("~/app_data/estados.xml");
            xDocument = XDocument.Load(xmlPath);

            IEnumerable<XElement> resultState = from c in xDocument.Elements("estados").Elements("estado")
                                                  select c;

            var listItemsState = new List<SelectListItem>();

            foreach (var xElement in resultState)
            {
                listItemsState.Add(new SelectListItem
                {
                    Text = xElement.Element("nome").Value,
                    Value = xElement.Element("idestado").Value,
                });
            }

            SelectList selectListState = new SelectList(listItemsState, "Value", "Text");

            return selectListState;
        }

        [HttpPost]
        public JsonResult MakeSelectListCity(string id)
        {
            xmlPath = Server.MapPath("~/app_data/cidades.xml");
            xDocument = XDocument.Load(xmlPath);

            IEnumerable<XElement> resultCity = from c in xDocument.Elements("cidades").Elements("cidade")
                                                  select c;

            var listItemsCity= new List<SelectListItem>();

            foreach (var xElement in resultCity)
            {
                listItemsCity.Add(new SelectListItem
                {
                    Text = xElement.Element("nome").Value,
                    Value = xElement.Element("idestado").Value,
                });
            }

            return Json(listItemsCity.Where(l => l.Value == id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendEmail(string emailTo, bool acceptCheck)
        {
            if (acceptCheck && !string.IsNullOrEmpty(emailTo)) 
            {
                var fromAddress = "fromEmail@teste.com";
                var toAddress = emailTo;
                const string fromPassword = "PasswordFromEmail";
                string subject = "Subject value here";
                string htmlBody = "Write some HTML code here";

                SmtpClient SmtpServer = new SmtpClient("smtp.teste.com");
                var mail = new MailMessage();
                mail.From = new MailAddress(fromAddress);
                mail.To.Add(toAddress);
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = htmlBody;
                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(fromAddress, fromPassword);
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);

                return RedirectToAction("Mapa", "Home");
            }
            else
                return RedirectToAction("Index", "Home");
        }
    }
}