﻿using Contacts.Common;
using Contacts.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Contacts.Controllers
{
    public class ContactsController : ApiController
    {
        private ContactsDB db = new ContactsDB();

        [HttpGet]
        [ActionName("ContactsProcessed")]
        public IHttpActionResult GetContactsProcessed()
        {
            List<Contact> allContactsProcessed = db.Contacts.ToList();

            var RequestQueryPairs = Request.GetQueryNameValuePairs().ToDictionary(p => p.Key, p => p.Value);

            int draw = Int32.Parse(RequestQueryPairs["draw"]);

            DataTableData<Contact> dataTableData = new DataTableData<Contact>();
            dataTableData.draw = draw;
            dataTableData.recordsTotal = allContactsProcessed.Count;

            int start = Int32.Parse(RequestQueryPairs["start"]);
            int length = Int32.Parse(RequestQueryPairs["length"]);

            if (length == -1)
            {
                length = dataTableData.recordsTotal;
            }

            string search = RequestQueryPairs["searchvalue"];
            if (!String.IsNullOrEmpty(search))
            {
                allContactsProcessed = allContactsProcessed.Where(c => (c.first_name != null && c.first_name.Contains(search, StringComparison.CurrentCultureIgnoreCase)) || (c.last_name != null && c.last_name.Contains(search, StringComparison.CurrentCultureIgnoreCase))).ToList();
            }

            if (RequestQueryPairs["ordercolumn"] != null && RequestQueryPairs["orderdir"] != null)
            {
                int sortColumn = Int32.Parse(RequestQueryPairs["ordercolumn"]);
                string sortDirection = RequestQueryPairs["orderdir"];

                switch (sortColumn)
                {
                    case 0:
                        {
                            allContactsProcessed.Sort((x, y) => Utilities.SortInteger(x.id, y.id, sortDirection));
                            break;
                        }
                    case 1:
                        {
                            allContactsProcessed.Sort((x, y) => Utilities.SortString(x.first_name, y.first_name, sortDirection));
                            break;
                        }
                    case 2:
                        {
                            allContactsProcessed.Sort((x, y) => Utilities.SortString(x.last_name, y.last_name, sortDirection));
                            break;
                        }
                    case 3:
                        {
                            allContactsProcessed.Sort((x, y) => Utilities.SortString(x.company_name, y.company_name, sortDirection));
                            break;
                        }
                    case 4:
                        {
                            allContactsProcessed.Sort((x, y) => Utilities.SortString(x.address, y.address, sortDirection));
                            break;
                        }
                    case 5:
                        {
                            allContactsProcessed.Sort((x, y) => Utilities.SortString(x.city, y.city, sortDirection));
                            break;
                        }
                    case 6:
                        {
                            allContactsProcessed.Sort((x, y) => Utilities.SortString(x.state, y.state, sortDirection));
                            break;
                        }
                    case 7:
                        {
                            allContactsProcessed.Sort((x, y) => Utilities.SortString(x.zip, y.zip, sortDirection));
                            break;
                        }
                    case 8:
                        {
                            allContactsProcessed.Sort((x, y) => Utilities.SortString(x.phone, y.phone, sortDirection));
                            break;
                        }
                    case 9:
                        {
                            allContactsProcessed.Sort((x, y) => Utilities.SortString(x.work_phone, y.work_phone, sortDirection));
                            break;
                        }
                    case 10:
                        {
                            allContactsProcessed.Sort((x, y) => Utilities.SortString(x.email, y.email, sortDirection));
                            break;
                        }
                    case 11:
                        {
                            allContactsProcessed.Sort((x, y) => Utilities.SortString(x.url, y.url, sortDirection));
                            break;
                        }
                    case 12:
                        {
                            allContactsProcessed.Sort((x, y) => Utilities.SortDateTime(x.dob, y.dob, sortDirection));
                            break;
                        }

                }
            }

            dataTableData.recordsFiltered = allContactsProcessed.Count;
            allContactsProcessed = allContactsProcessed.GetRange(start, Math.Min(length, allContactsProcessed.Count - start));
            dataTableData.data = allContactsProcessed;
            return Ok(dataTableData);
        }

        [HttpGet]
        [ActionName("ContactDetails")]
        [ResponseType(typeof(Contact))]
        public IHttpActionResult GetContactDetails(int id)
        {
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        [HttpPost]
        [ActionName("ContactAdd")]
        [ResponseType(typeof(Contact))]
        public IHttpActionResult ContactAdd(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Contacts.Add(contact);
            db.SaveChanges();

            return CreatedAtRoute("ActionApi", new { id = contact.id }, contact);
        }

        [HttpPut]
        [ActionName("ContactUpdate")]
        [ResponseType(typeof(void))]
        public IHttpActionResult ContactUpdate(int id, Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contact.id)
            {
                return BadRequest();
            }

            db.Entry(contact).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.OK);
        }

        [HttpDelete]
        [ActionName("ContactDelete")]
        [ResponseType(typeof(Contact))]
        public IHttpActionResult ContactDelete(int id)
        {
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return NotFound();
            }

            db.Contacts.Remove(contact);
            db.SaveChanges();

            return Ok(contact);
        }

        [HttpGet]
        [ActionName("ContactsSeed")]
        [ResponseType(typeof(void))]
        public IHttpActionResult ContactsSeed()
        {
            db.Contacts.RemoveRange(db.Contacts);
            db.SaveChanges();

            db.Database.ExecuteSqlCommand("DBCC CHECKIDENT (Contacts, reseed, 1000)");

            db.Contacts.AddOrUpdate
               (c => c.id,
               new Contact { id = 1001, address = "Address for Contact1001", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1001@Company.com", url = "Contact1001.Company.com", first_name = "MACK", last_name = "VICTOR", phone = "(123) 123 - 1001", work_phone = "(123) 123 - 1001" },
               new Contact { id = 1002, address = "Address for Contact1002", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1002@Company.com", url = "Contact1002.Company.com", first_name = "LUKE", last_name = "RICHARD", phone = "(123) 123 - 1002", work_phone = "(123) 123 - 1002" },
               new Contact { id = 1003, address = "Address for Contact1003", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1003@Company.com", url = "Contact1003.Company.com", first_name = "BRENT", last_name = "RALPH", phone = "(123) 123 - 1003", work_phone = "(123) 123 - 1003" },
               new Contact { id = 1004, address = "Address for Contact1004", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1004@Company.com", url = "Contact1004.Company.com", first_name = "CARTER", last_name = "CHARLES", phone = "(123) 123 - 1004", work_phone = "(123) 123 - 1004" },
               new Contact { id = 1005, address = "Address for Contact1005", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1005@Company.com", url = "Contact1005.Company.com", first_name = "MASON", last_name = "ANTONIO", phone = "(123) 123 - 1005", work_phone = "(123) 123 - 1005" },
               new Contact { id = 1006, address = "Address for Contact1006", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1006@Company.com", url = "Contact1006.Company.com", first_name = "CARSON", last_name = "LUIS", phone = "(123) 123 - 1006", work_phone = "(123) 123 - 1006" },
               new Contact { id = 1007, address = "Address for Contact1007", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1007@Company.com", url = "Contact1007.Company.com", first_name = "VINCENT", last_name = "HARRY", phone = "(123) 123 - 1007", work_phone = "(123) 123 - 1007" },
               new Contact { id = 1008, address = "Address for Contact1008", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1008@Company.com", url = "Contact1008.Company.com", first_name = "BLAKE", last_name = "NATHAN", phone = "(123) 123 - 1008", work_phone = "(123) 123 - 1008" },
               new Contact { id = 1009, address = "Address for Contact1009", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1009@Company.com", url = "Contact1009.Company.com", first_name = "EVERETT", last_name = "GARY", phone = "(123) 123 - 1009", work_phone = "(123) 123 - 1009" },
               new Contact { id = 1010, address = "Address for Contact1010", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1010@Company.com", url = "Contact1010.Company.com", first_name = "VANCE", last_name = "SHERRY", phone = "(123) 123 - 1010", work_phone = "(123) 123 - 1010" },
               new Contact { id = 1011, address = "Address for Contact1011", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1011@Company.com", url = "Contact1011.Company.com", first_name = "RYAN", last_name = "RUBY", phone = "(123) 123 - 1011", work_phone = "(123) 123 - 1011" },
               new Contact { id = 1012, address = "Address for Contact1012", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1012@Company.com", url = "Contact1012.Company.com", first_name = "NICHOLAS", last_name = "ALFRED", phone = "(123) 123 - 1012", work_phone = "(123) 123 - 1012" },
               new Contact { id = 1013, address = "Address for Contact1013", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1013@Company.com", url = "Contact1013.Company.com", first_name = "OLIVER", last_name = "ADAM", phone = "(123) 123 - 1013", work_phone = "(123) 123 - 1013" },
               new Contact { id = 1014, address = "Address for Contact1014", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1014@Company.com", url = "Contact1014.Company.com", first_name = "CRUZ", last_name = "FLORENCE", phone = "(123) 123 - 1014", work_phone = "(123) 123 - 1014" },
               new Contact { id = 1015, address = "Address for Contact1015", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1015@Company.com", url = "Contact1015.Company.com", first_name = "RICHARD", last_name = "JEAN", phone = "(123) 123 - 1015", work_phone = "(123) 123 - 1015" },
               new Contact { id = 1016, address = "Address for Contact1016", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1016@Company.com", url = "Contact1016.Company.com", first_name = "GABRIEL", last_name = "JOSEPH", phone = "(123) 123 - 1016", work_phone = "(123) 123 - 1016" },
               new Contact { id = 1017, address = "Address for Contact1017", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1017@Company.com", url = "Contact1017.Company.com", first_name = "KIRBY", last_name = "TROY", phone = "(123) 123 - 1017", work_phone = "(123) 123 - 1017" },
               new Contact { id = 1018, address = "Address for Contact1018", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1018@Company.com", url = "Contact1018.Company.com", first_name = "LEIGH", last_name = "JACK", phone = "(123) 123 - 1018", work_phone = "(123) 123 - 1018" },
               new Contact { id = 1019, address = "Address for Contact1019", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1019@Company.com", url = "Contact1019.Company.com", first_name = "WADE", last_name = "BEVERLY", phone = "(123) 123 - 1019", work_phone = "(123) 123 - 1019" },
               new Contact { id = 1020, address = "Address for Contact1020", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1020@Company.com", url = "Contact1020.Company.com", first_name = "GRADY", last_name = "SAMUEL", phone = "(123) 123 - 1020", work_phone = "(123) 123 - 1020" },
               new Contact { id = 1021, address = "Address for Contact1021", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1021@Company.com", url = "Contact1021.Company.com", first_name = "HAZEL", last_name = "MARVIN", phone = "(123) 123 - 1021", work_phone = "(123) 123 - 1021" },
               new Contact { id = 1022, address = "Address for Contact1022", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1022@Company.com", url = "Contact1022.Company.com", first_name = "MILES", last_name = "JEFFERY", phone = "(123) 123 - 1022", work_phone = "(123) 123 - 1022" },
               new Contact { id = 1023, address = "Address for Contact1023", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1023@Company.com", url = "Contact1023.Company.com", first_name = "GUY", last_name = "LEROY", phone = "(123) 123 - 1023", work_phone = "(123) 123 - 1023" },
               new Contact { id = 1024, address = "Address for Contact1024", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1024@Company.com", url = "Contact1024.Company.com", first_name = "STUART", last_name = "JAY", phone = "(123) 123 - 1024", work_phone = "(123) 123 - 1024" },
               new Contact { id = 1025, address = "Address for Contact1025", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1025@Company.com", url = "Contact1025.Company.com", first_name = "SAM", last_name = "SHIRLEY", phone = "(123) 123 - 1025", work_phone = "(123) 123 - 1025" },
               new Contact { id = 1026, address = "Address for Contact1026", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1026@Company.com", url = "Contact1026.Company.com", first_name = "TODD", last_name = "HAZEL", phone = "(123) 123 - 1026", work_phone = "(123) 123 - 1026" },
               new Contact { id = 1027, address = "Address for Contact1027", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1027@Company.com", url = "Contact1027.Company.com", first_name = "JAMES", last_name = "HOLLY", phone = "(123) 123 - 1027", work_phone = "(123) 123 - 1027" },
               new Contact { id = 1028, address = "Address for Contact1028", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1028@Company.com", url = "Contact1028.Company.com", first_name = "JOYCE", last_name = "TOM", phone = "(123) 123 - 1028", work_phone = "(123) 123 - 1028" },
               new Contact { id = 1029, address = "Address for Contact1029", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1029@Company.com", url = "Contact1029.Company.com", first_name = "ASHLEY", last_name = "RAYMOND", phone = "(123) 123 - 1029", work_phone = "(123) 123 - 1029" },
               new Contact { id = 1030, address = "Address for Contact1030", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1030@Company.com", url = "Contact1030.Company.com", first_name = "SYLVESTER", last_name = "PAUL", phone = "(123) 123 - 1030", work_phone = "(123) 123 - 1030" },
               new Contact { id = 1031, address = "Address for Contact1031", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1031@Company.com", url = "Contact1031.Company.com", first_name = "SHIRLEY", last_name = "ARTHUR", phone = "(123) 123 - 1031", work_phone = "(123) 123 - 1031" },
               new Contact { id = 1032, address = "Address for Contact1032", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1032@Company.com", url = "Contact1032.Company.com", first_name = "EDGAR", last_name = "ANTHONY", phone = "(123) 123 - 1032", work_phone = "(123) 123 - 1032" },
               new Contact { id = 1033, address = "Address for Contact1033", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1033@Company.com", url = "Contact1033.Company.com", first_name = "FRANCIS", last_name = "EARL", phone = "(123) 123 - 1033", work_phone = "(123) 123 - 1033" },
               new Contact { id = 1034, address = "Address for Contact1034", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1034@Company.com", url = "Contact1034.Company.com", first_name = "DEAN", last_name = "SAM", phone = "(123) 123 - 1034", work_phone = "(123) 123 - 1034" },
               new Contact { id = 1035, address = "Address for Contact1035", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1035@Company.com", url = "Contact1035.Company.com", first_name = "COLE", last_name = "JAMES", phone = "(123) 123 - 1035", work_phone = "(123) 123 - 1035" },
               new Contact { id = 1036, address = "Address for Contact1036", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1036@Company.com", url = "Contact1036.Company.com", first_name = "CHASE", last_name = "LOUIS", phone = "(123) 123 - 1036", work_phone = "(123) 123 - 1036" },
               new Contact { id = 1037, address = "Address for Contact1037", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1037@Company.com", url = "Contact1037.Company.com", first_name = "FLETCHER", last_name = "NICHOLAS", phone = "(123) 123 - 1037", work_phone = "(123) 123 - 1037" },
               new Contact { id = 1038, address = "Address for Contact1038", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1038@Company.com", url = "Contact1038.Company.com", first_name = "GEORGE", last_name = "AARON", phone = "(123) 123 - 1038", work_phone = "(123) 123 - 1038" },
               new Contact { id = 1039, address = "Address for Contact1039", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1039@Company.com", url = "Contact1039.Company.com", first_name = "MARSHALL", last_name = "BRENT", phone = "(123) 123 - 1039", work_phone = "(123) 123 - 1039" },
               new Contact { id = 1040, address = "Address for Contact1040", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1040@Company.com", url = "Contact1040.Company.com", first_name = "BROCK", last_name = "RAMON", phone = "(123) 123 - 1040", work_phone = "(123) 123 - 1040" },
               new Contact { id = 1041, address = "Address for Contact1041", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1041@Company.com", url = "Contact1041.Company.com", first_name = "VERA", last_name = "DANIEL", phone = "(123) 123 - 1041", work_phone = "(123) 123 - 1041" },
               new Contact { id = 1042, address = "Address for Contact1042", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1042@Company.com", url = "Contact1042.Company.com", first_name = "DREW", last_name = "CALVIN", phone = "(123) 123 - 1042", work_phone = "(123) 123 - 1042" },
               new Contact { id = 1043, address = "Address for Contact1043", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1043@Company.com", url = "Contact1043.Company.com", first_name = "KELLEY", last_name = "STACEY", phone = "(123) 123 - 1043", work_phone = "(123) 123 - 1043" },
               new Contact { id = 1044, address = "Address for Contact1044", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1044@Company.com", url = "Contact1044.Company.com", first_name = "JACK", last_name = "ANDRE", phone = "(123) 123 - 1044", work_phone = "(123) 123 - 1044" },
               new Contact { id = 1045, address = "Address for Contact1045", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1045@Company.com", url = "Contact1045.Company.com", first_name = "SHERMAN", last_name = "DERRICK", phone = "(123) 123 - 1045", work_phone = "(123) 123 - 1045" },
               new Contact { id = 1046, address = "Address for Contact1046", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1046@Company.com", url = "Contact1046.Company.com", first_name = "BOYD", last_name = "FRANCISCO", phone = "(123) 123 - 1046", work_phone = "(123) 123 - 1046" },
               new Contact { id = 1047, address = "Address for Contact1047", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1047@Company.com", url = "Contact1047.Company.com", first_name = "MARION", last_name = "MARION", phone = "(123) 123 - 1047", work_phone = "(123) 123 - 1047" },
               new Contact { id = 1048, address = "Address for Contact1048", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1048@Company.com", url = "Contact1048.Company.com", first_name = "HALEY", last_name = "ALBERT", phone = "(123) 123 - 1048", work_phone = "(123) 123 - 1048" },
               new Contact { id = 1049, address = "Address for Contact1049", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1049@Company.com", url = "Contact1049.Company.com", first_name = "YOUNG", last_name = "BRANDON", phone = "(123) 123 - 1049", work_phone = "(123) 123 - 1049" },
               new Contact { id = 1050, address = "Address for Contact1050", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1050@Company.com", url = "Contact1050.Company.com", first_name = "BOOKER", last_name = "JACOB", phone = "(123) 123 - 1050", work_phone = "(123) 123 - 1050" },
               new Contact { id = 1051, address = "Address for Contact1051", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1051@Company.com", url = "Contact1051.Company.com", first_name = "CECIL", last_name = "ANGEL", phone = "(123) 123 - 1051", work_phone = "(123) 123 - 1051" },
               new Contact { id = 1052, address = "Address for Contact1052", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1052@Company.com", url = "Contact1052.Company.com", first_name = "MANUEL", last_name = "LESLIE", phone = "(123) 123 - 1052", work_phone = "(123) 123 - 1052" },
               new Contact { id = 1053, address = "Address for Contact1053", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1053@Company.com", url = "Contact1053.Company.com", first_name = "MITCHELL", last_name = "FRANK", phone = "(123) 123 - 1053", work_phone = "(123) 123 - 1053" },
               new Contact { id = 1054, address = "Address for Contact1054", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1054@Company.com", url = "Contact1054.Company.com", first_name = "ROMAN", last_name = "JOYCE", phone = "(123) 123 - 1054", work_phone = "(123) 123 - 1054" },
               new Contact { id = 1055, address = "Address for Contact1055", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1055@Company.com", url = "Contact1055.Company.com", first_name = "FORREST", last_name = "TRACY", phone = "(123) 123 - 1055", work_phone = "(123) 123 - 1055" },
               new Contact { id = 1056, address = "Address for Contact1056", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1056@Company.com", url = "Contact1056.Company.com", first_name = "HARRISON", last_name = "WALTER", phone = "(123) 123 - 1056", work_phone = "(123) 123 - 1056" },
               new Contact { id = 1057, address = "Address for Contact1057", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1057@Company.com", url = "Contact1057.Company.com", first_name = "MIRANDA", last_name = "SHELLY", phone = "(123) 123 - 1057", work_phone = "(123) 123 - 1057" },
               new Contact { id = 1058, address = "Address for Contact1058", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1058@Company.com", url = "Contact1058.Company.com", first_name = "BURTON", last_name = "CECIL", phone = "(123) 123 - 1058", work_phone = "(123) 123 - 1058" },
               new Contact { id = 1059, address = "Address for Contact1059", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1059@Company.com", url = "Contact1059.Company.com", first_name = "JULIAN", last_name = "KYLE", phone = "(123) 123 - 1059", work_phone = "(123) 123 - 1059" },
               new Contact { id = 1060, address = "Address for Contact1060", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1060@Company.com", url = "Contact1060.Company.com", first_name = "SIMON", last_name = "ALLAN", phone = "(123) 123 - 1060", work_phone = "(123) 123 - 1060" },
               new Contact { id = 1061, address = "Address for Contact1061", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1061@Company.com", url = "Contact1061.Company.com", first_name = "LEONARD", last_name = "EDGAR", phone = "(123) 123 - 1061", work_phone = "(123) 123 - 1061" },
               new Contact { id = 1062, address = "Address for Contact1062", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1062@Company.com", url = "Contact1062.Company.com", first_name = "CHRISTIAN", last_name = "MELVIN", phone = "(123) 123 - 1062", work_phone = "(123) 123 - 1062" },
               new Contact { id = 1063, address = "Address for Contact1063", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1063@Company.com", url = "Contact1063.Company.com", first_name = "LLOYD", last_name = "GEORGE", phone = "(123) 123 - 1063", work_phone = "(123) 123 - 1063" },
               new Contact { id = 1064, address = "Address for Contact1064", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1064@Company.com", url = "Contact1064.Company.com", first_name = "OWEN", last_name = "ASHLEY", phone = "(123) 123 - 1064", work_phone = "(123) 123 - 1064" },
               new Contact { id = 1065, address = "Address for Contact1065", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1065@Company.com", url = "Contact1065.Company.com", first_name = "LINDSEY", last_name = "BENJAMIN", phone = "(123) 123 - 1065", work_phone = "(123) 123 - 1065" },
               new Contact { id = 1066, address = "Address for Contact1066", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1066@Company.com", url = "Contact1066.Company.com", first_name = "FELIX", last_name = "JOY", phone = "(123) 123 - 1066", work_phone = "(123) 123 - 1066" },
               new Contact { id = 1067, address = "Address for Contact1067", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1067@Company.com", url = "Contact1067.Company.com", first_name = "JACOB", last_name = "MANUEL", phone = "(123) 123 - 1067", work_phone = "(123) 123 - 1067" },
               new Contact { id = 1068, address = "Address for Contact1068", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1068@Company.com", url = "Contact1068.Company.com", first_name = "KAY", last_name = "PENNY", phone = "(123) 123 - 1068", work_phone = "(123) 123 - 1068" },
               new Contact { id = 1069, address = "Address for Contact1069", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1069@Company.com", url = "Contact1069.Company.com", first_name = "SCOTT", last_name = "MARCUS", phone = "(123) 123 - 1069", work_phone = "(123) 123 - 1069" },
               new Contact { id = 1070, address = "Address for Contact1070", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1070@Company.com", url = "Contact1070.Company.com", first_name = "DANIEL", last_name = "STACY", phone = "(123) 123 - 1070", work_phone = "(123) 123 - 1070" },
               new Contact { id = 1071, address = "Address for Contact1071", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1071@Company.com", url = "Contact1071.Company.com", first_name = "IRVING", last_name = "COREY", phone = "(123) 123 - 1071", work_phone = "(123) 123 - 1071" },
               new Contact { id = 1072, address = "Address for Contact1072", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1072@Company.com", url = "Contact1072.Company.com", first_name = "RAY", last_name = "GRACE", phone = "(123) 123 - 1072", work_phone = "(123) 123 - 1072" },
               new Contact { id = 1073, address = "Address for Contact1073", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1073@Company.com", url = "Contact1073.Company.com", first_name = "BRUCE", last_name = "NEIL", phone = "(123) 123 - 1073", work_phone = "(123) 123 - 1073" },
               new Contact { id = 1074, address = "Address for Contact1074", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1074@Company.com", url = "Contact1074.Company.com", first_name = "ARCHIE", last_name = "FLORA", phone = "(123) 123 - 1074", work_phone = "(123) 123 - 1074" },
               new Contact { id = 1075, address = "Address for Contact1075", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1075@Company.com", url = "Contact1075.Company.com", first_name = "ANGEL", last_name = "PATRICK", phone = "(123) 123 - 1075", work_phone = "(123) 123 - 1075" },
               new Contact { id = 1076, address = "Address for Contact1076", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1076@Company.com", url = "Contact1076.Company.com", first_name = "HENRY", last_name = "CHRISTY", phone = "(123) 123 - 1076", work_phone = "(123) 123 - 1076" },
               new Contact { id = 1077, address = "Address for Contact1077", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1077@Company.com", url = "Contact1077.Company.com", first_name = "SHANNON", last_name = "DALE", phone = "(123) 123 - 1077", work_phone = "(123) 123 - 1077" },
               new Contact { id = 1078, address = "Address for Contact1078", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1078@Company.com", url = "Contact1078.Company.com", first_name = "VAUGHN", last_name = "CLIFFORD", phone = "(123) 123 - 1078", work_phone = "(123) 123 - 1078" },
               new Contact { id = 1079, address = "Address for Contact1079", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1079@Company.com", url = "Contact1079.Company.com", first_name = "PAIGE", last_name = "KEITH", phone = "(123) 123 - 1079", work_phone = "(123) 123 - 1079" },
               new Contact { id = 1080, address = "Address for Contact1080", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1080@Company.com", url = "Contact1080.Company.com", first_name = "TERRELL", last_name = "FREDRICK", phone = "(123) 123 - 1080", work_phone = "(123) 123 - 1080" },
               new Contact { id = 1081, address = "Address for Contact1081", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1081@Company.com", url = "Contact1081.Company.com", first_name = "MARCUS", last_name = "VERA", phone = "(123) 123 - 1081", work_phone = "(123) 123 - 1081" },
               new Contact { id = 1082, address = "Address for Contact1082", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1082@Company.com", url = "Contact1082.Company.com", first_name = "FRANKLIN", last_name = "VERNON", phone = "(123) 123 - 1082", work_phone = "(123) 123 - 1082" },
               new Contact { id = 1083, address = "Address for Contact1083", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1083@Company.com", url = "Contact1083.Company.com", first_name = "BRANDON", last_name = "HERBERT", phone = "(123) 123 - 1083", work_phone = "(123) 123 - 1083" },
               new Contact { id = 1084, address = "Address for Contact1084", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1084@Company.com", url = "Contact1084.Company.com", first_name = "ANDRES", last_name = "ANGELO", phone = "(123) 123 - 1084", work_phone = "(123) 123 - 1084" },
               new Contact { id = 1085, address = "Address for Contact1085", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1085@Company.com", url = "Contact1085.Company.com", first_name = "WILLARD", last_name = "GERARD", phone = "(123) 123 - 1085", work_phone = "(123) 123 - 1085" },
               new Contact { id = 1086, address = "Address for Contact1086", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1086@Company.com", url = "Contact1086.Company.com", first_name = "MAYNARD", last_name = "HOMER", phone = "(123) 123 - 1086", work_phone = "(123) 123 - 1086" },
               new Contact { id = 1087, address = "Address for Contact1087", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1087@Company.com", url = "Contact1087.Company.com", first_name = "CARROLL", last_name = "JULIAN", phone = "(123) 123 - 1087", work_phone = "(123) 123 - 1087" },
               new Contact { id = 1088, address = "Address for Contact1088", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1088@Company.com", url = "Contact1088.Company.com", first_name = "ROSA", last_name = "ROSA", phone = "(123) 123 - 1088", work_phone = "(123) 123 - 1088" },
               new Contact { id = 1089, address = "Address for Contact1089", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1089@Company.com", url = "Contact1089.Company.com", first_name = "CRAIG", last_name = "DENNIS", phone = "(123) 123 - 1089", work_phone = "(123) 123 - 1089" },
               new Contact { id = 1090, address = "Address for Contact1090", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1090@Company.com", url = "Contact1090.Company.com", first_name = "COURTNEY", last_name = "CHESTER", phone = "(123) 123 - 1090", work_phone = "(123) 123 - 1090" },
               new Contact { id = 1091, address = "Address for Contact1091", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1091@Company.com", url = "Contact1091.Company.com", first_name = "ROSARIO", last_name = "HUBERT", phone = "(123) 123 - 1091", work_phone = "(123) 123 - 1091" },
               new Contact { id = 1092, address = "Address for Contact1092", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1092@Company.com", url = "Contact1092.Company.com", first_name = "JOY", last_name = "WESLEY", phone = "(123) 123 - 1092", work_phone = "(123) 123 - 1092" },
               new Contact { id = 1093, address = "Address for Contact1093", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1093@Company.com", url = "Contact1093.Company.com", first_name = "GILBERT", last_name = "LANCE", phone = "(123) 123 - 1093", work_phone = "(123) 123 - 1093" },
               new Contact { id = 1094, address = "Address for Contact1094", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1094@Company.com", url = "Contact1094.Company.com", first_name = "BRADFORD", last_name = "SHANNON", phone = "(123) 123 - 1094", work_phone = "(123) 123 - 1094" },
               new Contact { id = 1095, address = "Address for Contact1095", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1095@Company.com", url = "Contact1095.Company.com", first_name = "LANCE", last_name = "ALFONSO", phone = "(123) 123 - 1095", work_phone = "(123) 123 - 1095" },
               new Contact { id = 1096, address = "Address for Contact1096", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1096@Company.com", url = "Contact1096.Company.com", first_name = "KIRK", last_name = "LYNN", phone = "(123) 123 - 1096", work_phone = "(123) 123 - 1096" },
               new Contact { id = 1097, address = "Address for Contact1097", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1097@Company.com", url = "Contact1097.Company.com", first_name = "DENNIS", last_name = "ANDRES", phone = "(123) 123 - 1097", work_phone = "(123) 123 - 1097" },
               new Contact { id = 1098, address = "Address for Contact1098", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1098@Company.com", url = "Contact1098.Company.com", first_name = "PAUL", last_name = "BRUCE", phone = "(123) 123 - 1098", work_phone = "(123) 123 - 1098" },
               new Contact { id = 1099, address = "Address for Contact1099", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1099@Company.com", url = "Contact1099.Company.com", first_name = "ERVIN", last_name = "DAMON", phone = "(123) 123 - 1099", work_phone = "(123) 123 - 1099" },
               new Contact { id = 1100, address = "Address for Contact1100", city = "NYC", state = "NY", zip = "10001", company_name = "Company", dob = Utilities.GenerateRandomDOBForAge(24, 30), email = "Contact1100@Company.com", url = "Contact1100.Company.com", first_name = "COREY", last_name = "RODERICK", phone = "(123) 123 - 1100", work_phone = "(123) 123 - 1100" }
               );
            db.SaveChanges();

            return StatusCode(HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ContactExists(int id)
        {
            return db.Contacts.Count(e => e.id == id) > 0;
        }
    }
}