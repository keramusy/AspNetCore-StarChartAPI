using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var obj = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if(obj == null)
            {
                return NotFound();
            }

            var sats = _context.CelestialObjects.Where(co => co.OrbitedObjectId == id);
            obj.Satellites = sats.ToList();

            return Ok(obj);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var obj = _context.CelestialObjects.Where(co => co.Name == name).ToList();

            if (obj == null || !obj.Any())
            {
                return NotFound();
            }

            var result = new List<CelestialObject>();
            foreach (var co in obj)
            {
                var sats = _context.CelestialObjects.Where(cob => cob.OrbitedObjectId == co.Id);
                co.Satellites = sats.ToList();

                result.Add(co);
            }

            return Ok(result);
        }

        [HttpGet(Name = "GetByName")]
        public IActionResult GetAll()
        {
            var result = new List<CelestialObject>();
            foreach(var co in _context.CelestialObjects)
            {
                var sats = _context.CelestialObjects.Where(cob => cob.OrbitedObjectId == co.Id);
                co.Satellites = sats.ToList();

                result.Add(co);
            }

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject newObject)
        {
            _context.CelestialObjects.Add(newObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new {id = newObject.Id}, newObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject newObject)
        {
            var obj = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            obj.Name = newObject.Name;
            obj.OrbitalPeriod = newObject.OrbitalPeriod;
            obj.OrbitedObjectId = newObject.OrbitedObjectId;

            _context.CelestialObjects.Update(obj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var obj = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            obj.Name = name;

            _context.CelestialObjects.Update(obj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var obj = _context.CelestialObjects.Where(co => co.Id == id || co.OrbitedObjectId == id).ToList();

            if (obj == null || !obj.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(obj);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
