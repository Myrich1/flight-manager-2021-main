using flight_manager_2021.Models;
using flight_manager_2021.Models.Flights;
using flight_manager_2021.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Data;
using Data.Entity;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace flight_manager_2021.Controllers
{
    public class FlightController : Controller
    {
        private const int PageSize = 10;
        private readonly ConnectionDB _context;

        public FlightController()
        {
            _context = new ConnectionDB();
        }

        //GET : Flight
        public async Task<IActionResult> Index(FlightsIndexViewModel model)
        {
            model.Pager ??= new PagerViewModel();
            model.Pager.CurrentPage = model.Pager.CurrentPage <= 0 ? 1 : model.Pager.CurrentPage;

            List<FlightsViewModel> items =await _context.Flights.Skip((model.Pager.CurrentPage-1)*PageSize).Take(PageSize).Select(c=>new FlightsViewModel()         
            {
                LocationFrom=c.LocationFrom,
                LocationTo=c.LocationTo,
                Going=c.Going,
                Return=c.Return,
                TypeOfPlane=c.TypeOfPlane,
                PlaneID=c.PlaneID,
                NameOfAviator=c.NameOfAviator,
                CapacityOfEconomyClass=c.CapacityOfEconomyClass,
                CapacityOfBusinessClass=c.CapacityOfBusinessClass

            }).ToListAsync();

            model.Items = items;
            model.Pager.PagesCount = (int)Math.Ceiling(await _context.Flights.CountAsync() / (double)PageSize);

            return View(model);
        }

        //GET: Flight/Create

        public IActionResult Create()
        {
            FlightsCreateViewModel model = new FlightsCreateViewModel();

            return View(model);
        }
       
        //Post: Flight/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create (FlightsCreateViewModel createModel)
        {
            if (ModelState.IsValid)
            {
                Flight flight = new Flight
                {
                    LocationFrom = createModel.LocationFrom,
                    LocationTo = createModel.LocationTo,
                    Going=createModel.Going,
                    Return=createModel.Return,
                    TypeOfPlane=createModel.TypeOfPlane,
                    NameOfAviator=createModel.NameOfAviator,
                    CapacityOfEconomyClass=createModel.CapacityOfEconomyClass,
                    CapacityOfBusinessClass=createModel.CapacityOfBusinessClass
                };
                _context.Add(flight);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(createModel);
        }
        //GET: Flight/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            Flight flight = await _context.Flights.FindAsync(id);
            if (flight==null)
            {
                return NotFound();
            }
            FlightsEditViewModel model = new FlightsEditViewModel
            {
                LocationFrom = flight.LocationFrom,
                LocationTo = flight.LocationTo,
                Going = flight.Going,
                Return = flight.Return,
                TypeOfPlane = flight.TypeOfPlane,
                PlaneID = flight.PlaneID,
                NameOfAviator = flight.NameOfAviator,
                CapacityOfEconomyClass = flight.CapacityOfEconomyClass,
                CapacityOfBusinessClass = flight.CapacityOfBusinessClass
            };
            return View(model);
        }
        //POST : Flights/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FlightsEditViewModel editModel)
        {
            if (ModelState.IsValid)
            {
                Flight flight = new Flight()
                {
                    LocationFrom=editModel.LocationFrom,
                    LocationTo = editModel.LocationTo,
                    Going = editModel.Going,
                    Return = editModel.Return,
                    TypeOfPlane = editModel.TypeOfPlane,
                    NameOfAviator = editModel.NameOfAviator,
                    CapacityOfEconomyClass = editModel.CapacityOfEconomyClass,
                    CapacityOfBusinessClass = editModel.CapacityOfBusinessClass
                };
                try
                {
                    _context.Update(flight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExist(flight.PlaneID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(editModel);
        }
        
        //GET : Flight/Delete
        public async Task<IActionResult> Delete(int id)
        {
            Flight flight = await _context.Flights.FindAsync(id);
            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool FlightExist(int id)
        {
            return _context.Flights.Any(e => e.PlaneID == id);
        }
    }
}
