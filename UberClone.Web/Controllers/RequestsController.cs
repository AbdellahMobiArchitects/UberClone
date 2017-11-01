using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using UberClone.Web.Models;

namespace UberClone.Web.Controllers
{
    public class RequestsController : ApiController
    {
        private dbcontext db = new dbcontext();

        // GET: api/Requests
        public IQueryable<Requests> GetRequests()
        {
            return db.Requests;
        }
        // GET: api/Requests/GetThisUserRequest
        [Route("api/Requests/GetThisUserRequest")]
        [HttpGet]
        public async Task<Requests> GetThisUserRequest(string username)
        {
            List<Requests> requests = await  db.Requests.Where(x => x.requester_username == username).ToListAsync<Requests>();
            
            return requests.FirstOrDefault<Requests>();
        }

        // GET: api/Requests/GetRequestsWithoutDriver
        [Route("api/Requests/GetRequestsWithoutDriver")]
        [HttpGet]
        public IQueryable<Requests> GetRequestsWithoutDriver()
        {
            return db.Requests.Where(x => x.driver_usename == null);
        }

        // GET: api/Requests/5
        [ResponseType(typeof(Requests))]
        public async Task<IHttpActionResult> GetRequests(int id)
        {
            Requests requests = await db.Requests.FindAsync(id);
            if (requests == null)
            {
                return NotFound();
            }

            return Ok(requests);
        }

        // PUT: api/Requests/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRequests(int id, Requests requests)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != requests.request_id)
            {
                return BadRequest();
            }

            db.Entry(requests).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Requests
        [ResponseType(typeof(Requests))]
        public async Task<IHttpActionResult> PostRequests(Requests requests)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Requests.Add(requests);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = requests.request_id }, requests);
        }

        // DELETE: api/Requests/5
        [ResponseType(typeof(Requests))]
        public async Task<IHttpActionResult> DeleteRequests(int id)
        {
            Requests requests = await db.Requests.FindAsync(id);
            if (requests == null)
            {
                return NotFound();
            }

            db.Requests.Remove(requests);
            await db.SaveChangesAsync();

            return Ok(requests);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RequestsExists(int id)
        {
            return db.Requests.Count(e => e.request_id == id) > 0;
        }
    }
}