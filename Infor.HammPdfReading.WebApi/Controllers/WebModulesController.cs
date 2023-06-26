﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Infor.HammPdfReading.WebApi;
using iTextSharp.text.pdf;

namespace Infor.HammPdfReading.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebModulesController : Controller
    {
        private readonly ApplicationContext _context;

        public WebModulesController(ApplicationContext context)
        {
            _context = context;
        }

        // вапыварпвапореноапро
        private void CreateRange(int i, int count, HammPdfReader reader)
        {
            var details = reader.GetExtendedDetails(i + 1, count - 1);
            var modules = reader.GetModules(i + 1, count - 1);

            var webModules = from module in modules
                             select new WebModule(
                             module,
                             from detail in details
                             where
                             detail.Assembly == module.No &&
                             detail.Series == module.Series
                             select new WebDetail(detail));

            var matchingModules = from module in webModules
                                  where _context.WebModules.First(contextModule => 
                                  (module.Assembly == contextModule.Assembly) &&
                                  (module.Series == contextModule.Series)) != null
                                  select module;

            _context.WebModules.AddRange(from module in webModules
                                         where !matchingModules.Contains(module)
                                         select module);
            // foreach (var module in webModules)
                // _context.WebDetails.AddRange(module.Details);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;
                using (var pdfReader = new PdfReader(stream))
                {
                    var reader = new HammPdfReader(pdfReader);

                    int i = 0;
                    int step = 10;
                    for (; i < pdfReader.NumberOfPages; i += step)
                        CreateRange(i, step, reader);
                    CreateRange(i, pdfReader.NumberOfPages - i, reader);

                    return View();
                }
            }
        }

        /* // GET: WebDetails/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.WebDetails == null)
            {
                return NotFound();
            }

            var webDetail = await _context.WebDetails
                .FirstOrDefaultAsync(m => m.PartNo == id);
            if (webDetail == null)
            {
                return NotFound();
            }

            return View(webDetail);
        }

        // GET: WebDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WebDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PartNo,Item,ValidFor,Quantity,Unit,Designation")] WebDetail webDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(webDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(webDetail);
        }

        // GET: WebDetails/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.WebDetails == null)
            {
                return NotFound();
            }

            var webDetail = await _context.WebDetails.FindAsync(id);
            if (webDetail == null)
            {
                return NotFound();
            }
            return View(webDetail);
        }

        // POST: WebDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PartNo,Item,ValidFor,Quantity,Unit,Designation")] WebDetail webDetail)
        {
            if (id != webDetail.PartNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(webDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WebDetailExists(webDetail.PartNo))
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
            return View(webDetail);
        }

        // GET: WebDetails/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.WebDetails == null)
            {
                return NotFound();
            }

            var webDetail = await _context.WebDetails
                .FirstOrDefaultAsync(m => m.PartNo == id);
            if (webDetail == null)
            {
                return NotFound();
            }

            return View(webDetail);
        }

        // POST: WebDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.WebDetails == null)
            {
                return Problem("Entity set 'ApplicationContext.WebDetails'  is null.");
            }
            var webDetail = await _context.WebDetails.FindAsync(id);
            if (webDetail != null)
            {
                _context.WebDetails.Remove(webDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WebDetailExists(string id)
        {
            return (_context.WebDetails?.Any(e => e.PartNo == id)).GetValueOrDefault();
        } */
    }
}
