using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CuentasWeb.Models;
using System.Data.SqlClient;

namespace CuentasWeb.Controllers
{
    public class MovimientosController : Controller
    {
        private DbContext1 db = new DbContext1();

         static private string GetConnectionString()
        {
            // To avoid storing the connection string in your code,
            // you can retrieve it from a configuration file, using the
            // System.Configuration.ConfigurationManager.ConnectionStrings property
            return "Data Source = DESKTOP-MTFOLLP\\MSSQLSERVER02; initial catalog = cuentas; integrated security = True";
        }
            // GET: Movimientos
            public ActionResult Index()
        {
            var cuentaid = ViewBag.IdCuenta;
            var movimientos = db.Movimientos.Include(m => m.Cuentas );
            
            return View(movimientos.ToList());
        }

        // GET: Movimientos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimientos movimientos = db.Movimientos.Find(id);
            if (movimientos == null)
            {
                return HttpNotFound();
            }
            return View(movimientos);
        }
        public ActionResult DetailsCta(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimientos movimientos = db.Movimientos.Find(id);
            if (movimientos == null)
            {
                return HttpNotFound();
            }
            return View(movimientos);
        }
        // GET: Movimientos/Create
        public ActionResult Create()
        {
            ViewBag.IdCuenta = new SelectList(db.Cuentas, "IdCuenta", "NombreCta");
            return View();
        }

        // POST: Movimientos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdMovimiento,IdCuenta,Fecha,ValorRetiro,ValorGmf,SaldoMov,CuentaDestino,TipoTransaccion")] Movimientos movimientos)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "calculoSaldo";
                cmd.Parameters.Add("@IdCta", System.Data.SqlDbType.Int).Value = movimientos.IdCuenta;
                cmd.Parameters.Add("@valorRetiro", System.Data.SqlDbType.Decimal).Value = movimientos.ValorRetiro;
                cmd.Parameters.Add("@cuentaDestino", System.Data.SqlDbType.VarChar, 50).Value = movimientos.CuentaDestino + "_sp";
                cmd.Parameters.Add("@tipoTransaccion", System.Data.SqlDbType.VarChar, 1).Value = movimientos.TipoTransaccion;
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            Movimientos movimientos1 = db.Movimientos.Find(movimientos.IdCuenta);
            //  if (ModelState.IsValid)
            //{
            //db.Movimientos.Add(movimientos);
            //db.SaveChanges();
            ViewBag.IdCuenta = new SelectList(db.Cuentas, "IdCuenta", "NombreCta", movimientos.IdCuenta);
            return RedirectToAction("Index");
            //}

            
            //Movimientos movimientos1 = db.Movimientos.Find(movimientos.IdCuenta);
            //if (movimientos1 == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(movimientos1);
        }

        // GET: Movimientos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimientos movimientos = db.Movimientos.Find(id);
            if (movimientos == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCuenta = new SelectList(db.Cuentas, "IdCuenta", "NombreCta", movimientos.IdCuenta);
            return View(movimientos);
        }

        // POST: Movimientos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdMovimiento,IdCuenta,Fecha,ValorRetiro,ValorGmf,SaldoMov,CuentaDestino,TipoTransaccion")] Movimientos movimientos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movimientos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdCuenta = new SelectList(db.Cuentas, "IdCuenta", "NombreCta", movimientos.IdCuenta);
            return View(movimientos);
        }

        // GET: Movimientos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimientos movimientos = db.Movimientos.Find(id);
            if (movimientos == null)
            {
                return HttpNotFound();
            }
            return View(movimientos);
        }

        // POST: Movimientos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movimientos movimientos = db.Movimientos.Find(id);
            db.Movimientos.Remove(movimientos);
            db.SaveChanges();
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
