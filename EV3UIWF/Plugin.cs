namespace EV3UIWF
{
    internal class Plugin
    {
        public Plugin(string name, string fullFileName)
        {
            Name = name;
            FullFileName = fullFileName;
        }

        public string Name { get; set; }
        public string FullFileName { get; set; }
        public object Data { get; set; }
    }
}
