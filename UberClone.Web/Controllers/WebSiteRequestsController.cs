using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UberClone.Web.Models;

namespace UberClone.Web.Controllers
{
    public class WebSiteRequestsController : Controller
    {
        private dbcontext db = new dbcontext();

        // GET: WebSiteRequests
        public async Task<ActionResult> Index()
        {
            return View(await db.Requests.ToListAsync());
        }

        // GET: WebSiteRequests/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requests requests = await db.Requests.FindAsync(id);
            if (requests == null)
            {
                return HttpNotFound();
            }
            return View(requests);
        }

        // GET: WebSiteRequests/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebSiteRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "request_id,requester_username,requester_longitude,requester_latitude,driver_usename")] Requests requests)
        {
            if (ModelState.IsValid)
            {
                db.Requests.Add(requests);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(requests);
        }

        // GET: WebSiteRequests/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requests requests = await db.Requests.FindAsync(id);
            if (requests == null)
            {
                return HttpNotFound();
            }
            return View(requests);
        }

        // POST: WebSiteRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "request_id,requester_username,requester_longitude,requester_latitude,driver_usename")] Requests requests)
        {
            if (ModelState.IsValid)
            {
                db.Entry(requests).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(requests);
        }

        // GET: WebSiteRequests/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requests requests = await db.Requests.FindAsync(id);
            if (requests == null)
            {
                return HttpNotFound();
            }
            return View(requests);
        }

        // POST: WebSiteRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Requests requests = await db.Requests.FindAsync(id);
            db.Requests.Remove(requests);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
