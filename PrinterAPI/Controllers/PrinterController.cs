using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Printer;
using System.Text.RegularExpressions;

namespace People.Controllers {



    [ApiController]
    [Route("[controller]")]
    public class PrinterController : ControllerBase {

        private readonly ILogger<PrinterController> _logger;
        private SqliteConnection sqliteConnection = new SqliteConnection("Data Source=PrinterDB.db");

        //IP Regex Patern
        private Regex vIPRegEx = new Regex("(\b25[0-5]|\b2[0-4][0-9]|\b[01]?[0-9][0-9]?)(.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}");


        public PrinterController(ILogger<PrinterController> logger) {
            _logger = logger;
        }

        //============================================= doGET ==================================
        [HttpGet]
        public PrintersListJSON Get() {  //Get List of all Printers
            PrintersListJSON printersListJSON = new();
            using (var connection = sqliteConnection) {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"Select ID, PrinterName, PrinterIP, Status  Printer From Printers";

                using (var reader = command.ExecuteReader()) {
                    List<PrinterJSON> printers = new();
                    while (reader.Read()) {

                        printers.Add(new PrinterJSON(reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetInt32(3)
                            ));
                    }
                    printersListJSON.Printers = printers;
                }
            }
            return printersListJSON;
        }


        [HttpGet]
        [Route("printer/id")]//GET Printer by ID
        public PrinterJSON? GetByID([System.Web.Http.FromUri] int ID) {
            PrinterJSON printerJSON;
            using (var connection = sqliteConnection) {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"Select ID, PrinterName, PrinterIP, Status  Printer From Printers WHERE ID = $id";
                command.Parameters.AddWithValue("$id", ID);

                using (var reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        printerJSON = (new PrinterJSON(reader.GetInt32(0),
                             reader.GetString(1),
                             reader.GetString(2),
                             reader.GetInt32(3)
                             ));
                    }
                    else {
                        return printerJSON = new();
                    }
                }

                return printerJSON;
            }
        }




        //============================================= doDELETE ==================================
        [HttpDelete]
        [Route("removePrinter/ID")] //Delete by ID
        public System.Net.HttpStatusCode DeleteByID([System.Web.Http.FromUriAttribute] int id) {
            return DeletePrinter(id);
        }

        [HttpDelete]
        [Route("removePrinter/Name")]//Delete by Name
        public System.Net.HttpStatusCode DeleteByName([System.Web.Http.FromUri] String Name) {
            return DeletePrinter(Name);
        }

        [HttpDelete]
        [Route("removePrinter/IP")] //Delete by IP
        public System.Net.HttpStatusCode DeleteByIP([System.Web.Http.FromUri] String IP) {
            return DeletePrinter(IP);
        }

        private System.Net.HttpStatusCode DeletePrinter(int id) {
            return DeletePrinter(id, "");
        }
        private System.Net.HttpStatusCode DeletePrinter(String Name) {
            return DeletePrinter(-1, Name);
        }

        private System.Net.HttpStatusCode DeletePrinter(int id, String NameOrIP) {
            try {
                using (var connection = sqliteConnection) {
                    connection.Open();
                    var command = connection.CreateCommand();

                    if (id != -1) {
                        command.CommandText = @"DELETE FROM Printers WHERE ID = $id";
                        command.Parameters.AddWithValue("$id", id);
                    }
                    else if (vIPRegEx.IsMatch(NameOrIP)) {
                        command.CommandText = @"DELETE FROM Printers WHERE PrinterIP = $ip";
                        command.Parameters.AddWithValue("$ip", NameOrIP);
                    }
                    else if (NameOrIP != "") {
                        command.CommandText = @"DELETE FROM Printers WHERE PrinterName = $name";
                        command.Parameters.AddWithValue("$name", NameOrIP);
                    }
                    else {
                        return System.Net.HttpStatusCode.NotFound;
                    }
                    command.ExecuteNonQuery();
                    int i = command.ExecuteNonQuery();

                    if (i == 0) return System.Net.HttpStatusCode.NotFound;

                    return System.Net.HttpStatusCode.OK;
                }
            }
            catch (Exception ignored) {
                return System.Net.HttpStatusCode.BadRequest;
            }
        }




        //============================================= doPUT ==================================

        [HttpPut]
        [Route("addPrinter")]
        public System.Net.HttpStatusCode AddPrinter(PrinterJSON printerJSON) {
            try {

                if (!vIPRegEx.IsMatch(printerJSON.PrinterIP)) { //Match IP to Regex Pattern

                    using (var connection = sqliteConnection) {
                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = @"INSERT INTO Printers (PrinterName, PrinterIP, Status) VALUES ( $name, $ip, $status )";
                        command.Parameters.AddWithValue("$name", printerJSON.PrinterName);
                        command.Parameters.AddWithValue("$ip", printerJSON.PrinterIP);
                        command.Parameters.AddWithValue("$status", (printerJSON.StatusActive) ? 1 : 0);

                        command.ExecuteNonQuery();

                        return System.Net.HttpStatusCode.OK;
                    }
                }
                else {
                    return System.Net.HttpStatusCode.BadRequest;
                }
            }
            catch (Exception e) {
                return System.Net.HttpStatusCode.BadRequest;
            }
        }
        
        [HttpPut]
        [Route("updatePrinterByID")]
        public System.Net.HttpStatusCode UpdatePrinter(PrinterJSON printerJSON) {
            try {

                if (!vIPRegEx.IsMatch(printerJSON.PrinterIP)) {//Match IP to Regex Pattern

                    using (var connection = sqliteConnection) {
                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = @"UPDATE Printers SET PrinterName = $name,  PrinterIP = $ip,  Status = $status WHERE ID = $id";
                        command.Parameters.AddWithValue("$name", printerJSON.PrinterName);
                        command.Parameters.AddWithValue("$ip", printerJSON.PrinterIP);
                        command.Parameters.AddWithValue("$status", (printerJSON.StatusActive) ? 1 : 0);
                        command.Parameters.AddWithValue("$id", printerJSON.ID);

                        command.ExecuteNonQuery();

                        return System.Net.HttpStatusCode.OK;
                    }
                }
                else {
                    return System.Net.HttpStatusCode.BadRequest;
                }
            }
            catch (Exception e) {
                return System.Net.HttpStatusCode.BadRequest;
            }
        }
    }
}
