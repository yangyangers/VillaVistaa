using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using VillaVista.Models;

namespace VillaVista.ViewModels
{
    public class HomeownerDashboardViewModel
    {
        public List<Announcement> Announcements { get; set; }
        public Billing PaymentSummary { get; set; }
        public List<Event> Events { get; set; }
    }
}
