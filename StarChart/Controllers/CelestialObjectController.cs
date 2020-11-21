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
    }
}
