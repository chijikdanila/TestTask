namespace TestTask.Entities
{
    class RandomString
    {
        public string Date { get; set; }
        public string LatinString { get; set; }
        public string RussianString { get; set; }
        public int Number { get; set; }
        public float Float { get; set; }

        public RandomString(string _date, string _latin, string _russian, int _number, float _float)
        {
            Date = _date;
            LatinString = _latin;
            RussianString = _russian;
            Number = _number;
            Float = _float;
        }

        public override string ToString()
        {
            return $"{Date}||{LatinString}||{RussianString}||{Number}||{Float}||";
        }
    }
}
