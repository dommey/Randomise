using Randomise.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Randomise.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                Random random = new Random();
                int seed = random.Next();
                RandomiseDataContext dataContext = new RandomiseDataContext();
                foreach (var place in dataContext.Places.OrderBy(s => (~(s.ID & seed)) & (s.ID | seed)).Take(3))
                {
                    builder.Append(place.Name);
                    builder.Append("<br />");
                }
                
                ViewBag.Message += builder.ToString();
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.ToString();
            }
            
            return View();
        }

        public ActionResult Create()
        {
            var placeModel = new PlaceModel();
            return View(placeModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PlaceModel placeModel)
        {
            if (!ModelState.IsValid)
            {
                return View(placeModel);
            }

            RandomiseDataContext dataContext = new RandomiseDataContext();

            try
            {
                var place = new Place();
                place.Name = placeModel.Name;
                place.Address = placeModel.Address;
                dataContext.Places.InsertOnSubmit(place);
                dataContext.SubmitChanges();
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.ToString();
                return View(placeModel);
            }                    

            return RedirectToAction("Index");
        }

        public ActionResult Edit()
        {
            RandomiseDataContext dataContext = new RandomiseDataContext();

            var editModel = new EditModel();
            
            editModel.Places = dataContext.Places.Select(item =>
                new PlaceModel
                {
                    ID = item.ID.ToString(),
                    Name = item.Name,
                    Address = item.Address
                }).ToList();
            
            return View(editModel);
        }

        public ActionResult EditDetails(int id)
        {
            RandomiseDataContext dataContext = new RandomiseDataContext();

            var place = dataContext.Places.Where(item => item.ID == id).FirstOrDefault();
            if (place == null)
            {
                return RedirectToAction("Index");
            }

            var placeModel = new PlaceModel();
            placeModel.ID = place.ID.ToString();
            placeModel.Name = place.Name;
            placeModel.Address = place.Address;

            return View(placeModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDetails(PlaceModel placeModel)

        {
            if (!ModelState.IsValid)
            {
                return View(placeModel);
            }

            RandomiseDataContext dataContext = new RandomiseDataContext();
            var place = dataContext.Places.Where(item => item.ID.ToString() == placeModel.ID).FirstOrDefault();
            if (place == null)
            {
                return RedirectToAction("Index");
            }

            try
            {                
                place.Name = placeModel.Name;
                place.Address = placeModel.Address;

                dataContext.SubmitChanges();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.ToString();
                return View(placeModel);
            }

            return RedirectToAction("Index");
        }
    }
}