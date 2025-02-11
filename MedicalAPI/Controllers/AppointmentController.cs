﻿using MedicalAPI.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MedicalAPI.Application.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalAPI.Infrastructure.Presistance;
using MedicalAPI.Application.MedicalDto;
using Microsoft.AspNetCore.Authorization;
using MedicalAPI.Application.ApplicationUser;
using System.Security.Claims;

namespace MedicalAPI.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly MedicalDbContext _dbContext;
        private readonly IUserContext _userContext;

        public AppointmentController(IAppointmentService appointmentService,MedicalDbContext dbContext, IUserContext userContext)
        {
            _appointmentService = appointmentService;
            _dbContext = dbContext;
            _userContext = userContext;
        }
        [Authorize]
        public IActionResult Create()
        {
           /* if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }*/
          

            var patients = _dbContext.Patient
        .Select(p => new
        {
            p.PatientId,
            FullName = p.FirstName + " " + p.LastName
        })
        .ToList();


        var doctors = _dbContext.Doctor
        .Select(d => new 
        { 
            d.DoctorId, 
            FullName = d.FirstName + " " + d.LastName 
        })
        .ToList();

            // Przekazanie danych do widoku za pomocą ViewBag
            ViewBag.Patients = new SelectList(patients, "PatientId", "FullName");
            ViewBag.Doctors = new SelectList(doctors, "DoctorId", "FullName");

            // Przekazanie danych do widoku za pomocą ViewBag
            ViewBag.Patients = new SelectList(patients, "PatientId", "FullName");


            return View();
        }

        [HttpPost]
        
        public async Task<IActionResult> Create(Application.MedicalDto.AppointmentDto appointment)
        {
            /*if(!User.IsInRole("Patient"))
            {
                return RedirectToAction("NoAccess", "Home");
            }*/
            User.IsInRole("Patient");
            appointment.CreatedById = _userContext.GetCurrentUser().Id;
         
            await _appointmentService.Create(appointment);
            return RedirectToAction(nameof(Create)); //todo refactor tymczasowo tak żeby nie sadziło błedu, potem gdzies indziej przekierowanie zrobic


        }

        [HttpGet]
               public async Task<IActionResult> Index(Application.MedicalDto.AppointmentDto appointment)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Pobierz ID zalogowanego użytkownika
            var appointments = await _appointmentService.GetAppointmentsByUserIdAsync(userId); // Pobierz wizyty użytkownika
            return View(appointments); // Przekaż do widoku
        }
    }
}
