namespace Printer {

    public record PrintersListJSON {
        public List<PrinterJSON>? Printers { get; set; }
    }

    public class PrinterJSON {

        public PrinterJSON(int iD, String printerName, String printerIP, int statusActive) {
            ID = iD;
            PrinterName = printerName;
            PrinterIP = printerIP;
            StatusActive = (statusActive == 1);
        }
        public PrinterJSON(int iD, String printerName, String printerIP, Boolean statusActive) {
            ID = iD;
            PrinterName = printerName;
            PrinterIP = printerIP;
            StatusActive = statusActive;
        }

        public PrinterJSON(String printerName, String printerIP, Boolean statusActive) {
            PrinterName = printerName;
            PrinterIP = printerIP;
            StatusActive = statusActive;
        }

        public PrinterJSON(String printerName, String printerIP) {
            PrinterName = printerName;
            PrinterIP = printerIP;
            StatusActive = true;
        }

        public PrinterJSON() {
            ID = -1;
            PrinterName = "";
            PrinterIP = "";
            StatusActive = false;

        }

        public int ID { get; set; }

        public String PrinterName { get; set; }
        public String PrinterIP { get; set; }
        public bool StatusActive { get; set; }
    }
}
