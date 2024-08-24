namespace TestTask.Entities
{
    class BalanceSheet
    {
        public int SheetNumber { get; set; }
        public double IncomingActive { get; set; }
        public double IncomingPassive { get; set; }
        public double CircuitDebet { get; set; }
        public double CircuitCredit { get; set; }
        public double OutcomingActive { get; set; }
        public double OutcomingPassive { get; set; }
    }
}
