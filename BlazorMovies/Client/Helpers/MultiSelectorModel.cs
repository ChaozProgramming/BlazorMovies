namespace BlazorMovies.Client.Helpers
{
    public struct MultiSelectorModel
    {
        public MultiSelectorModel(string _key, string _value)
        { 
            Key = _key;
            Value = _value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
        //public bool Selectable { get; set; }    //todo
    }
}
